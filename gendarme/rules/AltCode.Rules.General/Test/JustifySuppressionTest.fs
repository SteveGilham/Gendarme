namespace Test.AltCode.General

open System
open System.Reflection

open Mono.Cecil
// Add open for the project of the rule being tested.
open AltCode.Rules.General

open NUnit.Framework
open Test.Rules.Fixtures
open Test.Rules.Helpers
open Test.Rules.Definitions

open Examples.AltCode.General

[<TestFixture>]
type TypeJustifySuppressionTest() =
  inherit TypeRuleTestFixture<AltCode.Rules.General.JustifySuppressionRule>()

  // [<Test>]
  // member this.DoesNotApply() =
  //   //base.AssertRuleDoesNotApply<DefineCmdletInTheCorrectNamespaceRule>
  //   ()

  [<Test>]
  member this.Good() =
    base.AssertRuleSuccess<TypeJustifySuppressionTest>()

  [<Test>]
  member this.Bad() =
    base.AssertRuleFailure<Justifications>()

[<TestFixture>]
type MethodJustifySuppressionTest() =
  inherit MethodRuleTestFixture<AltCode.Rules.General.JustifySuppressionRule>()

  // [<Test>]
  // member this.DoesNotApply() =
  //   //base.AssertRuleDoesNotApply<DefineCmdletInTheCorrectNamespaceRule>
  //   ()

  [<Test>]
  member this.Good() =
    base.AssertRuleSuccess<Justifications>("YetAnotherToken")
    base.AssertRuleSuccess<Justifications>("Token4")

  [<Test>]
  member this.Bad() =
    base.AssertRuleFailure<Justifications>("Token")
    base.AssertRuleFailure<Justifications>("EmptyToken")
    base.AssertRuleFailure<Justifications>("AnotherToken")

[<TestFixture>]
type AssemblyJustifySuppressionTest() =
  inherit AssemblyRuleTestFixture<AltCode.Rules.General.JustifySuppressionRule>()

  // [<Test>]
  // member this.DoesNotApply() =
  //   //base.AssertRuleDoesNotApply<DefineCmdletInTheCorrectNamespaceRule>
  //   ()

  [<Test>]
  member this.Good() =
    let a =
      System
        .Reflection
        .Assembly
        .GetExecutingAssembly()
        .Location

    use assembly = AssemblyDefinition.ReadAssembly(a)
    base.AssertRuleSuccess(assembly)

  [<Test>]
  member this.Bad() =
    let a = typeof<Justifications>.Assembly.Location
    use assembly = AssemblyDefinition.ReadAssembly(a)
    base.AssertRuleFailure(assembly)