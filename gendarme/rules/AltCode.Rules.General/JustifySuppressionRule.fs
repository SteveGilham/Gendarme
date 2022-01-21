namespace AltCode.Rules.General

open System
open System.Diagnostics.CodeAnalysis

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
    [<SuppressMessage("Microsoft.Design", "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification="F# interfaces are like that")>]
    member this.CheckType(``type``: TypeDefinition) : RuleResult =
        RuleResult.DoesNotApply

  interface IMethodRule with
    [<SuppressMessage("Microsoft.Design", "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification="F# interfaces are like that")>]
    member this.CheckMethod(``method``: MethodDefinition) : RuleResult =
        RuleResult.DoesNotApply

  interface IAssemblyRule with
    [<SuppressMessage("Microsoft.Design", "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification="F# interfaces are like that")>]
    member this.CheckAssembly(assembly: AssemblyDefinition) : RuleResult =
        RuleResult.DoesNotApply

  interface IRule with // keep compiler happy
    [<SuppressMessage("Microsoft.Design", "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification="F# interfaces are like that")>]
    member this.TearDown () = ()

[<assembly: SuppressMessage("Gendarme.Rules.Gendarme",
                            "UseCorrectSuffixRule",
                            Scope = "type", // TypeDefinition
                            Target = "<StartupCode$AltCode-Rules-General>.$JustifySuppressionRule",
                            Justification = "Rule needs fixing")>]
[<assembly: SuppressMessage("Gendarme.Rules.Gendarme",
                            "DefectsMustBeReportedRule",
                            Scope = "type", // TypeDefinition
                            Target = "AltCode.Rules.General.JustifySuppressionRule",
                            Justification = "Work in progress")>]
()
// to implement
// "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"