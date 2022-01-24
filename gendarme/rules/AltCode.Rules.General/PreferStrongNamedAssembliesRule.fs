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

[<Problem("The assembly is not strong-named.  It is better for code that is to be linked by third parties to strongnamed as it permits them a choice as to whether or not to strongname.")>]
[<Solution("Sign the assembly during build.")>]
[<FxCopCompatibility ("Microsoft.Design", "CA2210:Assemblies should have valid strong names")>]
[<Sealed>]
type PreferStrongNamedAssembliesRule() =
  inherit Rule()

  interface IAssemblyRule with
    [<SuppressMessage("Microsoft.Design",
                      "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification = "F# interfaces are like that")>]
    member self.CheckAssembly(assembly: AssemblyDefinition) : RuleResult =
      if assembly.Name.PublicKeyToken |> Array.isEmpty
      then
        let defect = Defect(self, assembly, assembly, Severity.Low, Confidence.High, "Assembly has no strong-name")
        self.Runner.Report defect

      self.Runner.CurrentRuleResult