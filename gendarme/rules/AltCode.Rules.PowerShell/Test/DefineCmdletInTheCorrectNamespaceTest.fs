namespace Test.AltCode.PowerShell

open System
open System.Reflection

open Mono.Cecil
// Add open for the project of the rule being tested.
open AltCode.Rules.PowerShell

open NUnit.Framework
open Test.Rules.Fixtures
open Test.Rules.Helpers
open Test.Rules.Definitions

open Examples.AltCode.PowerShell
open Examples.AltCode.Commands

[<TestFixture>]
type DefineCmdletInTheCorrectNamespaceTest() =
  inherit
    TypeRuleTestFixture<AltCode.Rules.PowerShell.DefineCmdletInTheCorrectNamespaceRule>()

  [<Test>]
  member this.DoesNotApply() =
    base.AssertRuleDoesNotApply<UndecoratedClassesAreNotCmdlets>()
    base.AssertRuleDoesNotApply<StructsAreNotCmdlets>()

    let probe =
      UndecoratedClassesAreNotCmdlets.Internal()

    ``base``.AssertRuleDoesNotApply(DefinitionLoader.GetTypeDefinition probe)
    base.AssertRuleDoesNotApply<UnattributedTypesAreNotCmdlets>()

  [<Test>]
  member this.Good() = base.AssertRuleSuccess<MergeThings>()

  [<Test>]
  member this.Bad() =
    base.AssertRuleFailure<WrongNamespace>()