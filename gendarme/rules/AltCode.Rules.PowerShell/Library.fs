namespace AltCode.Rules.PowerShell

open Mono.Cecil

module Tools =
  let IsCmdlet (td: TypeDefinition) =
    // TODO
    false