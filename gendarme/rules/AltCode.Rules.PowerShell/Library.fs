namespace AltCode.Rules.PowerShell

open Mono.Cecil

module Tools =
  let IsCmdlet (td:TypeDefinition) =
    // TODO
    false

module Say =
    let hello name =
        sprintf "Hello %s" name