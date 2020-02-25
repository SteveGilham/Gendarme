open System
open System.Diagnostics.Tracing
open System.IO
open System.Reflection
open System.Xml
open System.Xml.Linq

open Actions

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.DotNet.NuGet.NuGet
open Fake.DotNet.Testing.NUnit3
open Fake.Testing
open Fake.DotNet.Testing
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing
open Fake.IO.Globbing.Operators

open AltCover_Fake.DotNet.DotNet
open AltCover_Fake.DotNet.Testing

open NUnit.Framework

let Copyright = ref String.Empty
let Version = ref String.Empty
let consoleBefore = (Console.ForegroundColor, Console.BackgroundColor)

let programFiles = Environment.environVar "ProgramFiles"
let dotnetPath = "dotnet" |> Fake.Core.ProcessUtils.tryFindFileOnPath

let dotnetOptions (o: DotNet.Options) =
  match dotnetPath with
  | Some f -> { o with DotNetCliPath = f }
  | None -> o

let toolPackages =
  let xml =
    "./Build/dotnet-cli.csproj"
    |> Path.getFullName
    |> XDocument.Load
  xml.Descendants(XName.Get("PackageReference"))
  |> Seq.map
       (fun x ->
       (x.Attribute(XName.Get("Include")).Value, x.Attribute(XName.Get("Version")).Value))
  |> Map.ofSeq

let packageVersion (p: string) = p.ToLowerInvariant() + "/" + (toolPackages.Item p)

let nunitConsole =
  ("./packages/" + (packageVersion "NUnit.ConsoleRunner") + "/tools/nunit3-console.exe")
  |> Path.getFullName

let altcover =
  ("./packages/" + (packageVersion "altcover") + "/tools/net45/AltCover.exe")
  |> Path.getFullName

let framework_altcover = Fake.DotNet.ToolType.CreateFullFramework()

let AltCoverFilter(p: Primitive.PrepareParams) =
  { p with
      //MethodFilter = "WaitForExitCustom" :: (p.MethodFilter |> Seq.toList)
      AssemblyExcludeFilter = @"NUnit3\." :: (@"Tests\." :: (p.AssemblyExcludeFilter |> Seq.toList))
      AssemblyFilter = "FSharp" :: @"Test\.Rules" :: (p.AssemblyFilter |> Seq.toList)
      LocalSource = true
      TypeFilter = [ @"System\."; "Microsoft" ] @ (p.TypeFilter |> Seq.toList) 
        }

let cliArguments =
  { MSBuild.CliArguments.Create() with
      ConsoleLogParameters = []
      DistributedLoggers = None
      DisableInternalBinLog = true }

let withWorkingDirectoryVM dir o =
  { dotnetOptions o with
      WorkingDirectory = Path.getFullName dir
      Verbosity = Some DotNet.Verbosity.Minimal }

let withWorkingDirectoryOnly dir o =
  { dotnetOptions o with WorkingDirectory = Path.getFullName dir }

let withCLIArgs (o: Fake.DotNet.DotNet.TestOptions) =
  { o with MSBuildParams = cliArguments }
let withMSBuildParams (o: Fake.DotNet.DotNet.BuildOptions) =
  { o with MSBuildParams = cliArguments }

let defaultTestOptions fwk common (o: DotNet.TestOptions) =
  { o.WithCommon
      ((fun o2 -> { o2 with Verbosity = Some DotNet.Verbosity.Normal }) >> common) with
      NoBuild = true
      Framework = fwk // Some "netcoreapp3.0"
      Configuration = DotNet.BuildConfiguration.Debug }

let msbuildRelease proj =
  MSBuild.build (fun p ->
               { p with
                   Verbosity = Some MSBuildVerbosity.Normal
                   ConsoleLogParameters = []
                   DistributedLoggers = None
                   DisableInternalBinLog = true
                   Properties =
                     [ "Configuration", "Release"
                       "DebugSymbols", "True" ] }) proj

let msbuildDebug proj =
  MSBuild.build (fun p ->
           { p with
               Verbosity = Some MSBuildVerbosity.Normal
               ConsoleLogParameters = []
               DistributedLoggers = None
               DisableInternalBinLog = true
               Properties =
                 [ "Configuration", "Debug"
                   "DebugSymbols", "True" ] }) proj

let misses = ref 0

let uncovered (path:string) =
    misses := 0
    !! path
    |> Seq.collect (fun f ->
         let xml = XDocument.Load f
         xml.Descendants(XName.Get("Uncoveredlines"))
         |> Seq.filter (fun x ->
              match String.IsNullOrWhiteSpace x.Value with
              | false -> true
              | _ ->
                sprintf "No coverage from '%s'" f |> Trace.traceImportant
                misses := 1 + !misses
                false)
         |> Seq.map (fun e ->
              let coverage = e.Value
              match Int32.TryParse coverage with
              | (false, _) ->
                printfn "%A" xml
                Assert.Fail("Could not parse uncovered line value '" + coverage + "'")
                0
              | (_, numeric) ->
                printfn "%s : %A"
                  (f
                   |> Path.GetDirectoryName
                   |> Path.GetFileName) numeric
                numeric))
    |> Seq.toList


let _Target s f =
  Target.description s
  Target.create s f

// Preparation

_Target "Preparation" ignore

_Target "Clean" (fun _ ->
  printfn "Cleaning the build and deploy folders"
  Actions.Clean())

_Target "SetVersion" (fun _ ->

  let now = DateTime.Now
  let time = now.ToString("HHmmss").Substring(0,5).TrimStart('0')
  let y0 = now.Year
  let m0 = now.Month
  let d0 = now.Day
  let y = y0.ToString()
  let m = m0.ToString()
  let d = d0.ToString()
  Version := y + "." + m + "." + d + "."  + time

  let copy = sprintf "© 2010-%d by Steve Gilham <SteveGilham@users.noreply.github.com>" y0
  let copy2 = sprintf "Copyright (C) 2005-%d Novell, Inc. and contributors" y0
  Copyright := "Copyright " + copy

  Directory.ensure "./_Generated"

  let v' = !Version
  
  AssemblyInfoFile.create "./_Generated/AssemblyStaticInfo.fs"
         [ AssemblyInfo.Product "altcode.gendarme"
           AssemblyInfo.Version v'
           AssemblyInfo.FileVersion v'
           AssemblyInfo.Company "Steve Gilham"
           AssemblyInfo.Trademark ""
           AssemblyInfo.CLSCompliant true
           AssemblyInfo.ComVisible false
           AssemblyInfo.Copyright copy ] (Some AssemblyInfoFileConfig.Default)

  AssemblyInfoFile.create "./_Generated/AssemblyStaticInfo.cs"
         [ AssemblyInfo.Title "Gendarme"
           AssemblyInfo.Version v'
           AssemblyInfo.FileVersion v'
           AssemblyInfo.Company "Novell, Inc."
           AssemblyInfo.Trademark ""
           AssemblyInfo.Description "Rule-based assembly analyzer"
           AssemblyInfo.CLSCompliant false
           AssemblyInfo.ComVisible false
           AssemblyInfo.Copyright copy2 ] (Some AssemblyInfoFileConfig.Default)
)

// Basic compilation

_Target "Compilation" ignore

_Target "Restore" (fun _  ->
    [ (!!"./gendarme/**/*.*proj"); (!!"./Build/**/*.*proj") ]
    |> Seq.concat 
    |> Seq.iter (fun f -> 
                 let dir = Path.GetDirectoryName f
                 let proj = Path.GetFileName f
                 DotNet.restore (fun o -> o.WithCommon(withWorkingDirectoryVM dir)) proj))
    
_Target "BuildRelease" (fun _ ->
  "./gendarme/gendarme-win.sln"
  |> msbuildRelease)

_Target "BuildDebug" (fun _ ->
  "./gendarme/gendarme-win.sln"
  |> msbuildDebug)

_Target "UnitTest" ignore

_Target "JustUnitTest" (fun _ ->
  Directory.ensure "./_Reports"
  try
    !!(@"_Binaries/Tests.*/Debug+AnyCPU/net4*/Tests.*.dll")
    |> NUnit3.run (fun p ->
         { p with
             ToolPath = nunitConsole
             WorkingDir = "."
             ResultSpecs = [ "./_Reports/JustUnitTestReport.xml" ] })
  with x ->
    printfn "%A" x
    )//reraise()) // while fixing

_Target "UnitTestDotNet" (fun _ ->
  Directory.ensure "./_Reports"
  try
    !!(@"./**/Tests.*.csproj")
    |> Seq.iter
         (DotNet.test (fun p ->
           { p.WithCommon dotnetOptions with
               Configuration = DotNet.BuildConfiguration.Debug
               Framework = Some "netcoreapp2.1"
               NoBuild = true }
           |> withCLIArgs))
  with x ->
    printfn "%A" x
    )//reraise()) // while fixing

_Target "Coverage" ignore

_Target "UnitTestWithAltCoverRunner" (fun _ ->
  let reports = Path.getFullName "./_Reports"
  Directory.ensure reports
  let report = "./_Reports/_UnitTestWithAltCoverRunner"
  Directory.ensure report

  let coverage = 
      !!(@"_Binaries/Tests.*/Debug+AnyCPU/net4*/Tests.*.dll")
      |> Seq.fold (fun l test -> 
        let tname = test |> Path.GetFileNameWithoutExtension
        let testDirectory = test |> Path.getFullName |> Path.GetDirectoryName
        let altReport = reports @@ ("UnitTestWithAltCoverRunner." + tname + ".xml")

        let prep =
          AltCover.PrepareParams.Primitive
            ({ Primitive.PrepareParams.Create() with
                 XmlReport = altReport
                 OutputDirectories = [| "./__UnitTestWithAltCoverRunner" |]
                 Single = true
                 InPlace = false
                 Save = false }
             |> AltCoverFilter)
          |> AltCover.Prepare
        { AltCover.Params.Create prep with
            ToolPath = altcover
            WorkingDirectory = testDirectory }.WithToolType framework_altcover
        |> AltCover.run

        printfn "Unit test the instrumented code"
        let nunitparams =
          { NUnit3Defaults with
              ToolPath = nunitConsole
              WorkingDir = "."
              ResultSpecs = [ "./_Reports/UnitTestWithAltCoverRunnerReport." + tname + ".xml" ] }

        let nunitcmd =
          NUnit3.buildArgs nunitparams
            [ testDirectory @@ "./__UnitTestWithAltCoverRunner" @@ (test |> Path.GetFileName) ]

        try
          let collect =
            AltCover.CollectParams.Primitive
              { Primitive.CollectParams.Create() with
                  Executable = nunitConsole
                  RecorderDirectory = testDirectory @@ "__UnitTestWithAltCoverRunner"
                  CommandLine = AltCover.splitCommandLine nunitcmd }
            |> AltCover.Collect
          { AltCover.Params.Create collect with
              ToolPath = altcover
              WorkingDirectory = "." }.WithToolType framework_altcover
          |> AltCover.run
        with x ->
          printfn "%A" x
        altReport :: l
      ) [] //reraise()) // while fixing

  ReportGenerator.generateReports (fun p ->
    { p with
        ToolType = ToolType.CreateLocalTool()
        ReportTypes =
          [ ReportGenerator.ReportType.Html; ReportGenerator.ReportType.XmlSummary ]
        TargetDir = report }) coverage

  (report @@ "Summary.xml")
  |> uncovered
  |> printfn "%A uncovered lines"         

    // TODO coveralls ?
)

_Target "UnitTestWithAltCoverCoreRunner"  (fun _ ->
  ()// TODO
)

_Target "Packaging"  (fun _ ->
  let netcoresource = Path.getFullName "./gendarme/console/gendarme.csproj"
  let publish = Path.getFullName "./_Publish"


  DotNet.publish (fun options ->
    { options with
        OutputPath = Some publish
        Configuration = DotNet.BuildConfiguration.Release
        Framework = Some "netcoreapp2.1" }) netcoresource

  let housekeeping = 
      [ (Path.getFullName "./LICENS*", Some "", None)
        (Path.getFullName "./Image.*g", Some "", None) ]

  let rules = Directory.GetDirectories(".", "Gendarme.Rules.*", SearchOption.AllDirectories)
              |> Seq.toList
  //printfn "rules"
  //List.iter (printfn "%A") rules

  let n40rules = rules
                 |> List.collect (fun f -> !!((Path.getFullName f) @@ "Release+AnyCPU/net40/Gendarme.Rules.*")
                                           |> Seq.toList)
                 |> List.filter (fun f -> let ex = f |> Path.GetExtension
                                          match ex with
                                          | ".dll"
                                          | ".pdb" -> true
                                          | _ -> false)
                 |> List.distinctBy Path.GetFileName

  // printfn "n40rules"
  // List.iter (printfn "%A") n40rules

  let corerules = rules
                  |> List.collect (fun f -> !!((Path.getFullName f) @@ "Release+AnyCPU/netstandard2.0/Gendarme.Rules.*")
                                            |> Seq.toList)
                 |> List.filter (fun f -> let ex = f |> Path.GetExtension
                                          match ex with
                                          | ".dll"
                                          | ".pdb" -> true
                                          | _ -> false)
                 |> List.distinctBy Path.GetFileName
  //printfn "corerules"
  //List.iter (printfn "%A") corerules
 

  let net40 = 
    List.concat
      [ !!"./_Binaries/gendarme/Release+AnyCPU/net4*/*.*" |> Seq.toList
        n40rules ]
    |> List.map (fun f -> (f |> Path.getFullName, Some "tools/net40", None))

  let netcore =
    List.concat
      [ !!"./_Binaries/gendarme/Release+AnyCPU/netcoreapp2.1/*.*" |> Seq.toList
        corerules ]
    |> List.map (fun f -> (f |> Path.getFullName, Some "tools/netcoreapp2.1/any", None))

  let files = List.concat [ net40; housekeeping ]
  let globalfiles = List.concat [ netcore; housekeeping ]

  let workingDir = "./_Binaries/_Packaging"
  Directory.ensure workingDir
  let output = "./_Packaging"
  Directory.ensure output
  let nuspec = "./Build/altcode.gendarme.nuspec"
  
  let x s = XName.Get(s, "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd")
  let dotnetNupkg = XDocument.Load nuspec
  let title = dotnetNupkg.Descendants(x "title") |> Seq.head
  title.ReplaceNodes "altcode.gendarme (.net core global tool)"

  let tag = dotnetNupkg.Descendants(x "tags") |> Seq.head
  let insert = XElement(x "packageTypes")
  insert.Add(XElement(x "packageType", XAttribute(XName.Get "name", "DotnetTool")))
  tag.AddAfterSelf insert
  let globalnuspec = Path.getFullName "./_Packaging/altcode.gendarme.global.nuspec"
  dotnetNupkg.Save globalnuspec

  [
    ("altcode.gendarme", files, nuspec)
    ("altcode.gendarme.global", globalfiles, globalnuspec)
  ]
  |> List.iter (fun (project, payload, recipe) ->
        NuGet (fun p ->
           { p with
               Authors = [ "Steve Gilham" ]
               Project = project
               Description = 
                 "A somewhat updated build of the Mono.Gendarme static analysis tool for use with modern (including .net standard/core) assemblies."
               OutputPath = output
               WorkingDir = workingDir
               Files = payload
               Dependencies = []
               Version = (!Version + "-pre-release")
               Copyright = (!Copyright).Replace("©", "(c)")
               Publish = false
               ReleaseNotes = Path.getFullName "ReleaseNotes.md" |> File.ReadAllText
               ToolPath =
                 if Environment.isWindows then
                   ("./packages/" + (packageVersion "NuGet.CommandLine")
                    + "/tools/NuGet.exe") |> Path.getFullName
                 else "/usr/bin/nuget" }) recipe)
)

_Target "Unpack" (fun _ ->
  let unpack = Path.getFullName "./_Unpack"
  let config = unpack @@ ".config"
  Directory.ensure unpack
  Shell.cleanDir (unpack)
  Directory.ensure config

  let text = File.ReadAllText "./Build/dotnet-tools.json"
  let newtext = String.Format(text, !Version)
  File.WriteAllText ((config @@ "dotnet-tools.json"), newtext)


  let packroot = Path.GetFullPath "./_Packaging"
  let config = XDocument.Load "./Build/NuGet.config.dotnettest"
  let repo = config.Descendants(XName.Get("add")) |> Seq.head
  repo.SetAttributeValue(XName.Get "value", packroot)
  config.Save (unpack @@ "NuGet.config")

  let csproj = XDocument.Load "./Build/unpack.xml"
  let p = csproj.Descendants(XName.Get("PackageReference")) |> Seq.head
  p.Attribute(XName.Get "Version").Value <- (!Version + "-pre-release")
  let proj = unpack @@ "unpack.csproj"
  csproj.Save proj

  DotNet.restore (fun o ->
    { o.WithCommon(withWorkingDirectoryVM unpack) with
        Packages = [ "./packages" ] }) proj
)

_Target "All" ignore

let resetColours _ =
  Console.ForegroundColor <- consoleBefore |> fst
  Console.BackgroundColor <- consoleBefore |> snd

Target.description "ResetConsoleColours"
Target.createFinal "ResetConsoleColours" resetColours
Target.activateFinal "ResetConsoleColours"

// Dependencies

"Clean"
==> "SetVersion"
==> "Preparation"

"Preparation"
==> "Restore"
==> "BuildDebug"
==> "Compilation"

"Preparation"
==> "Restore"
==> "BuildRelease"
==> "Compilation"

"BuildDebug"
==> "JustUnitTest"
==> "UnitTestDotNet"
==> "UnitTest"

"BuildDebug"
==> "UnitTestWithAltCoverRunner"
==> "UnitTestWithAltCoverCoreRunner"
==> "Coverage"

"BuildRelease"
==> "Packaging"

"UnitTest"
==> "All"

"Packaging"
==> "Unpack"
==> "All"

"Coverage"
==> "All"

let defaultTarget() =
  resetColours()
  "All"

Target.runOrDefault <| defaultTarget ()