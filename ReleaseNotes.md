# 2022.12.5.105xx

* Adjust `DoNotAssumeIntPtrSizeRule` to allow for compiler changes at dotnet 7.0.  There will still be false negatives as casts involving `IntPtr` are now inlined, rather than going to `op_Explicit`

⁋For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/trunk/ReleaseNotes%20-%20Previously.md