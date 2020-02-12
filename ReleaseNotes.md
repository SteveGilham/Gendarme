# 2020.2.12.xxx-pre-release
F# niggles first pass
* Fix `AvoidMultidimensionalIndexerRule` for F# generated parameterless methods called `get_Item`
* Fix comparison of nested type names in parameters against supplied types
* Ignore [CompilerGenerated] methods for `AvoidSwitchStatementsRule` and `CheckParametersNullityInVisibleMethodsRule`
* Ignore [CompilerGenerated] fields and methods for `VariableNamesShouldNotMatchFieldNamesRule`
* Ignore `<StartupCode$` names in `UseCorrectCasingRule`
* Ignore generated types containg `@` in their names for `AvoidUnsealedUninheritedInternalTypesRule`
* Ignore the `Tags` generted type inside union types for `AvoidVisibleConstantFieldRule`

For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/master/ReleaseNotes%20-%20Previously.md