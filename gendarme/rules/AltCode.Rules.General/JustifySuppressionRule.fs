namespace AltCode.Rules.General

open System

open Mono.Cecil
open Mono.Cecil.Cil

open Gendarme.Framework
open Gendarme.Framework.Engines
open Gendarme.Framework.Helpers
open Gendarme.Framework.Rocks

[<Problem("The namespace in which the cmdlet class is defined must have a name in the following format: '<Product>.Commands'.>")>]
[<Solution("Move or rename the class to a namespace that identifies the product and ends in '.Commands'.")>]
[<Sealed>]
type JustifySuppressionRule() =
  inherit Rule()

  interface ITypeRule with
    member this.CheckType(td: TypeDefinition) : RuleResult =
        RuleResult.DoesNotApply

  interface IMethodRule with
    member this.CheckMethod(md: MethodDefinition) : RuleResult =
        RuleResult.DoesNotApply

  interface IAssemblyRule with
    member this.CheckAssembly(ad: AssemblyDefinition) : RuleResult =
        RuleResult.DoesNotApply

  interface IRule with // keep compiler happy
    member this.TearDown () = ()