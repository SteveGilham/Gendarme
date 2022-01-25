# 2022.x.x.xxxxx-pre-release 

* Reinstate `Gendarme.Rules.Security.Cas.DoNotExposeFieldsInSecuredTypeRule`, mistakenly deleted
* New rules
  * `AltCode.Rules.General.PreferStrongNamedAssembliesRule` to replace deprecated/withdrawn FxCop rule Microsoft.Design#CA2210
  * `AltCode.Rules.PowerShell.UseOnlyStandardVerbsRule` to replace "Microsoft.PowerShell#PS1001:UseOnlyStandardVerbs"
* [net472] Reinstate the obsolescing code access security rules as the assembly `Obsolete.Rules.Security.Cas.dll`; is covers rules
  * `AddMissingTypeInheritanceDemandRule`
  * `DoNotExposeMethodsProtectedByLinkDemandRule`
  * `DoNotReduceTypeSecurityOnMethodsRule`
  * `SecureGetObjectDataOverridesRule`

⁋For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/trunk/ReleaseNotes%20-%20Previously.md