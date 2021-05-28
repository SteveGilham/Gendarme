
# altcode.gendarme/altcode.gendarme-tool
A Mono.Gendarme fork, built against a recent Mono.Cecil version, one that can load assemblies built with current compilers.

## Features

* Can load .net core assemblies 
  * Will search the nuget cache for dependencies, though this can take some time as an alternative to using `dotnet publish` to get all the code you want to analyse in one place.
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

## Known Issues

Not all the classic Gendarme unit tests currently pass.  In the main, these failures are due to the Roslyn compiler producing different IL than the original C# compiler did.  A few failures are due to the API changes in `.netstandard` compared with the .net Framework. In production these will typically manifest as false negatives.

## Changes made for F# support
The F# compiler generates a large amount of code that does not conform to these rules, particularly with closures.  A full list for the most recent release is presented [here](https://github.com/SteveGilham/Gendarme/blob/release/pre-release/README.md#changes-made-for-f-support). 

## Badges
* [![Nuget](https://buildstats.info/nuget/altcode.gendarme?includePreReleases=true) Framework build command-line tool](https://www.nuget.org/packages/altcode.gendarme)
* [![Nuget](https://buildstats.info/nuget/altcode.gendarme-tool?includePreReleases=true) Global tool for .net core 2.1 and later](https://www.nuget.org/packages/altcode.gendarme-tool)

| | | |
| --- | --- | --- | 
| **Build** | AppVeyor [![Build status](https://img.shields.io/appveyor/ci/SteveGilham/Gendarme.svg)](https://ci.appveyor.com/project/SteveGilham/Gendarme) | ![Build history](https://buildstats.info/appveyor/chart/SteveGilham/Gendarme) 
| |GitHub [![CI](https://github.com/SteveGilham/Gendarme/workflows/CI/badge.svg)](https://github.com/SteveGilham/Gendarme/actions?query=workflow%3ACI) | [![Build history](https://buildstats.info/github/chart/SteveGilham/Gendarme?branch=trunk)](https://github.com/SteveGilham/Gendarme/actions?query=workflow%3ACI)

