namespace Test.AltCode.PowerShell

// Add open for the project of the rule being tested.
open AltCode.Rules.PowerShell

open NUnit.Framework
open Test.Rules.Fixtures
open Test.Rules.Helpers

open Examples.AltCode.PowerShell
open Examples.AltCode.Commands

[<TestFixture>]
type UseOnlyStandardVerbsTest() =
  inherit TypeRuleTestFixture<UseOnlyStandardVerbsRule>()

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