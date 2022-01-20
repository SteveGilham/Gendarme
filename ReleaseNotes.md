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

⁋For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/trunk/ReleaseNotes%20-%20Previously.md