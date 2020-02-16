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
  let ensureExitCode = id


module FSApi =
    let CSharpContainingMethod x =
      x
      |> Seq.filter (fun x -> x <> null)

    let createProcess (parameters:Params)  =
      let doTool() =
        parameters.ToolPath

      let withWorkingDirectory c =
        c
        |> if String.IsNullOrWhiteSpace parameters.WorkingDirectory
            then id
            else (fun s -> sprintf "(%s)%s" parameters.WorkingDirectory c)

      doTool()
      |> withWorkingDirectory
      |> CreateProcess.ensureExitCode

    let transform<'a> (x:'a) = x.ToString()