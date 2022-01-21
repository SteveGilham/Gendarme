namespace AltCode.Rules.PowerShell

open Mono.Cecil

module Tools =
  [<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters",
                                                    Justification="Work in progress")>]
  let IsCmdlet (td: TypeDefinition) =
    // TODO
    false