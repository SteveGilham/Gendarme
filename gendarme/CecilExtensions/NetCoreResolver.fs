﻿namespace AltCode.CecilExtensions

open System
open System.Collections.Generic
open System.IO
open System.Reflection

open Mono.Cecil

module NetCoreResolver =

  let private nugetCache =
    Path.Combine
      (Path.Combine
        (Environment.GetFolderPath Environment.SpecialFolder.UserProfile, ".nuget"),
       "packages")
  let internal ResolutionTable = Dictionary<string, AssemblyDefinition>()

  let internal FindAssemblyName f =
    try
      (AssemblyName.GetAssemblyName f).ToString()
    with
    | :? ArgumentException
    | :? FileNotFoundException
    | :? System.Security.SecurityException
    | :? BadImageFormatException
    | :? FileLoadException -> String.Empty

  let internal ResolveFromNugetCache _ (y : AssemblyNameReference) =
    let name = y.ToString()
    if ResolutionTable.ContainsKey name then
      ResolutionTable.[name]
    else
      // Console.WriteLine("Resolving assembly reference {0}", name)
      // Placate Gendarme here
      let share = "|usr|share".Replace('|', Path.DirectorySeparatorChar)
      let shared = "dotnet|shared".Replace('|', Path.DirectorySeparatorChar)

      let sources =
        [ Environment.GetEnvironmentVariable "NUGET_PACKAGES"
          Path.Combine
            (Environment.GetEnvironmentVariable "ProgramFiles"
             |> Option.ofObj
             |> (Option.defaultValue share), shared)
          Path.Combine(share, shared)
          nugetCache ]

      let candidate source =
        source
        |> List.filter (String.IsNullOrWhiteSpace >> not)
        |> List.filter Directory.Exists
        |> Seq.distinct
        |> Seq.collect
             (fun dir ->
               Directory.GetFiles(dir, y.Name + ".*", SearchOption.AllDirectories))
        |> Seq.sortDescending
        |> Seq.filter (fun f ->
             let x = Path.GetExtension f
             x.Equals(".exe", StringComparison.OrdinalIgnoreCase)
             || x.Equals(".dll", StringComparison.OrdinalIgnoreCase))
        |> Seq.filter (fun f ->
             name.Equals(FindAssemblyName f, StringComparison.Ordinal))
        |> Seq.tryHead
      match candidate sources with
      | None -> null
      | Some x ->
          String.Format
            (System.Globalization.CultureInfo.InvariantCulture,
             Environment.NewLine +
             "Resolved assembly reference '{0}' as file '{1}'.", name, x)
          |> Console.WriteLine
          let a = AssemblyDefinition.ReadAssembly x
          ResolutionTable.[name] <- a
          a

  let ResolveHandler = new AssemblyResolveEventHandler(ResolveFromNugetCache)

  let internal HookTable = HashSet<WeakReference>()

  let HookResolver(resolver : IAssemblyResolver) =
    if resolver.IsNotNull
    then
      if HookTable
        |> Seq.map (fun wr -> wr.Target)
        |> Seq.exists (fun t -> obj.ReferenceEquals(t, resolver))
        |> not
      then
        let hook = resolver.GetType().GetMethod("add_ResolveFailure")
        hook.Invoke(resolver, [| ResolveHandler :> obj |]) |> ignore
        HookTable.Add(WeakReference(resolver)) |> ignore