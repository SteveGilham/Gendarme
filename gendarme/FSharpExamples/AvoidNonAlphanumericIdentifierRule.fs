namespace AvoidNonAlphanumericIdentifier

open System
open System.IO

module Primitive =
    type CollectParams =
      { RecorderDirectory : String }
      static member Create() =
        { RecorderDirectory = String.Empty }
    type PrepareParams =
      { InputDirectories : String seq }
      static member Create() =
        { InputDirectories = Seq.empty }

module TypeSafe =
  type DirectoryPath =
  | DirectoryPath of String
  | DInfo of DirectoryInfo
  | NoDirectory

    type DirectoryPaths =
      | DirectoryPaths of DirectoryPath seq
      | NoDirectories

    type CollectParams =
      { RecorderDirectory : DirectoryPath }
      static member Create() =
        { RecorderDirectory = NoDirectory }

    type PrepareParams =
      { InputDirectories : DirectoryPaths }
      static member Create() =
        { InputDirectories = NoDirectories }


type CollectParams =
  | Primitive of Primitive.CollectParams
  | TypeSafe of TypeSafe.CollectParams

type PrepareParams =
  | Primitive of Primitive.PrepareParams
  | TypeSafe of TypeSafe.PrepareParams

[<NoComparison>]
type ArgType =
  | Collect of CollectParams
  | Prepare of PrepareParams
  | ImportModule
  | GetVersion

module Augment =
  let (|Right|Left|) =
    function
    | Choice1Of2 x -> Choice1Of2 x
    | Choice2Of2 x -> Choice2Of2 x

  type System.Object with
    member self.IsNotNull with get() = self <> null