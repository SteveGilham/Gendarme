namespace AltCode.Rules.General

open System.Diagnostics.CodeAnalysis
open System.Resources
open System.Reflection

module Tools =
  let internal resources =
    ResourceManager("AltCode.Rules.General.Strings", Assembly.GetExecutingAssembly())

  let resource x = resources.GetString x

[<assembly: SuppressMessage("Microsoft.Performance",
                            "CA1810:InitializeReferenceTypeStaticFieldsInline",
                            Scope = "member",
                            Target = "<StartupCode$AltCode-Rules-General>.$Library.#.cctor()",
                            Justification = "Compiler generated type")>]
[<assembly: System.Resources.NeutralResourcesLanguageAttribute("en-GB")>]
()