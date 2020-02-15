namespace RemoveDependenceOnObsoleteCode

open System

[<NoComparison; Obsolete("Use Fake.DotNet.ToolType instead")>]
type ToolType =
  | DotNet of string option // can't attribute this type and constructor for Gendarme
  | Mono of string option // can't attribute this type and constructor for Gendarme
  | Global
  | Framework
