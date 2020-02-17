# 2020.2.xx.xxxxx-pre-release
F# niggles part 4
* Exempt generated types with `@` in their names from `AvoidSpeculativeGeneralityRule` and `MethodCanBeMadeStaticRule`
* Exempt F# placeholder arguments `_` (compiled to `_arg...`) from `UseCorrectCasingRule`
* Exempt module-bound functions from `ConsiderConvertingMethodToPropertyRule`
* Exempt fields and constructors of records from `RemoveDependenceOnObsoleteCodeRule`; accessors will still be caught but can be `[SuppressMessage]`d as needed
* Take account of F#'s habit of making a virtual call to the base type constructor in object types constructors.
* Exempt F# code in modules, or where a `match` could equally be an `if` from `AvoidSwitchStatementsRule`, `match` being idiomatic and occasionally just happening to be on an explicit integral type



For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/master/ReleaseNotes%20-%20Previously.md
Since this is a release from a fork, [issues should be reported at my related repo](https://github.com/SteveGilham/altcode.fake/issues) that contains a Fake driver for the Gendarme tool, but noted as being against the forked Gendarme tool itself.