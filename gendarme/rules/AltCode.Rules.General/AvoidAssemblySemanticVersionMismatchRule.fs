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

/// <summary>
/// This rule checks that the <c>[AssemblyVersion]</c> matches the <c>[AssemblyFileVersion]</c>
/// when both are present inside an assembly. Having different version numbers in both
/// attributes can be confusing once the application is deployed.
/// </summary>
/// <example>
/// Bad example:
/// <code>
/// [assembly: AssemblyVersion ("2.2.0.0")]
/// [assembly: AssemblyFileVersion ("1.0.0.0")]
/// </code>
/// </example>
/// <example>
/// Good example:
/// <code>
/// [assembly: AssemblyVersion ("2.2.0.0")]
/// [assembly: AssemblyFileVersion ("2.2.18.22015")]
/// </code>
/// </example>

[<Problem("The assembly version (API contract), from [AssemblyVersion], is not consistent with the file version, from [AssemblyFileVersion].")>]
[<Solution("This situation can be confusing once deployed. Make the file version is a sub-version of the contract semantic version.")>]
[<Sealed>]
[<AutoSerializable(false)>]
type AvoidAssemblySemanticVersionMismatchRule() =
  inherit Rule()

  let afva =
    TypeName(Namespace = "System.Reflection", Name = "AssemblyFileVersionAttribute")

  interface IAssemblyRule with
    [<SuppressMessage("Microsoft.Design",
                      "CA1048:DoNotDeclareVirtualMembersInSealedTypes",
                      Justification = "F# interfaces are like that")>]
    member this.CheckAssembly(assembly: AssemblyDefinition) : RuleResult =
      // once compiled [AssemblyVersion] is not part of the custom attributes
      let assemblyVersion = assembly.Name.Version

      if (assembly.HasCustomAttributes |> not
          || assemblyVersion.IsEmpty()) then
        let msg =
          Tools.resource "IncompleteVersioning"

        this.Runner.Report(assembly, Severity.Medium, Confidence.High, msg)
        RuleResult.Failure
      else
        let fileVersion =
          assembly.CustomAttributes
          |> Seq.filter (fun ca -> ca.HasConstructorArguments)
          |> Seq.filter (fun ca -> ca.AttributeType.IsNamed(afva))
          |> Seq.map (fun ca -> ca.ConstructorArguments.[0].Value)
          |> Seq.filter (isNull >> not)
          |> Seq.map (fun ca -> Version.TryParse(ca.ToString()) |> snd)
          |> Seq.tryHead

        match fileVersion with
        | None ->
          let msg =
            Tools.resource "IncompleteVersioning"

          this.Runner.Report(assembly, Severity.Medium, Confidence.High, msg)
          RuleResult.Failure
        | Some version ->
          let s =
            if assemblyVersion.Major <> version.Major
               || //primary sem-ver facets
               assemblyVersion.Minor <> version.Minor then
              Some Severity.Critical
            else if (assemblyVersion.Build > 0)
                    && // if non-default, must match
                    (assemblyVersion.Build <> version.Build) then
              Some Severity.High
            else if (assemblyVersion.Revision > 0)
                    && // if non-default, must match
                    (assemblyVersion.Revision <> version.Revision) then
              Some Severity.Medium
            else
              None

          match s with
          | None -> RuleResult.Success
          | Some severity ->
            let msg =
              String.Format(
                CultureInfo.CurrentCulture,
                Tools.resource "MismatchedVersioning",
                assemblyVersion,
                fileVersion
              )

            this.Runner.Report(assembly, severity, Confidence.High, msg)
            RuleResult.Failure