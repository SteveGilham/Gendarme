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