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

open FSharpLint.Application
open FSharpLint.Framework
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
       (x.Attribute(XName.Get("Include")).Value, x.Attribute(XName.Get("version")).Value))
  |> Map.ofSeq

let packageVersion (p: string) = p.ToLowerInvariant() + "/" + (toolPackages.Item p)

let nunitConsole =
  ("./packages/" + (packageVersion "NUnit.ConsoleRunner") + "/tools/nunit3-console.exe")
  |> Path.getFullName

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
    !!"./gendarme/**/*.*proj"
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

"Compilation"
==> "JustUnitTest"
==> "UnitTestDotNet"
==> "UnitTest"

let defaultTarget() =
  resetColours()
  "All"

Target.runOrDefault <| defaultTarget ()