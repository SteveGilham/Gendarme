namespace AltCode.CecilExtensions

open System.Diagnostics.CodeAnalysis
open System.Runtime.CompilerServices

[<AutoOpen>]
module internal Augment =

  type System.Object with
    member self.IsNotNull = self |> isNull |> not

  [<MethodImplAttribute(MethodImplOptions.AggressiveInlining)>]
  let (==) (x: string) (y: string) =
    x.Equals(y, System.StringComparison.Ordinal)

  [<MethodImplAttribute(MethodImplOptions.AggressiveInlining)>]
  let (!=) (x: string) (y: string) = (x == y) |> not

  [<SuppressMessage("Microsoft.Globalization",
                    "CA1307:SpecifyStringComparison",
                    Justification =
                      "Preferred overload, no comparison exists in netstd2.0/net472")>]
  let internal charIndexOf (name: string) (token: char) = name.IndexOf(token)