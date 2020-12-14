# 2020.12.14.8xxx-pre-release

Fixes/updates
* net50 compatibility
* Fix possible type reference failure with F# code
* Enable `ProvideValidXmlStringRule`  and `ProvideValidXpathExpressionRule` for .net standard
* Fix `ProvideCorrectArgumentsToFormattingMethodsRule` and `ReviewLinqMethodRule` for Roslyn
* Avoid falsse positives in `UseIsOperatorRule` for Roslyn
* Because they use obsolescing functions not present in `netstandard2.0` the following `Gendarme.Rules.Security.Cas` rules are not implemented in the global tool version (so if this is relevant to you, use the .net Framework build):
  * `AddMissingTypeInheritanceDemandRule`
  * `DoNotExposeMethodsProtectedByLinkDemandRule`
  * `DoNotReduceTypeSecurityOnMethodsRule`
  * `SecureGetObjectDataOverridesRule`
* The obsolete `Gendarme.Rules.Portability.MonoCompatibilityReviewRule`has been removed.

For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/master/ReleaseNotes%20-%20Previously.md
Since this is a release from a fork, [issues should be reported at my related repo](https://github.com/SteveGilham/altcode.fake/issues) that contains a Fake driver for the Gendarme tool, but noted as being against the forked Gendarme tool itself.