namespace AvoidUnneededUnboxing

open System

module CommandLine =
  let logException (e: Exception) =
    printfn "%A" e

  let internal doPathOperation (f: unit -> 'a) (defaultValue: 'a) store =
    let mutable result = defaultValue

    try
      result <- f ()
    with
    | :? ArgumentException as a -> a |> logException
    | :? NotSupportedException as n -> n |> logException
    | :? IO.IOException as i -> i |> logException
    | :? System.Security.SecurityException as s ->
      s |> logException
    | :? UnauthorizedAccessException as u -> u |> logException

    result