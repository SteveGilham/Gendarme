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
type PreferStrongNamedAssembliesTest() =
  inherit AssemblyRuleTestFixture<AltCode.Rules.General.PreferStrongNamedAssembliesRule>()

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
    use stream =
      Assembly
        .GetExecutingAssembly()
        .GetManifestResourceStream("Test.AltCode.General.Sample1.exe")

    use assembly = AssemblyDefinition.ReadAssembly stream
    base.AssertRuleFailure(assembly)