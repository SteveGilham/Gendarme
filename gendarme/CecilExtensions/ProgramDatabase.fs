namespace AltCode.CecilExtensions

open System
open System.Collections.Generic
open System.Diagnostics.CodeAnalysis
open System.IO

open Mono.Cecil
open Mono.Cecil.Cil
open Mono.Cecil.Mdb
open Mono.Cecil.Pdb

// Code from https://github.com/SteveGilham/altcover/commit/431fa57e4bf1e75f2cf2c01c9c61726a57f894dc
// as modified at https://github.com/SteveGilham/altcover/commit/324f13c275ff020f60c4216969b668e7766e544a
// plus retrofit for the old F# version we are using here

module ProgramDatabase =
  let internal symbolFolders = List<String>()

  // We no longer have to violate Cecil encapsulation to get the PDB path
  // but we do to get the embedded PDB info
  let internal getEmbed =
    (typeof<Mono.Cecil.AssemblyDefinition>.Assembly.GetTypes ()
     |> Seq.filter (fun m -> m.FullName = "Mono.Cecil.Mixin")
     |> Seq.head)
      .GetMethod("GetEmbeddedPortablePdbEntry")

  let internal getEmbeddedPortablePdbEntry (assembly: AssemblyDefinition) =
    getEmbed.Invoke(null, [| assembly.MainModule.GetDebugHeader() :> obj |])
    :?> ImageDebugHeaderEntry

  let internal getPdbFromImage (assembly: AssemblyDefinition) =
    Some assembly.MainModule
    |> Option.filter (fun x -> x.HasDebugHeader)
    |> Option.map (fun x -> x.GetDebugHeader())
    |> Option.filter (fun x -> x.HasEntries)
    |> Option.bind (fun x -> x.Entries |> Seq.tryFind (fun t -> true))
    |> Option.map (fun x -> x.Data)
    |> Option.filter (fun x -> x.Length > 0x18)
    |> Option.map (fun x ->
      x
      |> Seq.skip 0x18 // size of the debug header
      |> Seq.takeWhile (fun x -> x <> byte 0)
      |> Seq.toArray
      |> System.Text.Encoding.UTF8.GetString)
    |> Option.filter (fun s -> s.Length > 0)
    |> Option.filter (fun s ->
      File.Exists s
      || (s = (assembly.Name.Name + ".pdb")
          && (assembly
              |> getEmbeddedPortablePdbEntry
              |> isNull
              |> not)))

  let internal getSymbolsByFolder fileName folderName =
    let name =
      Path.Combine(folderName, fileName)

    let fallback =
      Path.ChangeExtension(name, ".pdb")

    if File.Exists(fallback) then
      Some fallback
    else
      let fallback2 = name + ".mdb"
      // Note -- the assembly path, not the mdb path, because GetSymbolReader wants the assembly path for Mono
      if File.Exists(fallback2) then
        Some name
      else
        None

  let internal getSymbolsWithFallback (assembly: AssemblyDefinition) =
    let path = assembly.MainModule.FileName

    match getPdbFromImage assembly with
    | None when path |> String.IsNullOrWhiteSpace |> not -> // i.e. assemblies read from disk only
      let foldername = Path.GetDirectoryName path
      let filename = Path.GetFileName path

      foldername :: (Seq.toList symbolFolders)
      |> Seq.map (getSymbolsByFolder filename)
      |> Seq.choose id
      |> Seq.tryFind (fun _ -> true)
    | pdbpath -> pdbpath

  // Ensure that we read symbols from the .pdb path we discovered.
  // Cecil currently only does the Path.ChangeExtension(path, ".pdb") fallback if left to its own devices
  // Will fail  with InvalidOperationException if there is a malformed file with the expected name
  let ReadSymbols (assembly: AssemblyDefinition) =
    getSymbolsWithFallback assembly
    |> Option.iter (fun pdbpath ->
      let provider: ISymbolReaderProvider =
        if pdbpath.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase) then
          PdbReaderProvider() :> ISymbolReaderProvider
        else
          MdbReaderProvider() :> ISymbolReaderProvider

      let reader =
        provider.GetSymbolReader(assembly.MainModule, pdbpath)

      assembly.MainModule.ReadSymbols(reader))

[<assembly: SuppressMessage("Microsoft.Performance",
                            "CA1810:InitializeReferenceTypeStaticFieldsInline",
                            Scope = "member",
                            Target = "<StartupCode$CecilExtensions>.$ProgramDatabase.#.cctor()",
                            Justification = "Compiler generated")>]
[<assembly: SuppressMessage("Microsoft.Naming",
                            "CA1704:IdentifiersShouldBeSpelledCorrectly",
                            Scope = "member",
                            Target = "AltCode.CecilExtensions.ProgramDatabase.#Option.filter`1(Microsoft.FSharp.Core.FSharpFunc`2<!!0,System.Boolean>,Microsoft.FSharp.Core.FSharpOption`1<!!0>)",
                            MessageId = "a",
                            Justification = "Compiler generated")>]
()