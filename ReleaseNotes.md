# 2023.1.21.10xxx

* Exempt `Task` and `Task<'T>` from `EnsureLocalDisposalRule` as they generally should not be disposed manually.
* Remove the "no arguments means no error" hack from `InstantiateArgumentExceptionCorrectlyRule` to align its behaviour with FxCop's `CA2208:InstantiateArgumentExceptionsCorrectly`; only with the added trick of looking at the top-level user-declared method in case of compiler generated functions (e.g. in `yield` based iterators)

⁋For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/trunk/ReleaseNotes%20-%20Previously.md