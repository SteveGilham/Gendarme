# 2022.12.5.10565

* Adjust `DoNotAssumeIntPtrSizeRule` to allow for compiler changes at dotnet 7.0.  There will still be false negatives as casts involving `IntPtr` are now inlined, rather than going to `op_Explicit`

# 2022.5.17.8250

* Fix false positives for `DoNotThrowInNonCatchClausesRule` for `try/when/catch` cases that `throw` from the `catch`

# 2022.5.15.11273

* Fix some false negatives for `AvoidSwitchStatementsRule` (where the switch is on a `string` value)
* Fix false positives for `DelegatesPassedToNativeCodeMustIncludeExceptionHandlingRule` and `AvoidUnneededUnboxingRule` associated with changes in F# exception handling

# 2022.2.17.8350

* First stable release
* Make a heuristic fix for the changed IL that meant a test failure for `AvoidUnnecessarySpecializationRule`
  * code that was IL_0001: ldarga.s x/	IL_0003: constrained. !!T (stack unchanged) is now IL_0001: ldarg.1/IL_0002: box !!T which pops the loaded value and pushes the boxed copy, so treat ldarg+box as a unit.
* Make heuristic fixes for `CheckParametersNullityInVisibleMethods` -- also related to boxing generics; and cases of different choices of comparison operation
* Propagate the checks for the changed null comparison IL to `ProtectCallToEventDelegatesRule`
* Fix false negatives in `DelegatesPassedToNativeCodeMustIncludeExceptionHandlingRule`
* Fix false negative in `TestNativeFieldsArray` -- extended checks for new changed IL

# 2022.2.9.17153-pre-release 

* [NEW RULE] `AltCode.Rules.General.AvoidAssemblySemanticVersionMismatchRule` to insist that the API contract (major, minor, and optionally build if defined for the assembly) match, but the lesser facets, revision and possibly build are free.
* Revive Correctness rule `DeclareEventsExplicitlyRule`
* Update for modern C# dialect e.g. allow discard `_` as a variable name
* Fix `PreferIFormatProviderOverrideRule` and `PreferStringComparisonOverrideRule` to spot more than just the first instance of any given method with a preferred override
* In `AvoidMethodsWithSideEffectsInConditionalCodeRule`, consider `System.Array.Empty<T>()` to be pure.
* `AvoidUninstantiatedInternalClassesRule` checks if `internal` attribute types are used in the assembly, and counts those as instantiation
* Include changes from the upstream repo
  * Add more heuristics to `DoNotHardcodePathsRule`
  * Tune the severity and confidence levels of `AvoidUnnecessarySpecializationRule` and the "avoid duplicated code" rules according to context
  * Test a wider set of use cases in `DoNotUseLockedRegionOutsideMethodRule`

# 2022.1.25.12143-pre-release 

* Reinstate `Gendarme.Rules.Security.Cas.DoNotExposeFieldsInSecuredTypeRule`, mistakenly deleted
* New rules
  * `AltCode.Rules.General.PreferStrongNamedAssembliesRule` to replace deprecated/withdrawn FxCop rule Microsoft.Design#CA2210:AssembliesShouldHaveValidStrongNames
  * `AltCode.Rules.PowerShell.UseOnlyStandardVerbsRule` to replace "Microsoft.PowerShell#PS1001:UseOnlyStandardVerbs"
* [net472] Reinstate the obsolescing code access security rules as the assembly `Obsolete.Rules.Security.Cas.dll`; this covers rules
  * `AddMissingTypeInheritanceDemandRule`
  * `DoNotExposeMethodsProtectedByLinkDemandRule`
  * `DoNotReduceTypeSecurityOnMethodsRule`
  * `SecureGetObjectDataOverridesRule`

# 2022.1.23.20210-pre-release 

* New rule categories
  * `AltCode.Rules.General` for general purpose rules, starting with `JustifySuppressionRule` to check the `Justification` sproperty on `SuppressMessage` attribute
  * `AltCode.Rules.PowerShell` for re-implementing the old Microsoft PowerShell FxCop rules, starting with `DefineCmdletInTheCorrectNamespaceRule` to check the naming convention
* Fix the Gendarme.Rules.Gendarme.UseCorrectSuffixRule to ignore unutterable classes in namespaces starting with "`<StartupCode$`".
* Fix where the Gendarme.Rules.Globalization.SatelliteResourceMismatchRule reads bitmaps from resource assemblies, providing a dummy type as needed.

# 2022.1.20.8043-pre-release 

* Fix false positives in globalization rules `PreferIFormatProviderOverrideRule` and `PreferStringComparisonOverrideRule` in a net6.0-only environment

# 2022.1.19.15132-pre-release 

* Fix intermittent/environmentally dependent bugs in --
  * Globalization rules `PreferIFormatProviderOverrideRule` and `PreferStringComparisonOverrideRule`
  * Correctness rule `BadRecursiveInvocationRule` 

# 2022.1.17.12282-pre-release

* `net40` build removed; the Framework tool now uses shared `netstandard2.0` assemblies with a `net472` executable.  This also means that the stale code access security rules are now removed --
  * `AddMissingTypeInheritanceDemandRule`
  * `DoNotExposeMethodsProtectedByLinkDemandRule`
  * `DoNotReduceTypeSecurityOnMethodsRule`
  * `SecureGetObjectDataOverridesRule`
* In the text output, include a specimen global suppression attribute for each issue (F# syntax, ready to copy and paste; for other languages, tweak the `[<>]` part of the declaration).  This is for convenience when dealing with intractable issues e.g. arising from code generation
  * While `Scope` is not heeded by the Gendarme process, it's there to placate other consumers (which will ignore the foreign rule); the comment indicates the corresponding object type within the Gendarme analysis in case they should ever be out of line.
  * The syntax and punctuation of the `Target` with regards to nested types and special names is as Gendarme expects, which differs somewhat from FxCop in annoying details
```
Global Suppression Attribute:
[<assembly: SuppressMessage("Gendarme.Rules.Correctness",
                            "MethodCanBeMadeStaticRule",
                            Scope = "member", // MethodDefinition
                            Target = "ParameterNamesShouldMatch.Handler::ShowMessage(a,System.String)",
                            Justification = "")>]

```
* Fixes `DoNotLockOnThisOrTypesRule` for current C# compiler IL generation
* Reenable several rules omitted in previous builds
  * bad practice rules `AvoidNullCheckWithAsOperatorRule` and `DoNotDecreaseVisibilityRule`
  * design rule `DoNotDeclareSettersOnCollectionPropertiesRule` (excluding the `PermissionSet` exemption)
  * exception rule `DoNotThrowInNonCatchClausesRule`
  * globalization rules `PreferIFormatProviderOverrideRule` and `PreferStringComparisonOverrideRule`

# 2021.5.28.12260-pre-release

Issues found in use
* Don't apply DoNotDeclareVirtualMethodsInSealedTypeRule to F# closures in addition to existing type exemptions

# 2020.12.14.8435-pre-release

Fixes/updates
* net50 compatibility
* Fix possible type reference failure with F# code
* Enable `ProvideValidXmlStringRule`  and `ProvideValidXpathExpressionRule` for .net standard
* Fix `ProvideCorrectArgumentsToFormattingMethodsRule` and `ReviewLinqMethodRule` for Roslyn
* Avoid false positives in `UseIsOperatorRule` for Roslyn
* Because they use obsolescing functions not present in `netstandard2.0` the following `Gendarme.Rules.Security.Cas` rules are not implemented in the global tool version (so if this is relevant to you, use the .net Framework build):
  * `AddMissingTypeInheritanceDemandRule`
  * `DoNotExposeMethodsProtectedByLinkDemandRule`
  * `DoNotReduceTypeSecurityOnMethodsRule`
  * `SecureGetObjectDataOverridesRule`
* The obsolete `Gendarme.Rules.Portability.MonoCompatibilityReviewRule`has been removed.

# 2020.11.13.18423-pre-release

Issues found in use
* Don't apply ParameterNamesShouldMatchOverridenMethodRule to cases where the base method has a null or empty parameter name (e.g. F# interfaces)

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
* In the unhandled exception message, own this fork.
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
