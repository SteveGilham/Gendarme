namespace DelegatesPassedToNativeCodeMustIncludeExceptionHandling

open System

[<NoComparison>]
type SummaryFormat =
  | Default
  | N
  | O
  | C
  | R
  | B
  | Many of SummaryFormat list
  static member ToList x =
    match x with
    | Many l -> l
    | f -> [ f ]

  static member Factory(s: string) =
    let expanded =
      match s with
      | x when String.IsNullOrWhiteSpace x -> "B"
      | "+" -> "BOC"
      | _ -> s.Replace("+", "OC").ToUpperInvariant()

    try
      expanded
      |> Seq.distinct
      |> Seq.fold
           (fun state c ->
             match (c, state) with
             | ('N', _)
             | (_, N) -> N
             | ('B', _) -> Many(B :: (SummaryFormat.ToList state))
             | ('R', _) -> Many(R :: (SummaryFormat.ToList state))
             | ('O', _) -> Many(O :: (SummaryFormat.ToList state))
             | ('C', _) -> Many(C :: (SummaryFormat.ToList state))
             | _ -> raise (FormatException s))
           (Many [])
    with
    | :? FormatException -> Default