# 2022.x.xx.xxxxx-pre-release 

* New rule categories
  * `AltCode.Rules.General` for general purpose rules, starting with `JustifySuppressionRule` to check the `Justification` sproperty on `SuppressMessage` attribute
  * `AltCode.Rules.PowerShell` for re-implementing the old Microsoft PowerShell FxCop rules, starting with `DefineCmdletInTheCorrectNamespaceRule`to check the naming convention
* Fix the Gendarme.Rules.Gendarme.UseCorrectSuffixRule to ignore unutterable classes in namespaces starting with "`<StartupCode$`".
* Fix where the Gendarme.Rules.Globalization reads bitmaps from resource assemblies, providing a dummy type as needed.

⁋For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/trunk/ReleaseNotes%20-%20Previously.md