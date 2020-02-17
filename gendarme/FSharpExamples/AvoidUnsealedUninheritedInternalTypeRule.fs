namespace AvoidUnsealedUninheritedInternalType

open System.Diagnostics.CodeAnalysis;

type internal Track =
  | Null
  | Time of int64
  | Call of int


[<SuppressMessage("Gendarme.Rules.Smells","AvoidSpeculativeGeneralityRule")>]
module Tracking =
  let createNull() = Track.Null :> obj
  let createTime i = (Track.Time i) :> obj
  let createCall i = (Track.Call i) :> obj

