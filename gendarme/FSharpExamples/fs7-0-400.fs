namespace Fs7x0x400

open System
open Mono.Options

[<System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage;
  NoComparison;
  AutoSerializable(false)>]
type internal UsageInfo =
  { Intro: String
    Options: OptionSet
    Options2: OptionSet }