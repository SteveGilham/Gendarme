namespace AltCode.Gendarme

module Targets =

  // latest tweet -- https://twitter.com/stevegilham1/status/1494237884659998722

  open System
  open System.IO
  open System.Xml.Linq

  open Fake.Core
  open Fake.Core.TargetOperators
  open Fake.DotNet
  open Fake.DotNet.NuGet.NuGet
  open Fake.DotNet.Testing.NUnit3
  open Fake.Testing
  open Fake.DotNet.Testing
  open Fake.IO
  open Fake.IO.FileSystemOperators
  open Fake.IO.Globbing.Operators
  open Fake.Tools.Git

  open AltCode.Fake.DotNet
  open AltCoverFake.DotNet.DotNet
  open AltCoverFake.DotNet.Testing

  open NUnit.Framework

  let mutable Copyright = String.Empty
  let mutable Version = "0.0.0.0"

  let consoleBefore =
    (Console.ForegroundColor, Console.BackgroundColor)

  let programFiles =
    Environment.environVar "ProgramFiles"

  let dotnetPath =
    "dotnet"
    |> Fake.Core.ProcessUtils.tryFindFileOnPath

  let dotnetOptions (o: DotNet.Options) =
    match dotnetPath with
    | Some f -> { o with DotNetCliPath = f }
    | None -> o

  let dotnetInfo =
    DotNet.exec (fun o -> dotnetOptions (o.WithRedirectOutput true)) "" "--info"

  let dotnetSdkPath =
    dotnetInfo.Results
    |> Seq.filter (fun x -> x.IsError |> not)
    |> Seq.map (fun x -> x.Message)
    |> Seq.tryFind (fun x -> x.Contains "Base Path:")
    |> Option.map (fun x -> x.Replace("Base Path:", "").TrimStart())

  let refdir =
    dotnetSdkPath
    |> Option.map (fun path -> path @@ "ref")

  let currentBranch =
    "."
    |> Path.getFullName
    |> Information.getBranchName

  let badge =
    if
      currentBranch.StartsWith("release/", StringComparison.Ordinal)
      && (currentBranch.Contains("pre-release") |> not)
    then
      String.Empty
    else
      "-pre-release"

  let toolPackages =
    let xml =
      "./Directory.Packages.props"
      |> Path.getFullName
      |> XDocument.Load

    xml.Descendants()
    |> Seq.filter (fun x -> x.Attribute(XName.Get("Include")) |> isNull |> not)
    |> Seq.map (fun x ->
      (x.Attribute(XName.Get("Include")).Value, x.Attribute(XName.Get("Version")).Value))
    |> Map.ofSeq

  let packageVersion (p: string) =
    p.ToLowerInvariant() + "/" + (toolPackages.Item p)

  let nunitConsole =
    ("./packages/"
     + (packageVersion "NUnit.ConsoleRunner")
     + "/tools/nunit3-console.exe")
    |> Path.getFullName

  let altcover =
    ("./packages/"
     + (packageVersion "altcover")
     + "/tools/net472/AltCover.exe")
    |> Path.getFullName

  let (fxcop, dixon) =
    if Environment.isWindows then
      let expect =
        "./packages/fxcop/FxCopCmd.exe"
        |> Path.getFullName

      if File.Exists expect then
        (Some expect,
         Some(
           "./packages/fxcop/DixonCmd.exe"
           |> Path.getFullName
         ))
      else
        (None, None)
    else
      (None, None)

  let frameworkAltcover =
    Fake.DotNet.ToolType.CreateFullFramework()

  let nugetCache =
    Path.Combine(
      Environment.GetFolderPath Environment.SpecialFolder.UserProfile,
      ".nuget/packages"
    )

  let AltCoverFilter (p: Primitive.PrepareOptions) =
    { p with
        //MethodFilter = "WaitForExitCustom" :: (p.MethodFilter |> Seq.toList)
        AssemblyExcludeFilter =
          @"Examples\."
          :: @"NUnit3\."
          :: @"Tests\."
          :: (p.AssemblyExcludeFilter |> Seq.toList)
        AssemblyFilter =
          "FSharp"
          :: @"Test\.Rules"
          :: (p.AssemblyFilter |> Seq.toList)
        LocalSource = true
        TypeFilter =
          [ @"System\."; "Microsoft" ]
          @ (p.TypeFilter |> Seq.toList) }

  let cliArguments =
    { MSBuild.CliArguments.Create() with
        ConsoleLogParameters = []
        DistributedLoggers = None
        Properties = [ ("CheckEolTargetFramework", "false") ]
        DisableInternalBinLog = true }

  let withWorkingDirectoryVM dir o =
    { dotnetOptions o with
        WorkingDirectory = Path.getFullName dir
        Verbosity = Some DotNet.Verbosity.Minimal }

  let withWorkingDirectoryOnly dir o =
    { dotnetOptions o with
        WorkingDirectory = Path.getFullName dir }

  let withCLIArgs (o: Fake.DotNet.DotNet.TestOptions) =
    { o with MSBuildParams = cliArguments }

  let withMSBuildParams (o: Fake.DotNet.DotNet.BuildOptions) =
    { o with MSBuildParams = cliArguments }

  let defaultTestOptions fwk common (o: DotNet.TestOptions) =
    { o.WithCommon(
        (fun o2 ->
          { o2 with
              Verbosity = Some DotNet.Verbosity.Normal })
        >> common
      ) with
        NoBuild = true
        Framework = fwk // Some "netcoreapp3.0"
        Configuration = DotNet.BuildConfiguration.Debug }

  let dotnetBuildRelease proj =
    DotNet.build
      (fun p ->
        { p.WithCommon dotnetOptions with
            Configuration = DotNet.BuildConfiguration.Release
            NoRestore = false }
        |> withMSBuildParams)
      (Path.GetFullPath proj)

  let dotnetBuildDebug proj =
    DotNet.build
      (fun p ->
        { p.WithCommon dotnetOptions with
            Configuration = DotNet.BuildConfiguration.Debug
            NoRestore = false }
        |> withMSBuildParams)
      (Path.GetFullPath proj)

  let misses = ref 0

  let uncovered (path: string) =
    misses.Value <- 0

    !!path
    |> Seq.collect (fun f ->
      let xml = XDocument.Load f

      xml.Descendants(XName.Get("Uncoveredlines"))
      |> Seq.filter (fun x ->
        match String.IsNullOrWhiteSpace x.Value with
        | false -> true
        | _ ->
          sprintf "No coverage from '%s'" f
          |> Trace.traceImportant

          misses.Value <- 1 + misses.Value
          false)
      |> Seq.map (fun e ->
        let coverage = e.Value

        match Int32.TryParse coverage with
        | (false, _) ->
          printfn "%A" xml

          Assert.Fail(
            "Could not parse uncovered line value '"
            + coverage
            + "'"
          )

          0
        | (_, numeric) ->
          printfn "%s : %A" (f |> Path.GetDirectoryName |> Path.GetFileName) numeric
          numeric))
    |> Seq.toList

  let commitHash =
    Information.getCurrentSHA1 (".")

  let infoV =
    Information.showName "." commitHash

  printfn "Build at %A" infoV

  let _Target s f =
    let doTarget s f =
      let banner x =
        printfn ""
        printfn " ****************** %s ******************" s
        f x

      Target.create s banner

    Target.description s
    doTarget s f

    let s2 = "Replay" + s
    Target.description s2
    doTarget s2 f

  // Preparation

  //_Target "Preparation" ignore

  let Clean =
    (fun _ ->
      printfn "Cleaning the build and deploy folders"
      Actions.Clean())

  let SetVersion =
    (fun _ ->
      Directory.ensure "./_Generated"

      let hack =
        """namespace AltCover
  module SolutionRoot =
    let location = """
        + "\"\"\""
        + (Path.getFullName ".")
        + "\"\"\""

      let path = "_Generated/SolutionRoot.fs"
      File.WriteAllText(path, hack)

      let now = DateTime.Now

      let time =
        now
          .ToString("HHmmss")
          .Substring(0, 5)
          .TrimStart('0')

      let y0 = now.Year
      let m0 = now.Month
      let d0 = now.Day
      let y = y0.ToString()
      let m = m0.ToString()
      let d = d0.ToString()
      Version <- y + "." + m + "." + d + "." + time

      let copy =
        sprintf "© 2010-%d by Steve Gilham <SteveGilham@users.noreply.github.com>" y0

      let copy2 =
        sprintf "Copyright (C) 2005-%d Novell, Inc. and contributors" y0

      Copyright <- "Copyright " + copy

      let rn =
        File.ReadAllLines "./ReleaseNotes.md"

      let tag =
        rn
        |> Array.findIndex (fun l -> l.StartsWith("# ", StringComparison.Ordinal))

      rn.[tag] <- String.Format("# {0}", Version)
      printfn "%s" rn.[tag]
      File.WriteAllLines("./_Generated/ReleaseNotes.md", rn)

      // make the first one `true` if we ever want the static fields
      let config =
        AssemblyInfoFileConfig(false, false, "Gendarme")

      AssemblyInfoFile.create
        "./_Generated/AssemblyStaticInfo.fs"
        [ AssemblyInfo.Product "altcode.gendarme"
          AssemblyInfo.Version Version
          AssemblyInfo.FileVersion Version
          AssemblyInfo.InformationalVersion(commitHash + " " + currentBranch)
          AssemblyInfo.Company "Steve Gilham"
          AssemblyInfo.Trademark ""
          AssemblyInfo.CLSCompliant true
          AssemblyInfo.ComVisible false
          AssemblyInfo.Copyright copy
          AssemblyInfo.Metadata(
            "RepositoryUrl",
            "https://github.com/SteveGilham/Gendarme"
          )
          AssemblyInfo.Metadata("CommitHash", commitHash)
          AssemblyInfo.Metadata("Branch", currentBranch) ]
        (Some config)

      AssemblyInfoFile.create
        "./_Generated/AssemblyStaticInfo.cs"
        [ AssemblyInfo.Title "Gendarme"
          AssemblyInfo.Version Version
          AssemblyInfo.FileVersion Version
          AssemblyInfo.InformationalVersion(commitHash + " " + currentBranch)
          AssemblyInfo.Company "Novell, Inc."
          AssemblyInfo.Trademark ""
          AssemblyInfo.Description "Rule-based assembly analyzer"
          AssemblyInfo.CLSCompliant false
          AssemblyInfo.ComVisible false
          AssemblyInfo.Copyright copy2
          AssemblyInfo.Metadata(
            "RepositoryUrl",
            "https://github.com/SteveGilham/Gendarme"
          )
          AssemblyInfo.Metadata("CommitHash", commitHash)
          AssemblyInfo.Metadata("Branch", currentBranch) ]
        (Some config)

      AssemblyInfoFile.create
        "./_Generated/MockerAssemblyStaticInfo.cs"
        [ AssemblyInfo.Title "Gendarme"
          AssemblyInfo.Version "1.0.0.0"
          AssemblyInfo.FileVersion Version
          AssemblyInfo.InformationalVersion(commitHash + " " + currentBranch)
          AssemblyInfo.Company "Novell, Inc."
          AssemblyInfo.Trademark ""
          AssemblyInfo.Description "Rule-based assembly analyzer"
          AssemblyInfo.CLSCompliant false
          AssemblyInfo.ComVisible false
          AssemblyInfo.Copyright copy2
          AssemblyInfo.Metadata(
            "RepositoryUrl",
            "https://github.com/SteveGilham/Gendarme"
          )
          AssemblyInfo.Metadata("CommitHash", commitHash)
          AssemblyInfo.Metadata("Branch", currentBranch) ]
        (Some config))

  // Basic compilation

  //_Target "Compilation" ignore

  let BuildRelease =
    (fun _ ->
      "./gendarme/gendarme-win.slnx"
      |> dotnetBuildRelease

      let publish =
        Path.getFullName "./_Publish.Globalization"

      DotNet.publish
        (fun options ->
          { options with
              OutputPath = Some publish
              Configuration = DotNet.BuildConfiguration.Release
              MSBuildParams =
                { options.MSBuildParams with
                    ConsoleLogParameters = []
                    DistributedLoggers = None
                    DisableInternalBinLog = true
                    Properties = options.MSBuildParams.Properties }
              Framework = Some "netstandard2.0" })
        "./gendarme/rules/Gendarme.Rules.Globalization/Gendarme.Rules.Globalization.csproj")

  let BuildDebug =
    (fun _ -> "./gendarme/gendarme-win.slnx" |> dotnetBuildDebug)

  //_Target "UnitTest" ignore

  let FxCop =
    (fun _ ->
      Directory.ensure "./_Reports"

      let dumpSuppressions (report: String) =
        let x = XDocument.Load report

        let messages =
          x.Descendants(XName.Get "Message")

        messages
        |> Seq.iter (fun m ->
          let mpp = m.Parent.Parent
          let target = mpp.Name.LocalName

          let tname =
            mpp.Attribute(XName.Get "Name").Value

          let (text, fqn) =
            match target with
            | "Namespace" -> ("namespace", tname)
            | "Resource" -> ("resource", tname)
            | "File"
            | "Module" -> ("module", String.Empty)
            | "Type" ->
              let spp = mpp.Parent.Parent

              ("type",
               spp.Attribute(XName.Get "Name").Value
               + "."
               + tname)
            | _ ->
              let spp = mpp.Parent.Parent
              let sp4 = spp.Parent.Parent

              ("member",
               sp4.Attribute(XName.Get "Name").Value
               + "."
               + spp.Attribute(XName.Get "Name").Value
               + "."
               + tname)

          let text2 = "[<assembly: SuppressMessage("

          let id = m.Attribute(XName.Get "Id")

          let text3 =
            (if id |> isNull |> not then
               ", MessageId=\"" + id.Value + "\""
             else
               String.Empty)
            + ", Justification=\"\")>]"

          let category =
            m.Attribute(XName.Get "Category").Value

          let checkId =
            m.Attribute(XName.Get "CheckId").Value

          let name =
            m.Attribute(XName.Get "TypeName").Value

          let finish t t2 =
            let t5 =
              t2
              + "\""
              + category
              + "\", \""
              + checkId
              + ":"
              + name
              + "\""

            if t |> isNull || t = "module" then
              t5 + text3
            else
              t5
              + ", Scope=\""
              + t
              + "\", Target=\""
              + fqn
              + "\""
              + text3

          printfn "%s" (finish text text2))

      let deprecatedRules =
        [ "-Microsoft.Usage#CA2202" // double dispose
          "-Microsoft.Security#CA2104" ] // // :DoNotDeclareReadOnlyMutableReferenceTypes"

      let gendarmeRules =
        [ "-Microsoft.Design#CA1002" // :DoNotExposeGenericLists"
          "-Microsoft.Design#CA1011" // :ConsiderPassingBaseTypesAsParameters"
          "-Microsoft.Design#CA1016" // :MarkAssembliesWithAssemblyVersion"
          "-Microsoft.Design#CA1021" //:AvoidOutParameters"
          "-Microsoft.Design#CA1028" // :EnumStorageShouldBeInt32"
          "-Microsoft.Design#CA1031" // :DoNotCatchGeneralExceptionTypes"
          "-Microsoft.Design#CA1051" //:DoNotDeclareVisibleInstanceFields"
          "-Microsoft.Design#CA1062" //:Validate arguments of public methods" -- candidate
          "-Microsoft.Maintainability#CA1502" //:AvoidExcessiveComplexity" -- candidate
          "-Microsoft.Usage#CA1801" // :ReviewUnusedParameters"
          "-Microsoft.Globalization#CA1305" // :SpecifyIFormatProvider"
          "-Microsoft.Globalization#CA1307" // :SpecifyStringComparison"
          "-Microsoft.Performance#CA1822" // :MarkMembersAsStatic"
          "-Microsoft.Usage#CA2208" ] // :InstantiateArgumentExceptionsCorrectly"

      let nonFsharpRules =
        [ "-Microsoft.Design#CA1006" // nested generics
          "-Microsoft.Design#CA1034" // nested classes being visible
          "-Microsoft.Naming#CA1709" // defer to the Gendarme casing rule for implicit 'a
          "-Microsoft.Naming#CA1715" // defer to the Gendarme naming rule for implicit 'a
          "-Microsoft.Usage#CA2235" // closures being serializable
          "-Microsoft.Maintainability#CA1506" ] // AvoidExcessiveClassCoupling

      let standardRules =
        [ "-Microsoft.Design#CA1020" // small namespaces
          "-Microsoft.Naming#CA1702" // :CompoundWordsShouldBeCasedCorrectly" // too opinionated
          "-Microsoft.Naming#CA1704" // :IdentifiersShouldBeSpelledCorrectly"
          "-Microsoft.Naming#CA2204" // Literals should be spelled correctly
          "-Microsoft.Usage#CA2243:AttributeStringLiteralsShouldParseCorrectly" ]

      let defaultFSharpRules =
        List.concat
          [ deprecatedRules
            gendarmeRules
            standardRules
            nonFsharpRules ]

      let defaultCSharpRules =
        List.concat
          [ deprecatedRules
            gendarmeRules
            standardRules
            [ "-Microsoft.Design#CA1026:DefaultParametersShouldNotBeUsed" ] ]

      let dd =
        toolPackages
        |> Map.toSeq
        |> Seq.map (fun (k, v) -> k.ToLowerInvariant(), v)
        |> Map.ofSeq

      let ddItem x =
        try
          dd.Item x
        with _ ->
          printfn "Failed to get %A" x
          reraise ()

      try
        [ Path.GetFullPath "./_Binaries/gendarme/Debug+AnyCPU/net472/gendarme.exe" ]
        |> FxCop.run
          { FxCop.Params.Create() with
              WorkingDirectory = "."
              DependencyDirectories =
                [ nugetCache
                  @@ "mono.cecil/"
                     + (ddItem "mono.cecil")
                     + "/lib/netstandard2.0"
                  nugetCache
                  @@ "fsharp.core/"
                     + (ddItem "fsharp.core")
                     + "/lib/netstandard2.0" ]
              ToolPath = Option.get fxcop
              UseGAC = true
              Verbose = false
              ReportFileName = "_Reports/FxCopReport.xml"
              Types = []
              Rules = defaultCSharpRules
              FailOnError = FxCop.ErrorLevel.Warning
              IgnoreGeneratedCode = true }
      with _ ->
        dumpSuppressions "_Reports/FxCopReport.xml"
        reraise ()

      try
        [ Path.GetFullPath
            "./_Binaries/CecilExtensions/Debug+AnyCPU/netstandard2.0/CecilExtensions.dll" ]
        |> FxCop.run
          { FxCop.Params.Create() with
              WorkingDirectory = "."
              DependencyDirectories =
                [ nugetCache
                  @@ "mono.cecil/"
                     + (ddItem "mono.cecil")
                     + "/lib/netstandard2.0"
                  nugetCache
                  @@ "fsharp.core/"
                     + (ddItem "fsharp.core")
                     + "/lib/netstandard2.0" ]

              ToolPath = Option.get dixon
              PlatformDirectory = Option.get refdir
              UseGAC = true
              Verbose = false
              ReportFileName = "_Reports/FxCopReport.xml"
              Types = []
              Rules = defaultFSharpRules
              FailOnError = FxCop.ErrorLevel.Warning
              IgnoreGeneratedCode = true }

      with _ ->
        dumpSuppressions "_Reports/FxCopReport.xml"
        reraise ()

      try
        !!("./_Binaries/AltCode.*/Debug+AnyCPU/netstandard2.0/AltCode.*.dll")
        |> Seq.map Path.GetFullPath
        |> Seq.distinctBy Path.GetFileName
        |> Seq.toList
        |> FxCop.run
          { FxCop.Params.Create() with
              WorkingDirectory = "."
              DependencyDirectories =
                [ nugetCache
                  @@ "mono.cecil/"
                     + (ddItem "mono.cecil")
                     + "/lib/netstandard2.0"
                  nugetCache
                  @@ "fsharp.core/"
                     + (ddItem "fsharp.core")
                     + "/lib/netstandard2.0" ]
              ToolPath = Option.get dixon
              PlatformDirectory = Option.get refdir
              UseGAC = true
              Verbose = false
              ReportFileName = "_Reports/FxCopReport.xml"
              Types = []
              Rules = defaultFSharpRules
              FailOnError = FxCop.ErrorLevel.Warning
              IgnoreGeneratedCode = true }
      with _ ->
        dumpSuppressions "_Reports/FxCopReport.xml"
        reraise ()

      let targets =
        !!("./_Binaries/Gendarme.*/Debug+AnyCPU/netstandard2.0/Gendarme.*.dll")
        |> Seq.map Path.GetFullPath
        |> Seq.distinctBy Path.GetFileName
        |> Seq.toList

      try
        targets
        |> FxCop.run
          { FxCop.Params.Create() with
              WorkingDirectory = "."
              DependencyDirectories =
                [ nugetCache
                  @@ "mono.cecil/"
                     + (ddItem "mono.cecil")
                     + "/lib/netstandard2.0"
                  nugetCache
                  @@ "system.resources.extensions/"
                     + (ddItem "system.resources.extensions")
                     + "/lib/netstandard2.0" ]
              ToolPath = Option.get dixon
              PlatformDirectory = Option.get refdir
              UseGAC = true
              Verbose = false
              ReportFileName = "_Reports/FxCopReport.xml"
              Types = []
              Rules = defaultCSharpRules
              FailOnError = FxCop.ErrorLevel.Warning
              IgnoreGeneratedCode = true }
      with _ ->
        dumpSuppressions "_Reports/FxCopReport.xml"
        reraise ())

  let JustUnitTest =
    (fun _ ->
      Directory.ensure "./_Reports"

      !!(@"_Binaries/Test.*/Debug+AnyCPU/net472/Test.*.dll")
      |> Seq.filter (fun p ->
        (p |> Path.GetFileNameWithoutExtension)
        <> "Test.Rules")
      |> Seq.iter (fun p ->
        let tname =
          Path.GetFileNameWithoutExtension p

        let nunitparams =
          { NUnit3Defaults with
              ToolPath = nunitConsole
              WorkingDir = "."
              ResultSpecs = [ "./_Reports/JustUnitTestReport." + tname + ".xml" ] }

        let nunitcmd =
          NUnit3.buildArgs nunitparams [ p ]

        let result =
          CreateProcess.fromRawCommandLine nunitConsole nunitcmd
          |> CreateProcess.withWorkingDirectory "."
          |> CreateProcess.withFramework
          |> Proc.run

        // while fixing
        let maxFail =
          match tname with
          | "Test.Framework" -> 2
          | "Test.Rules.Smells" -> 2
          | _ -> 0

        Assert.That(
          result.ExitCode,
          Is
            .GreaterThanOrEqualTo(0)
            .And.LessThanOrEqualTo(maxFail),
          "Unexpected failures in " + tname
        )

      ))

  let UnitTestDotNet =
    (fun _ ->
      Directory.ensure "./_Reports"

      !!(@"./**/Test.*.*sproj")
      |> Seq.filter (fun p ->
        (p |> Path.GetFileNameWithoutExtension)
        <> "Test.Rules")
      |> Seq.iter (fun proj ->
        try
          DotNet.test
            (fun p ->
              { p.WithCommon dotnetOptions with
                  Configuration = DotNet.BuildConfiguration.Debug
                  Framework = Some "net8.0"
                  NoBuild = true }
              |> withCLIArgs)
            proj
        with x -> // while fixing
          match Path.GetFileNameWithoutExtension proj with
          | "Test.Framework"
          | "Test.Rules.Smells" -> printfn "%A" x
          | _ -> reraise ()))

  //_Target "Coverage" ignore

  let UnitTestWithAltCoverRunner =
    (fun _ ->
      let reports = Path.getFullName "./_Reports"
      Directory.ensure reports

      let report =
        "./_Reports/_UnitTestWithAltCoverRunner"

      Directory.ensure report

      let coverage =
        !!(@"_Binaries/Test.*/Debug+AnyCPU/net472/Test.*.dll")
        |> Seq.filter (fun p ->
          (p |> Path.GetFileNameWithoutExtension)
          <> "Test.Rules")
        |> Seq.fold
          (fun l test ->
            let tname =
              test |> Path.GetFileNameWithoutExtension

            let testDirectory =
              test |> Path.getFullName |> Path.GetDirectoryName

            let altReport =
              reports
              @@ ("UnitTestWithAltCoverRunner." + tname + ".xml")

            let prep =
              AltCover.PrepareOptions.Primitive(
                { Primitive.PrepareOptions.Create() with
                    StrongNameKey = Path.getFullName "./Build/Infrastructure.snk"
                    Report = altReport
                    OutputDirectories = [| "./__UnitTestWithAltCoverRunner" |]
                    SingleVisit = true
                    InPlace = false
                    Save = false }
                |> AltCoverFilter
              )
              |> AltCoverCommand.Prepare

            { AltCoverCommand.Options.Create prep with
                ToolPath = altcover
                ToolType = frameworkAltcover
                WorkingDirectory = testDirectory }
            |> AltCoverCommand.run

            printfn "Unit test the instrumented code"

            let nunitparams =
              { NUnit3Defaults with
                  ToolPath = nunitConsole
                  WorkingDir = "."
                  ResultSpecs =
                    [ "./_Reports/UnitTestWithAltCoverRunnerReport."
                      + tname
                      + ".xml" ] }

            let nunitcmd =
              NUnit3.buildArgs
                nunitparams
                [ testDirectory
                  @@ "__UnitTestWithAltCoverRunner"
                  @@ (test |> Path.GetFileName) ]

            try
              let collect =
                AltCover.CollectOptions.Primitive
                  { Primitive.CollectOptions.Create() with
                      Executable = nunitConsole
                      RecorderDirectory =
                        testDirectory @@ "__UnitTestWithAltCoverRunner"
                      CommandLine = AltCoverCommand.splitCommandLine nunitcmd }
                |> AltCoverCommand.Collect

              { AltCoverCommand.Options.Create collect with
                  ToolPath = altcover
                  ToolType = frameworkAltcover
                  WorkingDirectory = "." }
              |> AltCoverCommand.run
            with x -> // while fixing
              let exitCode () =
                if x.Message.Contains("'") then
                  let m = x.Message.Split('\'').[1]
                  let (ok, n) = m |> Int32.TryParse
                  if ok then n else Int32.MaxValue
                else
                  Int32.MaxValue

              match tname with
              | "Test.Framework" when exitCode () <= 2 -> printfn "%A" x.Message
              | "Test.Rules.Smells" when exitCode () <= 2 -> printfn "%A" x.Message
              | _ -> reraise ()

            altReport :: l)
          []

      ReportGenerator.generateReports
        (fun p ->
          { p with
              ToolType = ToolType.CreateLocalTool()
              ReportTypes =
                [ ReportGenerator.ReportType.Html
                  ReportGenerator.ReportType.XmlSummary ]
              TargetDir = report })
        coverage

      (report @@ "Summary.xml")
      |> uncovered
      |> printfn "%A uncovered lines"

      if
        Environment.isWindows
        && [ "GITHUB_RUN_NUMBER"
             "COVERALLS_REPO_TOKEN" ]
           |> List.forall (
             Environment.environVar
             >> String.IsNullOrWhiteSpace
             >> not
           )
      then
        let pwsh =
          match "pwsh" |> Fake.Core.ProcessUtils.tryFindFileOnPath with
          | Some path -> path
          | _ -> "pwsh"

        CreateProcess.fromRawCommand
          pwsh
          [ "-NoProfile"
            "./Build/merge-coverage.ps1" ]
        |> CreateProcess.withWorkingDirectory "."
        |> Proc.run
        |> (Actions.AssertResult "pwsh")

        let combined =
          reports
          @@ "CombinedTestWithAltCoverRunner.coveralls"

        let log = Information.shortlog "."
        let gap = log.IndexOf ' '
        let commit = log.Substring gap

        Actions.Run
          ("dotnet",
           "_Reports",
           [ "csmacnz.Coveralls"
             "--opencover"
             "-i"
             combined
             "--repoToken"
             Environment.environVar "COVERALLS_REPO_TOKEN"
             "--commitId"
             commitHash
             "--commitBranch"
             Information.getBranchName (".")
             "--commitAuthor"
             String.Empty
             "--commitEmail"
             String.Empty
             "--commitMessage"
             commit
             "--jobId"
             DateTime.UtcNow.ToString("yyMMdd-HHmmss") ])
          "Coveralls upload failed"

    )

  let UnitTestWithAltCoverCoreRunner =
    (fun _ ->
      let reports = Path.getFullName "./_Reports"
      Directory.ensure reports

      let report =
        "./_Reports/_UnitTestWithAltCoverCoreRunner"

      Directory.ensure report

      let coverage =
        !!(@"gendarme/**/Test.*.*sproj")
        |> Seq.filter (fun p ->
          (p |> Path.GetFileNameWithoutExtension)
          <> "Test.Rules")
        |> Seq.fold
          (fun l test ->
            printfn "%A" test

            let tname =
              test |> Path.GetFileNameWithoutExtension

            let testDirectory =
              test |> Path.getFullName |> Path.GetDirectoryName

            let altReport =
              reports
              @@ ("UnitTestWithAltCoverCoreRunner." + tname + ".xml")

            let altReport2 =
              reports
              @@ ("UnitTestWithAltCoverCoreRunner."
                  + tname
                  + ".net8.0.xml")

            let collect =
              AltCover.CollectOptions.Primitive(Primitive.CollectOptions.Create()) // FSApi

            let prepare =
              AltCover.PrepareOptions.Primitive( // FSApi
                { Primitive.PrepareOptions.Create() with
                    Report = altReport
                    StrongNameKey = Path.getFullName "./Build/Infrastructure.snk"
                    SingleVisit = true }
                |> AltCoverFilter
              )

            let forceTrue = DotNet.CLIOptions.Force true
            //printfn "Test arguments : '%s'" (DotNet.ToTestArguments prepare collect forceTrue)

            let t =
              DotNet.TestOptions.Create().WithAltCoverOptions prepare collect forceTrue

            printfn "WithAltCoverOptions returned '%A'" t.Common.CustomParams

            let setBaseOptions (o: DotNet.Options) =
              { o with
                  WorkingDirectory = Path.getFullName testDirectory
                  Verbosity = Some DotNet.Verbosity.Minimal }

            let testWithCLIArguments (o: Fake.DotNet.DotNet.TestOptions) =
              let msb =
                { o.MSBuildParams with
                    ConsoleLogParameters = []
                    DistributedLoggers = None
                    DisableInternalBinLog = true }

              { o with MSBuildParams = msb }

            try
              DotNet.test
                (fun to' ->
                  { (to'.WithCommon(setBaseOptions).WithAltCoverOptions
                      prepare
                      collect
                      forceTrue) with
                      Framework = Some "net8.0" }
                  |> testWithCLIArguments)
                test
            with x -> // while fixing
              match tname with
              | "Test.Framework"
              | "Test.Rules.Smells" -> printfn "%A" x
              | _ -> reraise ()

            altReport2 :: l)
          []

      ReportGenerator.generateReports
        (fun p ->
          { p with
              ToolType = ToolType.CreateLocalTool()
              ReportTypes =
                [ ReportGenerator.ReportType.Html
                  ReportGenerator.ReportType.XmlSummary ]
              TargetDir = report })
        coverage

      (report @@ "Summary.xml")
      |> uncovered
      |> printfn "%A uncovered lines")

  let Packaging =
    (fun _ ->
      let netcoresource =
        Path.getFullName "./gendarme/console/gendarme.csproj"

      let publish = Path.getFullName "./_Publish"

      DotNet.publish
        (fun options ->
          { options with
              OutputPath = Some publish
              Configuration = DotNet.BuildConfiguration.Release
              MSBuildParams =
                { options.MSBuildParams with
                    ConsoleLogParameters = []
                    DistributedLoggers = None
                    DisableInternalBinLog = true
                    Properties = options.MSBuildParams.Properties }
              Framework = Some "netcoreapp2.1" })
        netcoresource

      let housekeeping =
        [ (Path.getFullName "./Nu*.md", Some "", None)
          (Path.getFullName "./LICENS*", Some "", None)
          (Path.getFullName "./Image.*g", Some "", None) ]

      let rulesDirs =
        Directory.GetDirectories(
          "./_Binaries",
          "Gendarme.Rules.*",
          SearchOption.AllDirectories
        )
        |> Seq.map Path.getFullName
        |> Seq.toList

      //rulesDirs |> List.iter (printfn "%A")

      let rules =
        rulesDirs
        |> List.collect (fun f ->
          !!(f
             @@ "Release+AnyCPU/netstandard2.0/Gendarme.Rules.*.dll")
          |> Seq.toList)
        |> List.distinctBy Path.GetFileName

      //rules |> List.iter (printfn "%A")
      let altrules = // plus mocker
        !!("./_Binaries/AltCode.*/Release+AnyCPU/netstandard2.0/AltCode.*.dll")
        |> Seq.map Path.getFullName
        |> Seq.toList

      let syslibs =
        !!("./_Publish.Globalization/System.*.dll")
        |> Seq.map Path.getFullName
        |> Seq.toList

      let obsolete =
        !!("./_Binaries/Obsolete.*/Release+AnyCPU/net472/Obsolete.*.dll")
        |> Seq.map Path.getFullName
        |> Seq.toList

      do
        let rulesXml =
          "./_Binaries/gendarme/Release+AnyCPU/net472/rules.xml"
          |> Path.getFullName

        let rulesDoc = rulesXml |> XDocument.Load

        let g =
          rulesDoc.Descendants(XName.Get("gendarme"))
          |> Seq.head

        g.Add(
          XElement(XName.Get "ruleset", XAttribute(XName.Get "name", "obsolete-cas"))
        )

        let sets =
          g.Descendants(XName.Get("ruleset"))

        sets
        |> Seq.iter (fun s ->
          let name =
            s.Attribute(XName.Get("name")).Value

          match name with
          | "obsolete-cas"
          | "self-test"
          | "default" ->
            let rule =
              XElement(
                XName.Get "rules",
                XAttribute(XName.Get "include", "*"),
                XAttribute(XName.Get "from", "Obsolete.Rules.Security.Cas.dll")
              )

            s.Add rule
          | _ -> ())

        rulesDoc.Save rulesXml

      let net472 =
        List.concat
          [ !! "./_Binaries/gendarme/Release+AnyCPU/net472/*.*"
            |> Seq.map Path.getFullName
            |> Seq.toList
            syslibs
            altrules
            obsolete
            rules ]
        |> List.map (fun f -> (f, Some "tools", None))

      let leadstring = publish.Length

      let netcoremain =
        !!(Path.getFullName "./_Publish/**/*.*")
        |> Seq.map (fun f ->
          let relpath =
            (leadstring |> f.Substring).Replace("\\", "/")

          (f, Some("tools/netcoreapp2.1/any" + relpath), None))
        |> Seq.toList

      let netcore =
        List.concat
          [ netcoremain
            [ syslibs; rules; altrules ]
            |> List.concat
            |> List.map (fun f -> (f, Some "tools/netcoreapp2.1/any", None)) ]

      let files =
        List.concat [ net472; housekeeping ]

      let globalfiles =
        List.concat [ netcore; housekeeping ]

      let workingDir = "./_Binaries/_Packaging"
      Directory.ensure workingDir
      let output = "./_Packaging"
      Directory.ensure output

      let nuspec =
        "./Build/altcode.gendarme.nuspec"

      let x s =
        XName.Get(s, "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd")

      let dotnetNupkg = XDocument.Load nuspec

      let title =
        dotnetNupkg.Descendants(x "title") |> Seq.head

      title.ReplaceNodes "altcode.gendarme (.net core global tool)"

      let tag =
        dotnetNupkg.Descendants(x "tags") |> Seq.head

      let insert = XElement(x "packageTypes")
      insert.Add(XElement(x "packageType", XAttribute(XName.Get "name", "DotnetTool")))
      tag.AddAfterSelf insert

      let globalnuspec =
        Path.getFullName "./_Packaging/altcode.gendarme-tool.nuspec"

      dotnetNupkg.Save globalnuspec

      [ ("altcode.gendarme", files, nuspec)
        ("altcode.gendarme-tool", globalfiles, globalnuspec) ]
      |> List.iter (fun (project, payload, recipe) ->
        NuGet
          (fun p ->
            { p with
                Authors = [ "Steve Gilham" ]
                Project = project
                Description =
                  "A somewhat updated build of the Mono.Gendarme static analysis tool for use with modern (including .net standard/core) assemblies."
                OutputPath = output
                WorkingDir = workingDir
                Files = payload
                Dependencies = []
                Version = (Version + badge)
                Copyright = Copyright.Replace("©", "(c)")
                Publish = false
                ReleaseNotes =
                  let source =
                    Path.getFullName "./_Generated/ReleaseNotes.md"
                    |> File.ReadAllLines
                    |> Seq.map (fun s ->
                      let t =
                        System.Text.RegularExpressions.Regex.Replace(
                          s,
                          "^\*\s",
                          "* •\u00A0"
                        )

                      let u =
                        System.Text.RegularExpressions.Regex.Replace(
                          t,
                          "^\s\s\*\s", // ◦ U+25E6 WHITE BULLET
                          "  * \u00A0\u00A0\u25E6\u00A0"
                        )

                      let v =
                        System.Text.RegularExpressions.Regex.Replace(
                          u,
                          "^\s\s\s+\*\s", // ⁃ U+2043 HYPHEN BULLET,
                          "    * \u00A0\u00A0\u00A0\u00A0\u2043\u00A0"
                        )

                      System.Text.RegularExpressions.Regex.Replace(
                        v,
                        "^#\s", // ⁋ U+204B REVERSED PILCROW SIGN
                        "# \u204B"
                      ))
                    |> (fun s -> String.Join(Environment.NewLine, s))

                  use w = new StringWriter()
                  // printfn "tweaked = %A" source
                  Markdig.Markdown.ToPlainText(source, w) |> ignore

                  let releaseNotes =
                    "This build from https://github.com/SteveGilham/Gendarme/tree/"
                    + commitHash
                    + Environment.NewLine
                    + Environment.NewLine
                    + w
                      .ToString()
                      .Replace("\u204B", Environment.NewLine)

                  printfn "release notes are %A characters" releaseNotes.Length
                  Assert.That(releaseNotes.Length, Is.LessThan 35000)
                  releaseNotes
                ToolPath =
                  ("./packages/"
                   + (packageVersion "NuGet.CommandLine")
                   + "/tools/NuGet.exe")
                  |> Path.getFullName })
          recipe))

  //_Target "OperationalTest" ignore

  let Unpack =
    (fun _ ->
      Directory.ensure "./_Reports"
      let unpack = Path.getFullName "./_Unpack"
      let config = unpack @@ ".config"
      Directory.ensure unpack
      Shell.cleanDir (unpack)
      Directory.ensure config

      let text =
        File.ReadAllText "./Build/dotnet-tools.json"

      let newtext =
        text.Replace("{0}", (Version + badge))

      File.WriteAllText((config @@ "dotnet-tools.json"), newtext)

      let packroot =
        Path.GetFullPath "./_Packaging"

      let config =
        XDocument.Load "./Build/NuGet.config.dotnettest"

      let repo =
        config.Descendants(XName.Get("add")) |> Seq.head

      repo.SetAttributeValue(XName.Get "value", packroot)
      config.Save(unpack @@ "NuGet.config")

      let csproj =
        XDocument.Load "./Build/unpack.xml"

      let p =
        csproj.Descendants(XName.Get("PackageReference"))
        |> Seq.head

      p.Attribute(XName.Get "VersionOverride").Value <- (Version + badge)
      let proj = unpack @@ "unpack.csproj"
      csproj.Save proj

      DotNet.restore
        (fun o ->
          { o.WithCommon(withWorkingDirectoryVM unpack) with
              MSBuildParams =
                { o.MSBuildParams with
                    ConsoleLogParameters = []
                    DistributedLoggers = None
                    DisableInternalBinLog = true
                    Properties = o.MSBuildParams.Properties }
              Packages = [ "./packages" ] })
        proj

      let vname = Version + badge

      let from =
        (Path.getFullName @"_Unpack/packages/altcode.gendarme/")
        @@ vname

      printfn "Copying from %A to %A" from unpack
      Shell.copyDir unpack from (fun _ -> true)

      Assert.Throws<Exception>(fun () ->
        Gendarme.run
          { Gendarme.Params.Create() with
              WorkingDirectory = unpack
              Severity = Gendarme.Severity.All
              Confidence = Gendarme.Confidence.All
              Configuration =
                (Path.GetFullPath "./gendarme/FSharpExamples/fsharp-rules.xml")
              Console = true
              Log = Path.GetFullPath "./_Reports/gendarme.html"
              LogKind = Gendarme.LogKind.Html
              Targets =
                [ Path.GetFullPath
                    "./_Binaries/FSharpExamples/Release+AnyCPU/net472/FSharpExamples.dll" ]
              ToolPath = Path.GetFullPath "_Unpack/tools/gendarme.exe"
              FailBuildOnDefect = true })
      |> ignore)

  let DotnetGlobalIntegration =
    (fun _ ->
      let working =
        Path.getFullName "./_Unpack-tool"

      let mutable set = false

      try
        Directory.ensure working
        Shell.cleanDir working
        Directory.ensure "./_Reports"

        let nugget =
          !! "./_Packaging/altcode.gendarme-tool.*.nupkg"
          |> Seq.last

        let unpack =
          Path.getFullName "_Unpack-tool/tool-raw"

        System.IO.Compression.ZipFile.ExtractToDirectory(nugget, unpack)

        let from =
          Path.getFullName @"_Unpack-tool/tool-raw/tools/netcoreapp2.1/any/"

        let shallow =
          Path.getFullName "_Unpack-tool/tool"

        Shell.copyDir shallow from (fun _ -> true)

        let packroot =
          Path.GetFullPath "./_Packaging"

        let config =
          XDocument.Load "./Build/NuGet.config.dotnettest"

        let repo =
          config.Descendants(XName.Get("add")) |> Seq.head

        repo.SetAttributeValue(XName.Get "value", packroot)
        config.Save(working @@ "NuGet.config")

        Actions.RunDotnet
          (fun o' ->
            { dotnetOptions o' with
                WorkingDirectory = working })
          "tool"
          ("install -g altcode.gendarme-tool --add-source "
           + (Path.getFullName "./_Packaging")
           + " --version "
           + Version
           + badge)
          "Installed"

        Actions.RunDotnet
          (fun o' ->
            { dotnetOptions o' with
                WorkingDirectory = working })
          "tool"
          ("list -g ")
          "Checked"

        set <- true

        Assert.Throws<Exception>(fun () ->
          Gendarme.run
            { Gendarme.Params.Create() with
                WorkingDirectory = working
                Severity = Gendarme.Severity.All
                Confidence = Gendarme.Confidence.All
                Configuration =
                  (Path.GetFullPath "./gendarme/FSharpExamples/fsharp-rules.xml")
                Console = true
                Log = Path.GetFullPath "./_Reports/gendarme-tool.html"
                LogKind = Gendarme.LogKind.Html
                Targets =
                  [ Path.GetFullPath
                      "./_Binaries/FSharpExamples/Release+AnyCPU/netstandard2.0/FSharpExamples.dll" ]
                ToolPath = "gendarme"
                ToolType = ToolType.CreateGlobalTool()
                FailBuildOnDefect = true })
        |> ignore // (printfn "%A")
        // System.Exception: Process exit code '1' <> 0. Command Line: gendarme --config "C:\Users\steve\Documents\GitHub\Gendarme\gendarme\FSharpExamples\fsharp-rules.xml" --html "C:\Users\steve\Documents\GitHub\Gendarme\_Reports\gendarme-tool.html" --console --severity all --confidence all "C:\Users\steve\Documents\GitHub\Gendarme\_Binaries\FSharpExamples\Release\netstandard2.0\FSharpExamples.dll"

        // self-test
        let targets =
          !!("./_Binaries/*endarm*/Debug+AnyCPU/*/*endarm*.dll")
          |> Seq.map Path.GetFullPath
          |> Seq.filter (fun f -> (Path.GetFileName f).StartsWith("Test") |> not)
          |> Seq.distinctBy Path.GetFileName
          |> Seq.toList

        Gendarme.run
          { Gendarme.Params.Create() with
              WorkingDirectory = working
              Severity = Gendarme.Severity.All
              Confidence = Gendarme.Confidence.All
              Configuration = (Path.GetFullPath "./Build/csharp-rules.xml")
              Console = true
              Log = Path.GetFullPath "./_Reports/gendarme-tool-selftest.html"
              LogKind = Gendarme.LogKind.Html
              Targets = targets
              ToolPath = "gendarme"
              ToolType = ToolType.CreateGlobalTool()
              FailBuildOnDefect = true }

        let targets =
          !!("./_Binaries/AltCode.*/Debug+AnyCPU/*/AltCode.*.dll")
          |> Seq.map Path.GetFullPath
          |> Seq.distinctBy Path.GetFileName
          |> Seq.toList

        Gendarme.run
          { Gendarme.Params.Create() with
              WorkingDirectory = working
              Severity = Gendarme.Severity.All
              Confidence = Gendarme.Confidence.All
              Configuration = (Path.GetFullPath "./Build/fsharp-rules.xml")
              Console = true
              Log = Path.GetFullPath "./_Reports/gendarme-tool-acselftest.html"
              LogKind = Gendarme.LogKind.Html
              Targets = targets
              ToolPath = "gendarme"
              ToolType = ToolType.CreateGlobalTool()
              FailBuildOnDefect = true }

        Gendarme.run
          { Gendarme.Params.Create() with
              WorkingDirectory = working
              Severity = Gendarme.Severity.All
              Confidence = Gendarme.Confidence.All
              Configuration = (Path.GetFullPath "./Build/fsharp-rules.xml")
              Console = true
              Log = Path.GetFullPath "./_Reports/gendarme-tool-fsselftest.html"
              LogKind = Gendarme.LogKind.Html
              Targets =
                [ Path.GetFullPath
                    "./_Binaries/CecilExtensions/Debug+AnyCPU/netstandard2.0/CecilExtensions.dll" ]
              ToolPath = "gendarme"
              ToolType = ToolType.CreateGlobalTool()
              FailBuildOnDefect = true }

        Gendarme.run
          { Gendarme.Params.Create() with
              WorkingDirectory = working
              Severity = Gendarme.Severity.All
              Confidence = Gendarme.Confidence.All
              Configuration = (Path.GetFullPath "./Build/build-rules.xml")
              Console = true
              Log = Path.GetFullPath "./_Reports/gendarme-tool-fsselftest.html"
              LogKind = Gendarme.LogKind.Html
              Targets =
                [ Path.GetFullPath "./$Binaries/Build/Debug+AnyCPU/net8.0/Build.dll"
                  Path.GetFullPath "./$Binaries/Setup/Debug+AnyCPU/net8.0/Setup.dll" ]
              ToolPath = "gendarme"
              ToolType = ToolType.CreateGlobalTool()
              FailBuildOnDefect = true }

      finally
        if set then
          Actions.RunDotnet
            (fun o' ->
              { dotnetOptions o' with
                  WorkingDirectory = working })
            "tool"
            ("uninstall -g altcode.gendarme-tool")
            "uninstalled"

        let folder =
          (nugetCache @@ "altcode.gendarme-tool")
          @@ (Version + badge)

        Shell.mkdir folder
        Shell.deleteDir folder)

  let Lint =
    (fun _ ->
      let cfg =
        Path.getFullName "./fsharplint.json"

      let doLint f =
        CreateProcess.fromRawCommand "dotnet" [ "fsharplint"; "lint"; "-l"; cfg; f ]
        |> CreateProcess.setEnvironmentVariable
          "DOTNET_ROLL_FORWARD_ON_NO_CANDIDATE_FX"
          "2"
        |> CreateProcess.ensureExitCodeWithMessage "Lint issues were found"
        |> Proc.run

      let doLintAsync f = async { return (doLint f).ExitCode }

      let throttle x =
        Async.Parallel(x, System.Environment.ProcessorCount)

      let demo = Path.getFullName "./Demo"

      let regress =
        Path.getFullName "./RegressionTesting"

      let sample = Path.getFullName "./Samples"

      let failOnIssuesFound (issuesFound: bool) =
        Assert.That(issuesFound, Is.False, "Lint issues were found")

      [ !! "./**/*.fsproj"
        |> Seq.sortBy (Path.GetFileName)
        |> Seq.filter (fun f ->
          ((f.Contains demo)
           || (f.Contains regress)
           || (f.Contains sample))
          |> not)
        !! "./Build/*.fsx" |> Seq.map Path.GetFullPath ]
      |> Seq.concat
      |> Seq.map doLintAsync
      |> throttle
      |> Async.RunSynchronously
      |> Seq.exists (fun x -> x <> 0)
      |> failOnIssuesFound)

  let CheckAltCover =
    (fun _ -> // Needs debug because release is compiled --standalone which contaminates everything
      Directory.ensure "./_Reports"

      let packroot =
        Path.GetFullPath "./_Packaging"

      let working =
        Path.getFullName "./_Unpack-tool"

      let altcover =
        Path.getFullName "../altcover"

      let mutable set = false

      Directory.ensure working

      let nugget =
        !!(packroot @@ "altcode.gendarme-tool.*.nupkg")
        |> Seq.last

      let nuggetVer =
        (nugget |> Path.GetFileNameWithoutExtension)
          .Substring("altcode.gendarme-tool.".Length)

      try
        let config =
          XDocument.Load "./Build/NuGet.config.dotnettest"

        let repo =
          config.Descendants(XName.Get("add")) |> Seq.head

        repo.SetAttributeValue(XName.Get "value", packroot)
        config.Save(working @@ "NuGet.config")

        Actions.RunDotnet
          (fun o' ->
            { dotnetOptions o' with
                WorkingDirectory = working })
          "tool"
          ("install -g altcode.gendarme-tool --add-source "
           + (Path.getFullName "./_Packaging")
           + " --version "
           + nuggetVer)
          "Installed"

        Actions.RunDotnet
          (fun o' ->
            { dotnetOptions o' with
                WorkingDirectory = working })
          "tool"
          ("list -g ")
          "Checked"

        set <- true

        [ ("./Build/common-rules.xml",
           [ "_Binaries/AltCover/Debug+AnyCPU/netcoreapp2.1/AltCover.dll" // global tool builds
             "_Binaries/AltCover.Avalonia/Debug+AnyCPU/netcoreapp2.1/AltCover.Visualizer.dll" ])
          ("./Build/common-rules.xml",
           [ "_Binaries/AltCover.Engine/Debug+AnyCPU/netstandard2.0/AltCover.Engine.dll"
             "_Binaries/AltCover/Debug+AnyCPU/netcoreapp2.0/AltCover.dll"
             "_Binaries/AltCover.Recorder/Debug+AnyCPU/net20/AltCover.Recorder.dll"
             "_Binaries/AltCover.Async/Debug+AnyCPU/net46/AltCover.Async.dll"
             "_Binaries/AltCover.PowerShell/Debug+AnyCPU/netstandard2.0/AltCover.PowerShell.dll"
             "_Binaries/AltCover.Fake/Debug+AnyCPU/netstandard2.0/AltCover.Fake.dll"
             "_Binaries/AltCover.DotNet/Debug+AnyCPU/netstandard2.0/AltCover.DotNet.dll"
             "_Binaries/AltCover.Toolkit/Debug+AnyCPU/netstandard2.0/AltCover.Toolkit.dll"
             "_Binaries/AltCover.UICommon/Debug+AnyCPU/netstandard2.0/AltCover.UICommon.dll"
             "_Binaries/AltCover.Visualizer3/Debug+AnyCPU/netcoreapp2.1/AltCover.Visualizer.dll" // GTK3 (obsolete)
             "_Binaries/AltCover.Fake.DotNet.Testing.AltCover/Debug+AnyCPU/netstandard2.0/AltCover.Fake.DotNet.Testing.AltCover.dll" ])
          ("./Build/common-rules.xml", // Framework builds
           [ "_Binaries/AltCover/Debug+AnyCPU/net472/AltCover.exe" // framework builds
             "_Binaries/AltCover.Visualizer/Debug+AnyCPU/net472/AltCover.Visualizer.exe" ])
          ("./Build/csharp-rules.xml",
           [ "_Binaries/AltCover.DataCollector/Debug+AnyCPU/netstandard2.0/AltCover.DataCollector.dll"
             "_Binaries/AltCover.Monitor/Debug+AnyCPU/netstandard2.0/AltCover.Local.Monitor.dll"
             "_Binaries/AltCover.FontSupport/Debug+AnyCPU/netstandard2.0/AltCover.FontSupport.dll"
             "_Binaries/AltCover.Cake/Debug+AnyCPU/netstandard2.0/AltCover.Cake.dll" ])
          ("./Build/csharp-rules.xml", // Framework builds
           [ "_Binaries/AltCover.Monitor/Debug+AnyCPU/net20/AltCover.Local.Monitor.dll"
             "_Binaries/AltCover.FontSupport/Debug+AnyCPU/net472/AltCover.FontSupport.dll" ]) ]
        |> Seq.iter (fun (ruleset, files) ->
          Gendarme.run
            { Gendarme.Params.Create() with
                WorkingDirectory = working
                Severity = Gendarme.Severity.All
                Confidence = Gendarme.Confidence.All
                Configuration = altcover @@ ruleset
                Console = true
                Log = Path.GetFullPath "./_Reports/altcoverCheck.html"
                LogKind = Gendarme.LogKind.Html
                Targets = files |> Seq.map (fun f -> altcover @@ f)
                ToolType = ToolType.CreateGlobalTool()
                FailBuildOnDefect = true })
      finally
        if set then
          Actions.RunDotnet
            (fun o' ->
              { dotnetOptions o' with
                  WorkingDirectory = working })
            "tool"
            ("uninstall -g altcode.gendarme-tool")
            "uninstalled"

        let folder =
          nugetCache @@ "altcode.gendarme-tool" @@ nuggetVer

        Shell.mkdir folder
        Shell.deleteDir folder)

  let All =
    (fun _ ->
      if
        Environment.isWindows
        && currentBranch.StartsWith "release/"
        && "NUGET_API_TOKEN"
           |> Environment.environVar
           |> String.IsNullOrWhiteSpace
           |> not
      then
        (!! "./_Packagin*/*.nupkg")
        |> Seq.iter (fun f ->
          printfn "Publishing %A from %A" f currentBranch

          Actions.Run
            ("dotnet",
             ".",
             [ "nuget"
               "push"
               f
               "--api-key"
               Environment.environVar "NUGET_API_TOKEN"
               "--source"
               "https://api.nuget.org/v3/index.json" ])
            ("NuGet upload failed " + f)))

  let resetColours _ =
    Console.ForegroundColor <- consoleBefore |> fst
    Console.BackgroundColor <- consoleBefore |> snd
    (!! "internalTrace*.log") |> Seq.iter Shell.rm
    (!! "nunit-agent_*.log") |> Seq.iter Shell.rm

  //_Target "None" ignore

  Target.description "ResetConsoleColours"
  Target.createFinal "ResetConsoleColours" resetColours
  Target.activateFinal "ResetConsoleColours"

  let initTargets () =
    _Target "None" ignore
    _Target "Preparation" ignore
    _Target "Clean" Clean
    _Target "SetVersion" SetVersion
    _Target "Compilation" ignore
    _Target "BuildRelease" BuildRelease
    _Target "BuildDebug" BuildDebug
    _Target "UnitTest" ignore
    _Target "FxCop" FxCop
    _Target "JustUnitTest" JustUnitTest
    _Target "UnitTestDotNet" UnitTestDotNet
    _Target "Coverage" ignore
    _Target "UnitTestWithAltCoverRunner" UnitTestWithAltCoverRunner
    _Target "UnitTestWithAltCoverCoreRunner" UnitTestWithAltCoverCoreRunner
    _Target "Packaging" Packaging
    _Target "OperationalTest" ignore
    _Target "Unpack" Unpack
    _Target "DotnetGlobalIntegration" DotnetGlobalIntegration
    _Target "Lint" Lint
    _Target "CheckAltCover" CheckAltCover
    _Target "All" All

    // Dependencies
    "Clean" ==> "SetVersion" ==> "Preparation"
    |> ignore

    "Preparation" ==> "BuildDebug" ==> "Compilation"
    |> ignore

    "BuildDebug" ==> "Lint" ==> "All" |> ignore

    "BuildDebug" ==> "FxCop"
    =?> ("All", Environment.isWindows && fxcop |> Option.isSome) // where supported
    |> ignore

    "Preparation" ==> "BuildRelease" ==> "Compilation"
    |> ignore

    "BuildDebug" ==> "JustUnitTest" ==> "UnitTest"
    |> ignore

    "BuildDebug" ==> "UnitTestDotNet" ==> "UnitTest"
    |> ignore

    "BuildDebug"
    =?> ("UnitTestWithAltCoverRunner", Environment.isWindows)
    ==> "Coverage"
    |> ignore

    "BuildDebug"
    ==> "UnitTestWithAltCoverCoreRunner"
    ==> "Coverage"
    |> ignore

    "BuildRelease" ==> "Packaging" |> ignore

    // "UnitTest" ==> "All" // redundant

    "Packaging" ==> "Unpack" ==> "OperationalTest"
    |> ignore

    "Packaging"
    ==> "DotnetGlobalIntegration"
    ==> "OperationalTest"
    |> ignore

    "BuildDebug" ==> "DotnetGlobalIntegration"
    |> ignore

    "Packaging" ==> "CheckAltCover" |> ignore

    "OperationalTest" ==> "All" |> ignore

    "Coverage" ==> "All" |> ignore

  let defaultTarget () =
    resetColours ()
    "All"