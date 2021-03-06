
# altcode.gendarme
A Mono.Gendarme fork, built against a recent Mono.Cecil version, one that can load assemblies built with current compilers.

### Badges
* [![Nuget](https://buildstats.info/nuget/altcode.gendarme?includePreReleases=true) Framework build command-line tool](https://www.nuget.org/packages/altcode.gendarme)
* [![Nuget](https://buildstats.info/nuget/altcode.gendarme-tool?includePreReleases=true) Global tool for .net core 2.1 and later](https://www.nuget.org/packages/altcode.gendarme-tool)

| | | |
| --- | --- | --- | 
| **Build** | <sup>AppVeyor</sup> [![Build status](https://img.shields.io/appveyor/ci/SteveGilham/Gendarme.svg)](https://ci.appveyor.com/project/SteveGilham/Gendarme) | ![Build history](https://buildstats.info/appveyor/chart/SteveGilham/Gendarme) 
| |<sup>GitHub</sup> [![CI](https://github.com/SteveGilham/Gendarme/workflows/CI/badge.svg)](https://github.com/SteveGilham/Gendarme/actions?query=workflow%3ACI) | [![Build history](https://buildstats.info/github/chart/SteveGilham/Gendarme?branch=trunk)](https://github.com/SteveGilham/Gendarme/actions?query=workflow%3ACI)


## Build process from trunk as per the CI YAML

Assumes net50 build environment

* `dotnet tool restore`
* `dotnet fake run .\Build\setup.fsx`
* `dotnet fake run .\Build\build.fsx`

The `build` stage can be done in Visual Studio with the Debug configuration to run the unit tests

## Features
See [the head of the (pre-)release branch](https://github.com/SteveGilham/Gendarme/blob/release/pre-release/README.md) for the features in the latest actual release.

In this branch

* Can load .net core assemblies 
  * Will search the nuget cache for dependencies, though this can take some timeas an alternative to using `dotnet publish` to get all the code you want to analyse in one place.
* Will load debug information from embedded symbols or actual `.pdb` files if available even on non-Windows platforms.
  *  The main impact is that the `AvoidLongMethodsRule` works by LoC and not IL against .net core code on all platforms.
* Because they use obsolescing functions not present in `netstandard2.0` the following `Gendarme.Rules.Security.Cas` rules are not implemented in the global tool version (so if this is relevant to you, use the .net Framework build):
  * `AddMissingTypeInheritanceDemandRule`
  * `DoNotExposeMethodsProtectedByLinkDemandRule`
  * `DoNotReduceTypeSecurityOnMethodsRule`
  * `SecureGetObjectDataOverridesRule`
* The obsolete `Gendarme.Rules.Portability.MonoCompatibilityReviewRule` is not implemented in this fork.
* `DefineAZeroValueRule` does not trigger for non-int32 enums that have a suitably typed zero value.  This rule should not also be doing the job of `EnumsShouldUseInt32Rule`
* Due to IL changes `UseIsOperatorRule` has been tuned to avoid false positives at the cost of missing some failure cases

## Direction
After having achieved the first objective, of being able to analyze code from the new .net, the next goal of this fork has been to make the tool more F# aware, because that's where I personally use it the most.  There are several places where F# code generation emits patterns that are detected by legacy Gendarme as erroneous, but which are not under sufficiently fine control by the developer or cannot be annotated to suppress a warning.

## Known Issues


#### Unit test fixing

Having resolved many issues stemming from a Cecil change to what the name and namespace properties of a nested type returned, the next major sources of test failure have been compiler changes (from pre-Roslyn to now) and differences in behaviour under `.netstandard` compared with the .net Framework.  In particular, the `AvoidSwitchStatements` rule needs some serious decompiler code to recognise Roslyn's mangled switch constructs (compiled as multiple conditional branches) so some tests have just been set to `[Ignore]`

The following rule suites have unit test failures

* Framework -- 2 failures for Stack entry analysis (Roslyn, most likely)
  * TestMultipleCatch()
  * TestTryCatchFinally()
* Concurrency -- 6 failures
  * Do not lock on Static Type/This/Type (false negatives)
  * `ProtectCallToEventDelegatesRule` (3 * false positives)
* Correctness -- 5 failures (false negatives)
  * `ProvideCorrectArgumentsToFormattingMethods` * 3 -- changed IL : `call Array.Empty` used instead of an explict load
  * `TestNativeFieldsArray` -- changed IL
  * `CheckParametersNullityInVisibleMethods` -- not sure what's up here
* Globalization -- 1 failure (Cannot read satellite resources with available reader)
* Interoperability -- 17 failures (false negatives)
  * 17 false negatives in `DelegatesPassedToNativeCodeMustIncludeExceptionHandling` due to anonymous delegates -- presumably an IL change
* Maintainability -- 1 failure (false negative in `AvoidUnnecessarySpecializationRule` possibly Stack entry analysis)
* Smells -- 2 failure
  * false positive in `SuccessOnNonDuplicatedCodeIntoForeachLoopTest`
  * false positive in `SuccesOnNonDuplicatedInSwitchsLoadingByFieldsTest`
  * 2 other `[Ignore]`d switch related tests

## Changes made for F# support
For the moment this seems to suffice to tame unreasonable, or unfixable generated, issues --

* Fix `AvoidMultidimensionalIndexerRule` for F# generated parameterless methods called `get_Item`
* Fix comparison of nested type names in parameters against supplied types
* Ignore [CompilerGenerated] methods for `AvoidSwitchStatementsRule` and `CheckParametersNullityInVisibleMethodsRule`
* Ignore [CompilerGenerated] fields and methods for `VariableNamesShouldNotMatchFieldNamesRule`
* Ignore `<StartupCode$` names in `UseCorrectCasingRule`
* Ignore generated types containg `@` in their names for `AvoidUnsealedUninheritedInternalTypesRule`
* Ignore the `Tags` generated type inside union types for `AvoidVisibleConstantFieldRule`
* Explicitly exempt the `get_` and `set_` prefixes of getter and setter methods from the `AvoidNonAlphanumericIdentifierRule` since that was not already a thing.
* Make `AvoidUnneededUnboxingRule` not applicable to `[CompilerGenerated]` functions (e.g. Union case `CompareTo`)
* Exempt debugger-related generated types related to union types from `AvoidUnsealedUninheritedInternalTypeRule` and `UseCorrectCasingRule`
* Exempt getter and setter methods from `ConsiderConvertingMethodToPropertyRule` (well, duh!)
* Exempt types with only fully `[CompilerGenerated]` `Equals` and `CompareTo` methods from `ImplementIComparableCorrectlyRule`; also explicitly exempt record types
* Even if a union type is `[Obsolete]` don't bother telling us its cases and case constructors depend on it.
* In F# code (type with F# `[CompilationMapping]` attribute) then allow single lower-case letter generic types
* Exempt methods of F# generated types with `@` in the name from ` ParameterNamesShouldMatchOverriddenMethodRule`
* Exempt match on union types from `AvoidSwitchStatementsRule`
* Exempt F# generated types with `@` in the name from `UseCorrectPrefixRule`,`VariableNamesShouldNotMatchFieldNamesRule` and `UseCorrectCasingRule`
* Exempt generated abstract closure types from `AbstractTypesShouldNotHavePublicConstructorsRule`
* Exempt constructors of record types, or generated types with `@` in the name, from `AvoidLongParameterListsRule`
* Exempt generated types with `@` in their names from `AvoidUnnecessarySpecializationRule`, `AvoidSpeculativeGeneralityRule` and `MethodCanBeMadeStaticRule`
* Exempt F# placeholder arguments `_` (compiled to `_arg...`) from `UseCorrectCasingRule`
* Exempt module-bound functions from `ConsiderConvertingMethodToPropertyRule`
* Exempt fields and constructors of records from `RemoveDependenceOnObsoleteCodeRule`; accessors will still be caught but can be `[SuppressMessage]`d as needed
* Take account of F#'s habit of making a virtual call to the base type constructor in object types constructors.
* Exempt F# code in modules, or where a `match` could equally be an `if` from `AvoidSwitchStatementsRule`, `match` being idiomatic and occasionally just happening to be on an explicit integral type
* Exempt property backing fields for code like `member val LocalSource = false with get, set` from `AvoidUnneededFieldInitializationRule`
* Exempt union cases (unsealed but not likely to be inherited) from `AvoidUnsealedUninheritedInternalTypeRule`
* Exempt generated types with "@" in the name from `AvoidMethodWithUnusedGenericTypeRule`
* Exempt the field accessors of F# types (usually bypassed by the compiler by code that goes direct to the backing field) from `AvoidUncalledPrivateCodeRule`
* Allow for F# extension properties (`TypeName.get_...` and `TypeName.set_...`) and for generated parameter names of the form `_arg...` in `AvoidNonAlphanumericIdentifierRule`
* Consider F# extension methods/properties to be object-bound rather than module bound for `UseCorrectCasingRule`
* Add a heuristic to recognise F# compiler generated disposal after a `use` in `EnsureLocalDisposalRule`
* Add a `RelaxedAvoidCodeDuplicatedInSameClassRule`, which looks for patterns aligning with visible sequence points, and excludes patterns containing `throw new ArgumentNullException(...)`
* For `AvoidLargeClassesRule`, ignore `FSharpFunc` and `FSharpTypeFunc` valued fields in generated types with `@` in their names; treat them as methods in the type instead.
* For `AvoidDeepNamespaceHierarchyRule`, ignore F# generated namespaces of the form `<StartupCode$a-b-c-d>.$.NETFramework,Version=...`
* For `AvoidRepetitiveCastsRule`, ignore F# `is` then `as` of anonymous temporaries (often happens in `match` expressions on sum types)
* Adapt `UseCorrectCasingRule` to be compatible with `FSharpLint` for F# code (Non-class, non-public, functions should be camel-cased)
* Add a `RelaxedMarkAllNonSerializableFieldsRule` which ignores F# types with `@` in the name, keeping the full-strength version for cases where serializing a closure is intentional.
* Skip types called `<PrivateImplementationDetails>`
* Don't apply `ParameterNamesShouldMatchOverridenMethodRule` to cases where the base method has a null or empty parameter name (e.g. F# interfaces)
