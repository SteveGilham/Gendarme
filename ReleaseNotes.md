# Unreleased

# 2026.8.12.15560 

* Update build tools (net10.0) and dependencies.
* F# compiler also generates types with names like T_12Bytes@, which are `System.ValueType` subclasses that do not override Equals and GetHashCode, but are not user defined. So we exclude them from `OverrideValueTypeDefaultsRule`

# 2026.2.11.18114 

* Update build tools (net10.0) and dependencies.
* Make minimal fixes for System.ReadOnlySpan<> instances included by the compiler in string concatenation and string format checking.
* F# compiler generates types with names like T8_314Bytes@, which are `System.ValueType` subclasses that do not override Equals and GetHashCode, but are not user defined. So we exclude them from `OverrideValueTypeDefaultsRule`

⁋For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/trunk/ReleaseNotes%20-%20Previously.md