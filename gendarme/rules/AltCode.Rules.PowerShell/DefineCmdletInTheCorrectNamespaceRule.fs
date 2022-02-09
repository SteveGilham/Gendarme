namespace AltCode.Rules.PowerShell

open System

open Mono.Cecil
open Mono.Cecil.Cil

open Gendarme.Framework
open Gendarme.Framework.Engines
open Gendarme.Framework.Helpers
open Gendarme.Framework.Rocks
open System.Diagnostics.CodeAnalysis
open System.Globalization

[<Problem("The namespace in which the cmdlet class is defined must have a name in the following format: '<Product>.Commands'.>")>]
[<Solution("Move or rename the class to a namespace that identifies the product and ends in '.Commands'.")>]
[<FxCopCompatibility("Microsoft.PowerShell", "PS1011:DefineCmdletInTheCorrectNamespace")>]
[<Sealed>]
type DefineCmdletInTheCorrectNamespaceRule() =
  inherit Rule()

  interface ITypeRule with
    [<SuppressMessage("Microsoft.Design",
                      "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification = "F# interfaces are like that")>]
    member this.CheckType(``type``: TypeDefinition) : RuleResult =
      let td = ``type``
      let ns = td.Namespace

      if Tools.IsCmdlet td then
        if
          ns.EndsWith(".Commands", StringComparison.Ordinal)
          |> not
        then
          let msg =
            String.Format(
              CultureInfo.CurrentCulture,
              Tools.resource "IncorrectNamespace",
              ns
            )

          this.Runner.Report(td, Severity.High, Confidence.High, msg)

        this.Runner.CurrentRuleResult
      else
        RuleResult.DoesNotApply