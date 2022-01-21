namespace AltCode.Rules.PowerShell

open System

open Mono.Cecil
open Mono.Cecil.Cil

open Gendarme.Framework
open Gendarme.Framework.Engines
open Gendarme.Framework.Helpers
open Gendarme.Framework.Rocks

/// <summary>
/// TODO: Add a summary of the rule.
/// </summary>
/// <example>
/// Bad example:
/// <code>
/// TODO: Add an example where the rule would fail.
/// </code>
/// </example>
/// <example>
/// Good example:
/// <code>
/// TODO: Show how to fix the bad example.
/// </code>
/// </example>

// TODO: Describe the problem and solution
[<Problem ("")>]
[<Solution ("")>]
type DefineCmdletInTheCorrectNamespaceRule() =
  inherit Rule()
  interface ITypeRule with
    member this.CheckType(``type`` : TypeDefinition) : RuleResult =
      RuleResult.Success