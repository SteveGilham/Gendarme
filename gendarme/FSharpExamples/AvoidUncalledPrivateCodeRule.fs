namespace AvoidUncalledPrivateCode

open System.Collections.Generic

type internal PointVisit =
  { mutable Count : int64
    [<System.Diagnostics.CodeAnalysis.SuppressMessage("Gendarme.Rules.Performance",
                                                      "AvoidUncalledPrivateCodeRule",
                                                      Justification =
                                                        "Not part of the example")>]
    Tracks : List<int> }

  static member Create() =
    { Count = 0L
      Tracks = List<int>() }

  static member Init n l =
    let tmp = { PointVisit.Create() with Count = n }
    tmp.Tracks.AddRange l
    tmp

module Instance =
  [<System.Diagnostics.CodeAnalysis.SuppressMessage("Gendarme.Rules.Performance",
                                                    "AvoidUncalledPrivateCodeRule",
                                                    Justification =
                                                      "Not part of the example")>]
  let mutable internal visits = new Dictionary<string, Dictionary<int, PointVisit>>()

module Adapter =
  let visitsAdd name line number =
    let v = PointVisit.Init number []
    Instance.visits.[name].Add(line, v)

  let visitCount key key2 = (Instance.visits.[key].[key2]).Count