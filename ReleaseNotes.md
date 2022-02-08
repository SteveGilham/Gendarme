# 2022.x.x.xxxxx-pre-release 

* Update for modern C# dialect e.g. allow discard `_` as a variable name
* Fix `PreferIFormatProviderOverrideRule` and `PreferStringComparisonOverrideRule` to spot more than just the first instance of any given method with a preferred override
* In `AvoidMethodsWithSideEffectsInConditionalCodeRule`, consider `System.Array.Empty<T>()` to be pure.
* [NEW RULE] `AltCode.Rules.General.AvoidAssemblySemanticVersionMismatchRule` to insist that the API contract (major, minor, and optionally build if defined for the assembly) match, but the lesser facets, revision and possibly build are free.
* `AvoidUninstantiatedInternalClassesRule` checks if `internal` attribute types are used in the assembly, and counts those as instantiation
* Include changes from the upstream repo
  * Add more heuristics to `DoNotHardcodePathsRule`
  * Tune the severity and confidence levels of `AvoidUnnecessarySpecializationRule` and the avoid duplicated code rules according to context

⁋For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/trunk/ReleaseNotes%20-%20Previously.md