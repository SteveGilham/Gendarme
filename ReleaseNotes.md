# 2020.3.9.15xx-pre-release

F# niggles part 7
* Adapt `UseCorrectCasingRule` to be compatible with `FSharpLint` for F# code
* Add a `RelaxedMarkAllNonSerializableFieldsRule` which ignores F# types with `@` in the name, keeping the full-strength version for cases where serializing a closure is intentional.

For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/master/ReleaseNotes%20-%20Previously.md
Since this is a release from a fork, [issues should be reported at my related repo](https://github.com/SteveGilham/altcode.fake/issues) that contains a Fake driver for the Gendarme tool, but noted as being against the forked Gendarme tool itself.