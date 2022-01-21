namespace Tests.AltCode.General

open System
open System.Reflection

open Mono.Cecil
// Add open for the project of the rule being tested.
open AltCode.Rules.General

open NUnit.Framework
open Test.Rules.Fixtures
open Test.Rules.Helpers
open Test.Rules.Definitions

[<TestFixture>]
type TypeJustifySuppressionTest() =
  inherit TypeRuleTestFixture<AltCode.Rules.General.JustifySuppressionRule>()

  [<Test>]
  member this.DoesNotApply() =
    //base.AssertRuleDoesNotApply<DefineCmdletInTheCorrectNamespaceRule>
    ()

  [<Test>]
  member this.Good() =
    // AssertRuleSuccess<type> ()
    ()

  [<Test>]
  member this.Bad() =
    // AssertRuleFailure<type> ()
    ()

[<TestFixture>]
type MethodJustifySuppressionRule() =
  inherit MethodRuleTestFixture<AltCode.Rules.General.JustifySuppressionRule>()

  [<Test>]
  member this.DoesNotApply() =
    //base.AssertRuleDoesNotApply<DefineCmdletInTheCorrectNamespaceRule>
    ()

  [<Test>]
  member this.Good() =
    // AssertRuleSuccess<type> ()
    ()

  [<Test>]
  member this.Bad() =
    // AssertRuleFailure<type> ()
    ()

[<TestFixture>]
type AssemblyJustifySuppressionRule() =
  inherit AssemblyRuleTestFixture<AltCode.Rules.General.JustifySuppressionRule>()

  [<Test>]
  member this.DoesNotApply() =
    //base.AssertRuleDoesNotApply<DefineCmdletInTheCorrectNamespaceRule>
    ()

  [<Test>]
  member this.Good() =
    // AssertRuleSuccess<type> ()
    ()

  [<Test>]
  member this.Bad() =
    // AssertRuleFailure<type> ()
    ()