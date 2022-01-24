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

[<Problem("If an analysis warning is to be overridden, there should be a reason why.")>]
[<Solution("Specify a reason why the code could not be brought into conformance with the appropriate rule.  Must be > 10 characters.")>]
[<FxCopCompatibility("Dixon.Design", "DX0001:JustifySuppression")>]
[<Sealed>]
type JustifySuppressionRule() =
  inherit Rule()

  interface ITypeRule with
    [<SuppressMessage("Microsoft.Design",
                      "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification = "F# interfaces are like that")>]
    member this.CheckType(``type``: TypeDefinition) : RuleResult =
      this.VerifyAttributes ``type``.CustomAttributes ``type`` ``type``

      // // not scanned at this level by Gendarme or FxCop
      // ``type``.Events
      // |> Seq.iter (fun f -> this.VerifyAttributes f.CustomAttributes f ``type``)

      // // not scanned at this level by Gendarme or FxCop
      // ``type``.Fields
      // |> Seq.iter (fun f -> this.VerifyAttributes f.CustomAttributes f ``type``)

      // // not scanned at this level by Gendarme or FxCop
      // ``type``.Properties
      // |> Seq.iter (fun f -> this.VerifyAttributes f.CustomAttributes f ``type``)

      this.Runner.CurrentRuleResult

  interface IMethodRule with
    [<SuppressMessage("Microsoft.Design",
                      "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification = "F# interfaces are like that")>]
    member this.CheckMethod(method: MethodDefinition) : RuleResult =
      this.VerifyAttributes method.CustomAttributes method method

      // // not scanned at this level by Gendarme or FxCop
      // method.Parameters
      // |> Seq.iter (fun f -> this.VerifyAttributes f.CustomAttributes f method)

      this.Runner.CurrentRuleResult

  interface IAssemblyRule with
    [<SuppressMessage("Microsoft.Design",
                      "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification = "F# interfaces are like that")>]
    member this.CheckAssembly(assembly: AssemblyDefinition) : RuleResult =
      // no namespace scan available here, even if FxCop allows namespace scope
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
    let j =
      attribute.Properties
      |> Seq.tryFind (fun a -> a.Name = "Justification")

    match j with
    | Some a ->
      match a.Argument.Value :?> string with
      | y when String.IsNullOrWhiteSpace(y) -> self.Violation location target y
      | x when x.Trim().Length < 10 -> self.Violation location target x
      | _ -> ()
    | _ -> self.Violation location target String.Empty

  member private self.Violation location target just =
    let msg =
      String.Format(
        CultureInfo.InvariantCulture,
        "Insufficient justification '{0}'",
        just
      )

    let defect =
      Defect(self, target, location, Severity.Medium, Confidence.High, msg)

    self.Runner.Report defect

// to implement
// "Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes"
// "Microsoft.Design", "CA1048:DoNotDeclareVirtualMembersInSealedTypes"