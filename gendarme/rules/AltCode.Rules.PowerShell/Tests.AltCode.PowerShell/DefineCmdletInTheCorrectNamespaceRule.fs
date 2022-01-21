namespace Tests.AltCode.PowerShell

open System
open System.Reflection

open Mono.Cecil
// Add open for the project of the rule being tested.
open AltCode.Rules.PowerShell

open NUnit.Framework
open Test.Rules.Fixtures
open Test.Rules.Helpers
open Test.Rules.Definitions

[<TestFixture>]
type DefineCmdletInTheCorrectNamespaceRule() =
  inherit TypeRuleTestFixture<AltCode.Rules.PowerShell.DefineCmdletInTheCorrectNamespaceRule>()

  [<Test>]
  member this.DoesNotApply () =
    base.AssertRuleDoesNotApply<DefineCmdletInTheCorrectNamespaceRule> ()

  [<Test>]
  member this.Good () =
    // AssertRuleSuccess<type> ()
    ()

  [<Test>]
  member this.Bad () =
    // AssertRuleFailure<type> ()
    ()