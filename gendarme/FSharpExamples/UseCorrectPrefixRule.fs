namespace UseCorrectPrefix

open System

type Params =
  { /// Path to the Altcover executable.
    ToolPath : string
    /// Working directory for relative file paths.  Default is the current working directory
    WorkingDirectory : string }

  static member Create() =
    { ToolPath = "altcover"
      WorkingDirectory = String.Empty }

module CreateProcess =
  [<System.Diagnostics.CodeAnalysis.SuppressMessage("Gendarme.Rules.Design.Generic",
                                                    "AvoidMethodWithUnusedGenericTypeRule")>]
  let ensureExitCode = id

[<System.Diagnostics.CodeAnalysis.SuppressMessage("Gendarme.Rules.Smells",
                                                  "AvoidSpeculativeGeneralityRule")>]
module FSApi =
  [<System.Diagnostics.CodeAnalysis.SuppressMessage("Gendarme.Rules.Naming",
                                                    "UseCorrectCasingRule")>]
  let CSharpContainingMethod x = // makes classes beginning with "C" happen
    x |> Seq.filter (isNull >> not)

  let createProcess (parameters : Params) =
    let doTool() = parameters.ToolPath

    let withWorkingDirectory c =
      c
      |> if String.IsNullOrWhiteSpace parameters.WorkingDirectory
         then id
         else (fun s -> sprintf "(%s)%s" parameters.WorkingDirectory c)

    doTool()
    |> withWorkingDirectory
    |> CreateProcess.ensureExitCode

  let transform<'a> (x : 'a) = x.ToString()