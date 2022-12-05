$file = dir -recurse "./packages/altcover/AltCover.P*werShell.dll" | Select-Object -First 1

Import-Module $file.FullName

$files = dir "./_Reports/UnitTestWithAltCoverRunner.Test.*.xml" | % { $_.FullName }

$xml = $files | Merge-OpenCover -OutputFile "_Reports/CombinedTestWithAltCoverRunner.coveralls"