# 2022.x.x.xxxxx-pre-release 

* Update for modern C# dialect e.g. allow discard `_` as a variable name
* Fix `PreferIFormatProviderOverrideRule` and `PreferStringComparisonOverrideRule` to spot more than one violation of any given method with a preferred override
* In `AvoidMethodsWithSideEffectsInConditionalCodeRule`, consider `System.Array.Empty<T>()` to be pure.



⁋For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/trunk/ReleaseNotes%20-%20Previously.md