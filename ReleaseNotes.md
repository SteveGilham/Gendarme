# 2022.x.x.xxxxx-pre-release 

* Make a heuristic fix for the changed IL that meant a test failure for `AvoidUnnecessarySpecializationRule`
  * code that was IL_0001: ldarga.s x/	IL_0003: constrained. !!T (stack unchanged) is now IL_0001: ldarg.1/IL_0002: box !!T which pops the loaded value and pushes the boxed copy, so treat ldarg+box as a unit.
* Make heuristic fixes for `CheckParametersNullityInVisibleMethods` -- also related to boxing generics; and cases of different choices of comparison operation
* Propagate the checks for the changed null comparison IL

⁋For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/trunk/ReleaseNotes%20-%20Previously.md