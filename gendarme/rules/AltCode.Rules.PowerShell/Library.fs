namespace AltCode.Rules.PowerShell

open System.Diagnostics.CodeAnalysis

open Mono.Cecil
open Gendarme.Framework
open Gendarme.Framework.Engines
open Gendarme.Framework.Helpers
open Gendarme.Framework.Rocks

module Tools =
  let cmdlet =
    TypeName(Namespace = "System.Management.Automation", Name = "Cmdlet")

  let cmdletAttribute =
    TypeName(Namespace = "System.Management.Automation", Name = "CmdletAttribute")

  let IsCmdlet (td: TypeDefinition) =
    Some td
    |> Option.filter (fun t -> t.IsClass)
    |> Option.filter (fun t -> t.IsPublic)
    |> Option.filter (fun t -> t.Inherits cmdlet)
    |> Option.filter
         (fun t ->
           t.CustomAttributes
           |> Seq.exists (fun a -> a.AttributeType.Inherits cmdletAttribute))
    |> Option.isSome

[<assembly: SuppressMessage("Microsoft.Performance",
                            "CA1810:InitializeReferenceTypeStaticFieldsInline",
                            Scope = "member",
                            Target = "<StartupCode$AltCode-Rules-PowerShell>.$Library.#.cctor()",
                            Justification = "Compiler generated type")>]
()