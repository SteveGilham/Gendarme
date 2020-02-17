namespace AvoidUnnecessarySpecialization

open System.Collections.Generic
open System.IO

module Main =
  let prepareTargetFiles (fromInfos : DirectoryInfo seq)
      (toInfos : DirectoryInfo seq) (sourceInfos : DirectoryInfo seq)
      (targets : string seq) =
    // Copy all the files into the target directory
    let mapping = Dictionary<string, string>()
    Seq.zip sourceInfos targets
    |> Seq.map (fun (x, y) ->
         let f = x.FullName // trim separator
         (Path.Combine(f |> Path.GetDirectoryName, f |> Path.GetFileName), y))
    |> Seq.iter mapping.Add

    Seq.zip fromInfos toInfos
    |> Seq.iter (fun (fromInfo, toInfo) ->
         let files = fromInfo.GetFiles()
         files
         |> Seq.iter (fun info ->
              let fullName = info.FullName
              let filename = info.Name
              let copy = Path.Combine(toInfo.FullName, filename)
              File.Copy(fullName, copy, true)))