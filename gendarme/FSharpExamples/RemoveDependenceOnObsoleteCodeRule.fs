namespace RemoveDependenceOnObsoleteCode

open System

[<NoComparison; Obsolete("Use Fake.DotNet.ToolType instead")>]
type ToolType =
  | DotNet of string option // can't attribute this type and constructor for Gendarme
  | Mono of string option // can't attribute this type and constructor for Gendarme
  | Global
  | Framework

[<NoComparison; NoEquality>]
type Params =
  { /// Path to the Altcover executable.
    ToolPath : string
    /// Which version of the tool
    [<System.Diagnostics.CodeAnalysis.SuppressMessage("Gendarme.Rules.Maintainability",
      "RemoveDependenceOnObsoleteCodeRule",Justification="Goes at Genbu")>]
    ToolType : ToolType
    /// Working directory for relative file paths.  Default is the current working directory
    WorkingDirectory : string }