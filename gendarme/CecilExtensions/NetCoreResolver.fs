namespace AltCode.CecilExtensions

open System
open System.Collections.Generic
open System.IO
open System.Reflection

open Mono.Cecil
open System.Diagnostics.CodeAnalysis

module NetCoreResolver =

  let private nugetCache =
    Path.Combine(
      Path.Combine(
        Environment.GetFolderPath Environment.SpecialFolder.UserProfile,
        ".nuget"
      ),
      "packages"
    )

  let internal resolutionTable = Dictionary<string, AssemblyDefinition>()
  let internal searchLocations = HashSet<string>()

  let internal findAssemblyName f =
    try
      (AssemblyName.GetAssemblyName f).ToString()
    with
    | :? ArgumentException
    | :? FileNotFoundException
    | :? System.Security.SecurityException
    | :? BadImageFormatException
    | :? FileLoadException -> String.Empty

  [<SuppressMessage("Microsoft.Usage",
                    "CA1801:ReviewUnusedParameters",
                    Justification = "meets interface")>]
  let internal resolveFromNugetCache _ (y: AssemblyNameReference) =
    let name = y.ToString()

    if resolutionTable.ContainsKey name then
      resolutionTable.[name]
    else
      // Console.WriteLine("Resolving assembly reference {0}", name)
      // Placate Gendarme here
      let share =
        "|usr|share"
          .Replace('|', Path.DirectorySeparatorChar)

      let shared =
        "dotnet|shared"
          .Replace('|', Path.DirectorySeparatorChar)

      let sources =
        [ Environment.GetEnvironmentVariable "NUGET_PACKAGES"
          Path.Combine(
            Environment.GetEnvironmentVariable "ProgramFiles"
            |> Option.ofObj
            |> (Option.defaultValue share),
            shared
          )
          Path.Combine(share, shared)
          nugetCache ]
        |> List.distinct

      let explicitSources =
        searchLocations
        |> Seq.toList
        |> List.filter (String.IsNullOrWhiteSpace >> not)
        |> List.filter Directory.Exists

      let candidate source =
        source
        |> List.filter (String.IsNullOrWhiteSpace >> not)
        |> List.filter Directory.Exists
        |> Seq.distinct
        |> Seq.collect
             (fun dir ->
               Directory.GetFiles(dir, y.Name + ".*", SearchOption.AllDirectories))
        |> Seq.sortDescending
        |> Seq.filter
             (fun f ->
               let x = Path.GetExtension f

               x.Equals(".exe", StringComparison.OrdinalIgnoreCase)
               || x.Equals(".dll", StringComparison.OrdinalIgnoreCase))
        |> Seq.filter (fun f -> name.Equals(findAssemblyName f, StringComparison.Ordinal))
        |> Seq.tryHead

      let handleResolved (x: string) =
        String.Format(
          System.Globalization.CultureInfo.InvariantCulture,
          Environment.NewLine
          + "Resolved assembly reference '{0}' as file '{1}'.",
          name,
          x
        )
        |> Console.WriteLine

        let a = AssemblyDefinition.ReadAssembly x
        resolutionTable.[name] <- a
        a

      match candidate explicitSources with
      | None ->
        match candidate sources with
        | None -> null
        | Some x -> handleResolved x
      | Some x -> handleResolved x

  let ResolveHandler =
    new AssemblyResolveEventHandler(resolveFromNugetCache)

  let internal hookTable = HashSet<WeakReference>()

  let AddSearchLocation path = path |> searchLocations.Add |> ignore

  let ClearSearchLocations () = searchLocations.Clear()

  let HookResolver (resolver: IAssemblyResolver) =
    if resolver.IsNotNull then
      if hookTable
         |> Seq.map (fun wr -> wr.Target)
         |> Seq.exists (fun t -> obj.ReferenceEquals(t, resolver))
         |> not then
        let hook =
          resolver.GetType().GetMethod("add_ResolveFailure")

        hook.Invoke(resolver, [| ResolveHandler :> obj |])
        |> ignore

        hookTable.Add(WeakReference(resolver)) |> ignore

[<assembly: SuppressMessage("Microsoft.Performance",
                            "CA1810:InitializeReferenceTypeStaticFieldsInline",
                            Scope = "member",
                            Target = "<StartupCode$CecilExtensions>.$NetCoreResolver.#.cctor()",
                            Justification = "Compiler generated")>]
()