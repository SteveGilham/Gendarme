namespace AltCode.CecilExtensions

[<AutoOpen>]
module internal Augment =

  type System.Object with
    member self.IsNotNull = self |> isNull |> not

  type Microsoft.FSharp.Core.Option<'T> with
    static member DefaultValue (fallback : 'T) (x : option<'T>) = defaultArg x fallback