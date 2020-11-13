# 2020.7.21.12091-pre-release

Issues found in use
* Notice zero values in non-int32 enums
* Skip types called `<PrivateImplementationDetails>`

# 2020.6.12.11211-pre-release

Issues found in use
* Improve the assembly resolution to 
  * check more cases where type hierarchies are being explored
* In `PreferLiteralOverInitOnlyFieldsRule`. a field initialization may be by reference -- ensure it is resolved before treating it directly.

# 2020.3.9.15370-pre-release

F# niggles part 7
* Adapt `UseCorrectCasingRule` to be compatible with `FSharpLint` for F# code
* Add a `RelaxedMarkAllNonSerializableFieldsRule` which ignores F# types with `@` in the name, keeping the full-strength version for cases where serializing a closure is intentional.

# 2020.3.7.10241-pre-release

* Improve the assembly resolution to 
 * distinguish different versions of the same assembly
 * look in the nuget cache if the assembly isn't local (means that .net core code needn't be published, at the cost of the extra time taken to search the cache.)

# 2020.3.4.15094-pre-release

[BUGFIX] another fix needed for the tool, despite it working for the operational test.

# 2020.3.4.14134-pre-release

[BUGFIX] Fix up and operationally test the gendarme global tool as part of the build

# 2020.3.4.8524-pre-release
F# niggles part 6
* [BUGFIX] The extension method `IsEventCallback` would throw NRE for certain generic F# types
* For `AvoidLargeClassesRule`, ignore `FSharpFunc` and `FSharpTypeFunc` valued fields in generated types with `@` in their names; treat them as methods in the type instead.
* For `AvoidDeepNamespaceHierarchyRule`, ignore F# generated namespaces of the form `<StartupCode$a-b-c-d>.$.NETFramework,Version=...`
* For `AvoidRepetitiveCastsRule`, ignore F# `is` then `as` of anonymous temporaries (often happens in `match` expressions on sum types)
* Report the owning assembly of the defect target, always (disambiguates when a source file is shared bewteen projects in the same build).
* In the inhandled exception message, own this fork.
* Building on VS2019 for .net4.0 and .netcore2.1+
* Now available as a dotnet global tool in package `altcode.gendarme-tool`

# 2020.2.20.18134-pre-release
F# niggles part 5
* Exempt generated types with "@" in the name from `AvoidMethodWithUnusedGenericTypeRule`
* Exempt the field accessors of F# types (usually bypassed by the compiler by code that goes direct to the backing field) from `AvoidUncalledPrivateCodeRule`
* Allow for F# extension properties (`TypeName.get_...` and `TypeName.set_...`) and for generated parameter names of the form `_arg...` in `AvoidNonAlphanumericIdentifierRule`
* Consider F# extension methods/properties to be object-bound rather than module bound for `UseCorrectCasingRule`
* Add a heuristic to recognise F# compiler generated disposal after a `use` in `EnsureLocalDisposalRule`
* Add a `RelaxedAvoidCodeDuplicatedInSameClassRule`, which looks for patterns aligning with visible sequence points, and excludes patterns containing `throw new ArgumentNullException(...)`

# 2020.2.17.17051-pre-release
F# niggles part 4
* Exempt generated types with `@` in their names from `AvoidUnnecessarySpecializationRule`, `AvoidSpeculativeGeneralityRule` and `MethodCanBeMadeStaticRule`
* Exempt F# placeholder arguments `_` (compiled to `_arg...`) from `UseCorrectCasingRule`
* Exempt module-bound functions from `ConsiderConvertingMethodToPropertyRule`
* Exempt fields and constructors of records from `RemoveDependenceOnObsoleteCodeRule`; accessors will still be caught but can be `[SuppressMessage]`d as needed
* Take account of F#'s habit of making a virtual call to the base type constructor in object types constructors.
* Exempt F# code in modules, or where a `match` could equally be an `if` from `AvoidSwitchStatementsRule`, `match` being idiomatic and occasionally just happening to be on an explicit integral type
* Exempt property backing fields for code like `member val LocalSource = false with get, set` from `AvoidUnneededFieldInitializationRule`
* Exempt union cases (unsealed but not likely to be inherited) from `AvoidUnsealedUninheritedInternalTypeRule`

# 2020.2.16.16042-pre-release
F# niggles part 3
* For the purposes of generic parameter names, having the F# [CompilationMapping] attribute on the type is taken to detect F# code
* Even if a union type is [Obsolete] don't bother telling us its cases and case constructors depend on it.
* In F# code (assumes FSharp.Core referenced) then allow single lower-case letter generic types
* Exempt methods of F# generated types with `@` in the name from ` ParameterNamesShouldMatchOverriddenMethodRule`
* Exempt match on union types from `AvoidSwitchStatementsRule`
* Exempt F# generated types with `@` in the name from `UseCorrectPrefixRule`,`VariableNamesShouldNotMatchFieldNamesRule` and `UseCorrectCasingRule`
* Exempt generated abstract closure types from `AbstractTypesShouldNotHavePublicConstructorsRule`
* Exempt constructors of record types, or generated types with `@` in the name, from `AvoidLongParameterListsRule`
* Module-bound functions should be camel-cased in the `UseCorrectCasingRule`

# 2020.2.15.13034-pre-release
F# niggles part 2
* Explicitly exempt the `get_` and `set_` prefixes of getter and setter methods from the `AvoidNonAlphanumericIdentifierRule` since that was not already a thing.
* Make `AvoidUnneededUnboxingRule` not applicable to `[CompilerGenerated]` functions (e.g. Union case `CompareTo`)
* Exempt debugger-related generated types related to union types from `AvoidUnsealedUninheritedInternalTypeRule` and `UseCorrectCasingRule`
* Exempt getter and setter methods from `ConsiderConvertingMethodToPropertyRule` (well, duh!)
* Exempt types with only fully `[CompilerGenerated]` `Equals` and `CompareTo` methods from `ImplementIComparableCorrectlyRule`; also explicitly exempt record types

# 2020.2.12.16271-pre-release
F# niggles first pass
* Fix `AvoidMultidimensionalIndexerRule` for F# generated parameterless methods called `get_Item`
* Fix comparison of nested type names in parameters against supplied types
* Ignore [CompilerGenerated] methods for `AvoidSwitchStatementsRule` and `CheckParametersNullityInVisibleMethodsRule`
* Ignore [CompilerGenerated] fields and methods for `VariableNamesShouldNotMatchFieldNamesRule`
* Ignore `<StartupCode$` names in `UseCorrectCasingRule`
* Ignore generated types containg `@` in their names for `AvoidUnsealedUninheritedInternalTypesRule`
* Ignore the `Tags` generated type inside union types for `AvoidVisibleConstantFieldRule`

# 2020.2.11.15383-pre-release
* Replace the symbol reader extension with the more modern and general one from AltCover
* Static-link the FSharp.Core library into the symbol reader extension.

# 2020.2.10.17202-pre-release
* Build the code as of commit b45416d516b18e77c1e73be31649bb298b1c6652 (24-Oct-2016) against Cecil 0.11.1 and NUnit 2.7.1 using VS2013 (pre-Roslyn compiler)
* Unit test failures for rules
  * Gendarme.Rules.Performance\Test\AvoidUnnecessaryOverridesTest
  * Gendarme.Rules.Design\Test\ProvideTryParseAlternativeTest
  * Gendarme.Rules.Serialization\Test\CallBaseMethodsOnISerializableTypesTest
  * Gendarme.Rules.Correctness\Test\ProvideCorrectRegexPatternTest
  * Gendarme.Rules.Concurrency\Test\ProtectCallToEventDelegatesTest
  * Gendarme.Rules.Exceptions\Test\UseObjectDisposedExceptionTest
* Unit test failure for Gendarme.Framework.Helpers\StackEntryAnalysisTest
* Do not rely on the failed rules.
* Should work against .netstandard/.netcore binaries (you will need to `dotnet publish` to put any relevant dependencies in the same folder for Gendarme to inspect for inheritance -- especially so for F# code)
