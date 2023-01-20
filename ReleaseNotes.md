# 2023.1.x.x

* Exempt `Task` and `Task<'T>` from `EnsureLocalDisposalRule` as they generally should not be disposed manually.
* Remove the "no arguments means no error" hack from `InstantiateArgumentExceptionCorrectlyRule` to align its behaviour with FxCop's `CA2208:InstantiateArgumentExceptionsCorrectly`

⁋For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/trunk/ReleaseNotes%20-%20Previously.md