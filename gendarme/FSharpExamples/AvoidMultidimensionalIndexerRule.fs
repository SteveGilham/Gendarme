namespace AvoidMultidimensionalIndexer

module DotNet =
  // get_Item methods trip the AvoidMultidimensionalIndexerRule
  type CLIArgs =
    | Force of bool
    | FailFast of bool
    | ShowSummary of string
    | Many of CLIArgs seq

    member self.ForceDelete =
      match self with
      | Force b -> b
      | ShowSummary _
      | FailFast _ -> false
      | Many s -> s |> Seq.exists (fun f -> f.ForceDelete)

[<assembly:System.Runtime.InteropServices.ComVisible(false)>]
[<assembly:System.CLSCompliantAttribute(true)>]
()