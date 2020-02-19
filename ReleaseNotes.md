# 2020.2.xx.xxxxx-pre-release
F# niggles part 5
* Exempt generated types with "@" in the name from `AvoidMethodWithUnusedGenericTypeRule`
* Exempt the field accessors of records (usually bypassed by the compiler that goes direct to the backing field) from `AvoidUncalledPrivateCodeRule`
* Allow for F# extension properties (`TypeName.get_...` and `TypeName.set_...`) and for generated parameter names of the form `_arg...` in `AvoidNonAlphanumericIdentifierRule`
* Consider F# extension methods/properties to be object-bound rather than module bound for `UseCorrectCasingRule`



For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/master/ReleaseNotes%20-%20Previously.md
Since this is a release from a fork, [issues should be reported at my related repo](https://github.com/SteveGilham/altcode.fake/issues) that contains a Fake driver for the Gendarme tool, but noted as being against the forked Gendarme tool itself.