namespace MethodCanBeMadeStatic

open System.Collections.Generic
open Mono.Cecil

module Instrument =
  let internal resolutionTable = Dictionary<string, AssemblyDefinition>()

  let internal resolveFromNugetCache _ (y : AssemblyNameReference) =
    let name = y.ToString()
    if resolutionTable.ContainsKey name then
      resolutionTable.[name]
    else
      null

  let hookResolveHandler = new AssemblyResolveEventHandler(resolveFromNugetCache)
