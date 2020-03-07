namespace AltCode.CecilExtensions

[<AutoOpen>]
module internal Augment =

  type System.Object with
    member self.IsNotNull
      with get() =
        self |> isNull |> not

  type Microsoft.FSharp.Core.Option<'T> with
    static member getOrElse (fallback : 'T) (x : option<'T>) = defaultArg x fallback
    static member nullable (x : 'T) : option<'T> =
      if isNull (x :> obj) then None else Some x