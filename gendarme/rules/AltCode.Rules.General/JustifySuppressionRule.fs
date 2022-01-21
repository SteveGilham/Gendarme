namespace AltCode.Rules.General

open System
open System.Diagnostics.CodeAnalysis
open System.Globalization

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
    [<SuppressMessage("Microsoft.Design",
                      "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification = "F# interfaces are like that")>]
    member this.CheckType(``type``: TypeDefinition) : RuleResult =
      this.VerifyAttributes ``type``.CustomAttributes ``type`` ``type``

      ``type``.Events
      |> Seq.iter (fun f -> this.VerifyAttributes f.CustomAttributes f ``type``)

      ``type``.Fields
      |> Seq.iter (fun f -> this.VerifyAttributes f.CustomAttributes f ``type``)

      ``type``.Properties
      |> Seq.iter (fun f -> this.VerifyAttributes f.CustomAttributes f ``type``)

      this.Runner.CurrentRuleResult

  interface IMethodRule with
    [<SuppressMessage("Microsoft.Design",
                      "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification = "F# interfaces are like that")>]
    member this.CheckMethod(method: MethodDefinition) : RuleResult =
      this.VerifyAttributes method.CustomAttributes method method

      method.Parameters
      |> Seq.iter (fun f -> this.VerifyAttributes f.CustomAttributes f method)

      this.Runner.CurrentRuleResult

  interface IAssemblyRule with
    [<SuppressMessage("Microsoft.Design",
                      "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification = "F# interfaces are like that")>]
    member this.CheckAssembly(assembly: AssemblyDefinition) : RuleResult =
      this.VerifyAttributes assembly.CustomAttributes assembly assembly
      this.Runner.CurrentRuleResult

  interface IRule with
    [<SuppressMessage("Microsoft.Design",
                      "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification = "F# interfaces are like that")>]
    member this.TearDown() = ()

  // Common code to scan each node for its attributes and check justifications
  member private self.VerifyAttributes
    attributes
    (location: ICustomAttributeProvider)
    (target: ICustomAttributeProvider)
    =
    attributes
    |> Seq.cast<CustomAttribute>
    |> Seq.filter
         (fun attribute ->
           let t = attribute.AttributeType

           t.Name = "SuppressMessageAttribute"
           && t.Namespace = "System.Diagnostics.CodeAnalysis")
    |> Seq.iter (self.CheckJustification location target)

  // Separates sheep from goats so far as Justification strings go
  member private self.CheckJustification location target (attribute: CustomAttribute) =
    attribute.Properties
    |> Seq.filter (fun a -> a.Name = "Justification")
    |> Seq.iter
         (fun a ->
           match a.Argument.Value :?> string with
           | y when String.IsNullOrWhiteSpace(y) -> self.Violation location target y
           | x when x.Trim().Length < 10 -> self.Violation location target x
           | _ -> ())

  member private self.Violation location target just =
    let msg =
      String.Format(CultureInfo.InvariantCulture, "Insufficient justification {0}", just)

    let defect =
      Defect(self, target, location, Severity.Medium, Confidence.High, msg)

    self.Runner.Report defect

// to implement
// "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"
// "Microsoft.Design", "CA1048:DoNotDeclareVirtualMembersInSealedTypes"