namespace AltCode.Rules.PowerShell

open System

open Mono.Cecil
open Mono.Cecil.Cil

open Gendarme.Framework
open Gendarme.Framework.Engines
open Gendarme.Framework.Helpers
open Gendarme.Framework.Rocks

[<Problem ("The namespace in which the cmdlet class is defined must have a name in the following format: '<Product>.Commands'.>")>]
[<Solution ("Move or rename the class to a namespace that identifies the product and ends in '.Commands'.")>]
type DefineCmdletInTheCorrectNamespaceRule() =
  inherit Rule()
  interface ITypeRule with
    member this.CheckType(td : TypeDefinition) : RuleResult =
      if Tools.IsCmdlet td
      then if td.Namespace.EndsWith(".Commands", StringComparison.Ordinal)
           then RuleResult.Success
           else RuleResult.Failure
      else RuleResult.DoesNotApply