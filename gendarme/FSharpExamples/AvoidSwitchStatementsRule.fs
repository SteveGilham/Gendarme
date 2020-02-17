namespace AvoidSwitchStatements

open System

type ReportFormat =
  | NCover = 0
  | OpenCover = 1
  | OpenCoverWithTracking = 2

module Counter =
  let updateReport format =
      match format with
      | ReportFormat.OpenCoverWithTracking | ReportFormat.OpenCover ->
        "uspid"
      | _ -> String.Empty