module AvoidRepetitiveCasts
open System
open System.Globalization

type ContextItem =
  | CallItem of String
  | TimeItem of uint8
  member self.AsString() =
    match self with
    | CallItem c -> c
    | TimeItem t -> t.ToString(CultureInfo.InvariantCulture)

type Context =
  | Context of ContextItem seq
  | NoContext
  member self.AsStrings() =
    match self with
    | NoContext -> List.empty<String>
    | Context c ->
        c
        |> Seq.map (fun a -> a.AsString())
        |> Seq.toList