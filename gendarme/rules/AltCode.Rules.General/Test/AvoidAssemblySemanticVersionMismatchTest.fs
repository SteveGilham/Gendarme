namespace Test.AltCode.General

open System
open System.Reflection

open Mono.Cecil

open Gendarme.Framework
// Add open for the project of the rule being tested.
open AltCode.Rules.General

open NUnit.Framework
open Test.Rules.Fixtures
open Test.Rules.Helpers
open Test.Rules.Definitions

open Examples.AltCode.General

[<TestFixture>]
type AvoidAssemblySemanticVersionMismatchTest() =
  inherit AssemblyRuleTestFixture<AltCode.Rules.General.AvoidAssemblySemanticVersionMismatchRule>()

  member val private assembly: AssemblyDefinition = null with get, set

  [<OneTimeSetUp>]
  member this.FixtureSetUp() =
    let unit =
      System
        .Reflection
        .Assembly
        .GetExecutingAssembly()
        .Location

    this.assembly <- AssemblyDefinition.ReadAssembly(unit)

  [<Test>]
  member this.EmptyAssemblyVersion() =
    let v = this.assembly.Name.Version

    try
      this.assembly.Name.Version <- null // should not happen
      base.AssertRuleFailure(this.assembly, 1)
      Assert.AreEqual(Severity.Medium, this.Runner.Defects.[0].Severity, "Medium")

      this.assembly.Name.Version <- Version(0, 0, 0, 0)
      base.AssertRuleFailure(this.assembly, 1)
      Assert.AreEqual(Severity.Medium, this.Runner.Defects.[0].Severity, "Medium")
    finally
      this.assembly.Name.Version <- v

  [<Test>]
  member this.EmptyCustomAttributes() =
    let cac =
      this.assembly.CustomAttributes |> Seq.toList

    try
      this.assembly.CustomAttributes.Clear()
      base.AssertRuleFailure(this.assembly, 1)
      Assert.AreEqual(Severity.Medium, this.Runner.Defects.[0].Severity, "Medium")
    finally
      cac |> Seq.iter this.assembly.CustomAttributes.Add

  [<Test>]
  member this.AbsentAssemblyFileVersion() =
    let afv =
      this.assembly.CustomAttributes
      |> Seq.find
           (fun ca ->
             ca.AttributeType.FullName = "System.Reflection.AssemblyFileVersionAttribute")

    try
      Assert.That(
        this.assembly.CustomAttributes.Remove afv,
        "AssemblyFileVersionAttribute not found"
      )

      base.AssertRuleFailure(this.assembly, 1)
      Assert.AreEqual(Severity.Medium, this.Runner.Defects.[0].Severity, "Medium")
    finally
      this.assembly.CustomAttributes.Add afv

  [<Test>]
  member this.EmptyAssemblyFileVersion() =
    let afv =
      this.assembly.CustomAttributes
      |> Seq.find
           (fun ca ->
             ca.AttributeType.FullName = "System.Reflection.AssemblyFileVersionAttribute")

    let version = afv.ConstructorArguments.[0]

    try
      afv.ConstructorArguments[ 0 ] <- CustomAttributeArgument()
      base.AssertRuleFailure(this.assembly, 1)
      Assert.AreEqual(Severity.Medium, this.Runner.Defects.[0].Severity, "Medium")
    finally
      afv.ConstructorArguments[ 0 ] <- version

  [<Test>]
  member this.VersionMatch() =
    // full 4 facets by construction
    base.AssertRuleSuccess(this.assembly)

  // fsharplint:disable  NonPublicValuesNames

  [<Test>]
  member this.VersionMismatch() =
    let av = this.assembly.Name.Version

    let afv =
      this.assembly.CustomAttributes
      |> Seq.find
           (fun ca ->
             ca.AttributeType.FullName = "System.Reflection.AssemblyFileVersionAttribute")

    let fv = afv.ConstructorArguments.[0]

    let s =
      DefinitionLoader.GetTypeDefinition(typeof<string>)

    try
      this.assembly.Name.Version <- Version(8, 2)

      let file8_2 = CustomAttributeArgument(s, "8.2")
      afv.ConstructorArguments.[0] <- file8_2
      base.AssertRuleSuccess(this.assembly)

      let file7_1 = CustomAttributeArgument(s, "7.1")
      afv.ConstructorArguments.[0] <- file7_1
      base.AssertRuleFailure(this.assembly, 1)
      Assert.AreEqual(Severity.Critical, this.Runner.Defects.[0].Severity, "Critical")

      let file8_1 = CustomAttributeArgument(s, "8.1")
      afv.ConstructorArguments.[0] <- file8_1
      base.AssertRuleFailure(this.assembly, 1)
      Assert.AreEqual(Severity.Critical, this.Runner.Defects.[0].Severity, "Critical")

      let file8_3 = CustomAttributeArgument(s, "8.3")
      afv.ConstructorArguments.[0] <- file8_3
      base.AssertRuleFailure(this.assembly, 1)
      Assert.AreEqual(Severity.Critical, this.Runner.Defects.[0].Severity, "Critical")

      let file9_0 = CustomAttributeArgument(s, "9.0")
      afv.ConstructorArguments.[0] <- file9_0
      base.AssertRuleFailure(this.assembly, 1)
      Assert.AreEqual(Severity.Critical, this.Runner.Defects.[0].Severity, "Critical")

      let file8_2_1 = CustomAttributeArgument(s, "8.2.1")
      afv.ConstructorArguments.[0] <- file8_2_1
      base.AssertRuleSuccess(this.assembly)

      let file8_2_18_22015 =
        CustomAttributeArgument(s, "8.2.18.22015")

      afv.ConstructorArguments.[0] <- file8_2_18_22015
      base.AssertRuleSuccess(this.assembly)

      this.assembly.Name.Version <- Version(8, 2, 1)
      afv.ConstructorArguments.[0] <- file8_2_1
      base.AssertRuleSuccess(this.assembly)

      afv.ConstructorArguments.[0] <- file8_2_18_22015
      base.AssertRuleFailure(this.assembly, 1)
      Assert.AreEqual(Severity.High, this.Runner.Defects.[0].Severity, "High")

      let file8_2_1_22015 =
        CustomAttributeArgument(s, "8.2.1.22015")

      afv.ConstructorArguments.[0] <- file8_2_1_22015
      base.AssertRuleSuccess(this.assembly)

      this.assembly.Name.Version <- Version(8, 2, 1, 1)
      base.AssertRuleFailure(this.assembly, 1)
      Assert.AreEqual(Severity.Medium, this.Runner.Defects.[0].Severity, "Medium")
    finally
      this.assembly.Name.Version <- av
      afv.ConstructorArguments [0] <- fv