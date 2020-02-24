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

_Target "SetVersion" ignore


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

let defaultTarget() =
  resetColours()
  "All"

Target.runOrDefault <| defaultTarget ()