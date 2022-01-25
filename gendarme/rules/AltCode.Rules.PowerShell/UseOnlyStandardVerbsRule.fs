namespace AltCode.Rules.PowerShell

open System
open System.Globalization

open Mono.Cecil
open Mono.Cecil.Cil

open Gendarme.Framework
open Gendarme.Framework.Engines
open Gendarme.Framework.Helpers
open Gendarme.Framework.Rocks
open System.Diagnostics.CodeAnalysis

[<Problem("The namespace in which the cmdlet class is defined must have a name in the following format: '<Product>.Commands'.>")>]
[<Solution("Move or rename the class to a namespace that identifies the product and ends in '.Commands'.")>]
[<FxCopCompatibility("Microsoft.PowerShell", "PS1001:UseOnlyStandardVerbs")>]
[<Sealed>]
type UseOnlyStandardVerbsRule() =
  inherit Rule()

  interface ITypeRule with
    [<SuppressMessage("Microsoft.Design",
                      "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification = "F# interfaces are like that")>]
    member this.CheckType(``type``: TypeDefinition) : RuleResult =
      let td = ``type``

      if Tools.IsCmdlet td then
        let attr =
          td.CustomAttributes
          |> Seq.find (fun a -> a.AttributeType.Inherits Tools.cmdletAttribute)

        let verb =
          (attr.ConstructorArguments |> Seq.head)
            .Value.ToString()

        if Tools.standardVerbs
           |> Seq.exists (fun v -> v.Equals(verb, StringComparison.OrdinalIgnoreCase))
           |> not then
          let msg =
            String.Format(
              CultureInfo.InvariantCulture,
              "Non-standard verb {0} used here.",
              verb
            )

          this.Runner.Report(td, Severity.High, Confidence.High, msg)

        this.Runner.CurrentRuleResult

      else
        RuleResult.DoesNotApply