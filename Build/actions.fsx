open System
open System.IO
open System.Reflection
open System.Xml
open System.Xml.Linq

open Fake.Core
open Fake.DotNet
open Fake.IO.FileSystemOperators
open Fake.IO
open Fake.IO.Globbing.Operators

open HeyRed.MarkdownSharp
open NUnit.Framework
open YamlDotNet.RepresentationModel

module Actions =
  let Clean() =
    let rec Clean1 depth =
      try
        (DirectoryInfo ".").GetDirectories("*", SearchOption.AllDirectories)
        |> Seq.filter (fun x -> x.Name.StartsWith "_" || x.Name = "bin" || x.Name = "obj")
        |> Seq.filter (fun n ->
             "packages"
             |> Path.GetFullPath
             |> n.FullName.StartsWith
             |> not)
        |> Seq.map (fun x -> x.FullName)
        |> Seq.distinct
        // arrange so leaves get deleted first, avoiding "does not exist" warnings
        |> Seq.groupBy (fun x ->
             x
             |> Seq.filter (fun c -> c = '\\' || c = '/')
             |> Seq.length)
        |> Seq.map (fun (n, x) -> (n, x |> Seq.sort))
        |> Seq.sortBy (fun p -> -1 * (fst p))
        |> Seq.map snd
        |> Seq.concat
        |> Seq.iter (fun n ->
             printfn "Deleting %s" n
             Directory.Delete(n, true))
        !!(@"./*Tests/*.tests.core.fsproj")
        |> Seq.map (fun f -> (Path.GetDirectoryName f) @@ "coverage.opencover.xml")
        |> Seq.iter File.Delete

        let temp = Environment.environVar "TEMP"
        if not <| String.IsNullOrWhiteSpace temp then
          Directory.GetFiles(temp, "*.tmp.dll.mdb") |> Seq.iter File.Delete
      with
      | :? System.IO.IOException as x -> Clean' (x :> Exception) depth
      | :? System.UnauthorizedAccessException as x -> Clean' (x :> Exception) depth

    and Clean' x depth =
      printfn "looping after %A" x
      System.Threading.Thread.Sleep(500)
      if depth < 10
      then Clean1(depth + 1)
      else Assert.Fail "Could not clean all the files"

    Clean1 0
