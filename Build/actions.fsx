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

open NUnit.Framework

module Actions =
    let Clean () =
        let rec clean1 depth =
            try
                (DirectoryInfo ".").GetDirectories("*", SearchOption.AllDirectories)
                |> Seq.filter (fun x -> x.Name.StartsWith "_" || x.Name = "bin" || x.Name = "obj")
                |> Seq.filter (fun n -> "packages" |> Path.GetFullPath |> n.FullName.StartsWith |> not)
                |> Seq.map (fun x -> x.FullName)
                |> Seq.distinct
                // arrange so leaves get deleted first, avoiding "does not exist" warnings
                |> Seq.groupBy (fun x -> x |> Seq.filter (fun c -> c = '\\' || c = '/') |> Seq.length)
                |> Seq.map (fun (n, x) -> (n, x |> Seq.sort))
                |> Seq.sortBy (fst >> ((*) -1))
                |> Seq.collect snd
                |> Seq.iter (fun n ->
                    printfn "Deleting %s" n
                    Directory.Delete(n, true))

                let temp = Environment.environVar "TEMP"

                if not <| String.IsNullOrWhiteSpace temp then
                    Directory.GetFiles(temp, "*.tmp.dll.mdb") |> Seq.iter File.Delete
            with
            | :? System.IO.IOException as x -> clean' (x :> Exception) depth
            | :? System.UnauthorizedAccessException as x -> clean' (x :> Exception) depth

        and clean' x depth =
            printfn "looping after %A" x
            System.Threading.Thread.Sleep(500)

            if depth < 10 then
                clean1 (depth + 1)
            else
                Assert.Fail "Could not clean all the files"

        clean1 0

let HandleResults (msg: string) (result: Fake.Core.ProcessResult) =
    String.Join(Environment.NewLine, result.Messages) |> printfn "%s"

    let save = (Console.ForegroundColor, Console.BackgroundColor)

    match result.Errors |> Seq.toList with
    | [] -> ()
    | errors ->
        try
            Console.ForegroundColor <- ConsoleColor.Black
            Console.BackgroundColor <- ConsoleColor.White

            String.Join(Environment.NewLine, errors) |> printfn "ERR : %s"
        finally
            Console.ForegroundColor <- fst save
            Console.BackgroundColor <- snd save

    Assert.That(result.ExitCode, Is.EqualTo 0, msg)

let AssertResult (msg: string) (result: Fake.Core.ProcessResult<'a>) =
    Assert.That(result.ExitCode, Is.EqualTo 0, msg)

let Run (file, dir, args) msg =
    CreateProcess.fromRawCommand file args
    |> CreateProcess.withWorkingDirectory dir
    |> CreateProcess.withFramework
    |> Proc.run
    |> (AssertResult msg)

let RunDotnet (o: DotNet.Options -> DotNet.Options) cmd args msg =
    DotNet.exec o cmd args |> (HandleResults msg)