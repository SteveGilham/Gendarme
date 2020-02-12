namespace AvoidMultidimensionalIndexer

module DotNet =
  // get_Item methods trip the AvoidMultidimensionalIndexerRule 
  type CLIArgs =
    | Force of bool
    | FailFast of bool
    | ShowSummary of string
    | Many of CLIArgs seq