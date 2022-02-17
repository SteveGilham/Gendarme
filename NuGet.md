
# altcode.gendarme/altcode.gendarme-tool
A Mono.Gendarme fork, built against a recent Mono.Cecil version, one that can load assemblies built with current compilers.

## Features

* Can load .net core assemblies 
  * Will search the nuget cache for dependencies, though this can take some time as an alternative to using `dotnet publish` to get all the code you want to analyse in one place.
  * Updated for modern C# dialect e.g. allow discard `_` as a variable name
  * Bug fixes and updates for current compiler generated IL
* Will load debug information from embedded symbols or actual `.pdb` files if available even on non-Windows platforms.
  *  The main impact is that the `AvoidLongMethodsRule` works by LoC and not IL against .net core code on all platforms.
* Depending whether the Framework or dotnet tool version is used, the results may differ when faced with the same assembly, because of the different runtime being consulted
  * e.g. several types marked `[Serializable]` in the Framework are not so marked at `dotnet`, so serialization rules will give different answers
* Because they use obsolescing functions not present in `netstandard2.0` the following `Gendarme.Rules.Security.Cas` rules are only present in the Framework tool build, under the `Obsolete.Rules.Security.Cas` name:
  * `AddMissingTypeInheritanceDemandRule`
  * `DoNotExposeMethodsProtectedByLinkDemandRule`
  * `DoNotReduceTypeSecurityOnMethodsRule`
  * `SecureGetObjectDataOverridesRule`
* The obsolete `Gendarme.Rules.Portability.MonoCompatibilityReviewRule` is not implemented in this fork.
* `DefineAZeroValueRule` does not trigger for non-int32 enums that have a suitably typed zero value.  This rule should not also be doing the job of `EnumsShouldUseInt32Rule`
* Due to IL changes `UseIsOperatorRule` has been tuned to avoid false positives at the cost of missing some failure cases
* New rules/categories
  * `AltCode.Rules.General.AvoidAssemblySemanticVersionMismatchRule` to insist that the API contract (major, minor, and optionally build if defined for the assembly) match, but the lesser facets, revision and possibly build are free.
  * `AltCode.Rules.General.JustifySuppressionRule` to check the `Justification` property on `SuppressMessage` attribute
  * `AltCode.Rules.General.PreferStrongNamedAssembliesRule` to replace deprecated/withdrawn FxCop rule Microsoft.Design#CA2210
  * `AltCode.Rules.PowerShell.UseOnlyStandardVerbsRule` to replace "Microsoft.PowerShell#PS1001:UseOnlyStandardVerbs"
  * `AltCode.Rules.PowerShell.DefineCmdletInTheCorrectNamespaceRule` to replace "Microsoft.PowerShell#PS1011:DefineCmdletInTheCorrectNamespace"
  * `Gendarme.Rules.Serialization.RelaxedMarkAllNonSerializableFieldsRule` to ignore F# compiler generated closures
  * `Gendarme.Rules.Smells.RelaxedAvoidCodeDuplicatedInSameClassRule` to ignore some trivial cases e.g. argument null checks
* Reinstated rules
  * `Gendarme.Rules.BadPractice.AvoidNullCheckWithAsOperatorRule`
  * `Gendarme.Rules.BadPractice.DoNotDecreaseVisibilityRule`
  * `Gendarme.Rules.Correctness.DeclareEventsExplicitlyRule`
  * `Gendarme.Rules.Design.DoNotDeclareSettersOnCollectionPropertiesRule` (excluding the `PermissionSet` exemption)
  * `Gendarme.Rules.Exceptions.DoNotThrowInNonCatchClausesRule`
  * `Gendarme.Rules.Globalization.PreferIFormatProviderOverrideRule`
  * `Gendarme.Rules.Globalization.PreferStringComparisonOverrideRule`
* In the text output, include a specimen global suppression attribute for each issue, for convenience when dealing with remaining intractable issues e.g. arising from code generation
  * While `Scope` is not heeded by the Gendarme process, it's there to placate other consumers (which will ignore the foreign rule); the comment indicates the corresponding object type within the Gendarme analysis in case they should ever be out of line.
  * The syntax and punctuation of the `Target` with regards to nested types and special names is as Gendarme expects, which differs somewhat from FxCop in annoying details
  * The emitted section looks like this:
```
Global Suppression Attribute:
[<assembly: SuppressMessage("Gendarme.Rules.Correctness",
                            "MethodCanBeMadeStaticRule",
                            Scope = "member", // MethodDefinition
                            Target = "ParameterNamesShouldMatch.Handler::ShowMessage(a,System.String)",
                            Justification = "")>]

```

## Known Issues

Not all the classic Gendarme unit tests currently pass.  In the main, these failures are due to the Roslyn compiler producing different IL than the original C# compiler did.  A few failures are due to the API changes in `.netstandard` compared with the .net Framework. In production these will typically manifest as false negatives.

## Changes made for F# support
The F# compiler generates a large amount of code that does not conform to these rules, particularly with closures.  A full list for the most recent release is presented [here](https://github.com/SteveGilham/Gendarme/blob/release/stable/README.md#changes-made-for-f-support). 

## Badges
* [![Nuget](https://buildstats.info/nuget/altcode.gendarme?includePreReleases=true) Framework build command-line tool](https://www.nuget.org/packages/altcode.gendarme)
* [![Nuget](https://buildstats.info/nuget/altcode.gendarme-tool?includePreReleases=true) Global tool for .net core 2.1 and later](https://www.nuget.org/packages/altcode.gendarme-tool)

| | | |
| --- | --- | --- | 
| **Build** | AppVeyor [![Build status](https://img.shields.io/appveyor/ci/SteveGilham/Gendarme.svg)](https://ci.appveyor.com/project/SteveGilham/Gendarme) | ![Build history](https://buildstats.info/appveyor/chart/SteveGilham/Gendarme) 
| |GitHub [![CI](https://github.com/SteveGilham/Gendarme/workflows/CI/badge.svg)](https://github.com/SteveGilham/Gendarme/actions?query=workflow%3ACI) | [![Build history](https://buildstats.info/github/chart/SteveGilham/Gendarme?branch=trunk)](https://github.com/SteveGilham/Gendarme/actions?query=workflow%3ACI)

