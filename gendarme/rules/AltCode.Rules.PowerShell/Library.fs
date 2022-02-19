namespace AltCode.Rules.PowerShell

open System.Diagnostics.CodeAnalysis

open Mono.Cecil
open Gendarme.Framework
open Gendarme.Framework.Engines
open Gendarme.Framework.Helpers
open Gendarme.Framework.Rocks
open System.Resources
open System.Reflection

module Tools =

  let internal resources =
    ResourceManager("AltCode.Rules.PowerShell.Strings", Assembly.GetExecutingAssembly())

  let resource x = resources.GetString x

  let cmdlet =
    TypeName(Namespace = "System.Management.Automation", Name = "Cmdlet")

  let cmdletAttribute =
    TypeName(Namespace = "System.Management.Automation", Name = "CmdletAttribute")

  let standardVerbs =
    [| "Add"
       "Clear"
       "Close"
       "Copy"
       "Enter"
       "Exit"
       "Find"
       "Format"
       "Get"
       "Hide"
       "Join"
       "Lock"
       "Move"
       "New"
       "Open"
       "Pop"
       "Push"
       "Redo"
       "Remove"
       "Rename"
       "Reset"
       "Search"
       "Select"
       "Set"
       "Show"
       "Skip"
       "Split"
       "Step"
       "Switch"
       "Undo"
       "Unlock"
       "Watch"
       "Backup"
       "Checkpoint"
       "Compare"
       "Compress"
       "Convert"
       "ConvertFrom"
       "ConvertTo"
       "Dismount"
       "Edit"
       "Expand"
       "Export"
       "Group"
       "Import"
       "Initialize"
       "Limit"
       "Merge"
       "Mount"
       "Out"
       "Publish"
       "Restore"
       "Save"
       "Sync"
       "Unpublish"
       "Update"
       "Approve"
       "Assert"
       "Complete"
       "Confirm"
       "Deny"
       "Disable"
       "Enable"
       "Install"
       "Invoke"
       "Register"
       "Request"
       "Restart"
       "Resume"
       "Start"
       "Stop"
       "Submit"
       "Suspend"
       "Uninstall"
       "Unregister"
       "Wait"
       "Debug"
       "Measure"
       "Ping"
       "Repair"
       "Resolve"
       "Test"
       "Trace"
       "Connect"
       "Disconnect"
       "Read"
       "Receive"
       "Send"
       "Write"
       "Block"
       "Grant"
       "Protect"
       "Revoke"
       "Unblock"
       "Unprotect"
       "Use" |]
    |> Array.toList

  let IsCmdlet (td: TypeDefinition) =
    Some td
    |> Option.filter (fun t -> t.IsClass)
    |> Option.filter (fun t -> t.IsPublic)
    |> Option.filter (fun t -> t.Inherits cmdlet)
    |> Option.filter (fun t ->
      t.CustomAttributes
      |> Seq.exists (fun a -> a.AttributeType.Inherits cmdletAttribute))
    |> Option.isSome

[<assembly: SuppressMessage("Microsoft.Performance",
                            "CA1810:InitializeReferenceTypeStaticFieldsInline",
                            Scope = "member",
                            Target = "<StartupCode$AltCode-Rules-PowerShell>.$Library.#.cctor()",
                            Justification = "Compiler generated type")>]
[<assembly: System.Resources.NeutralResourcesLanguageAttribute("en-GB")>]
()