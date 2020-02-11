# set-version.ps1

$file = [System.Io.File]::ReadAllText("./gendarme/AssemblyStaticInfo.txt")
$now = [System.DateTime]::Now
$time = $now.ToString("HHmmss").Substring(0,5).TrimStart('0')
$version = $now.Year.ToString() + "." + $now.Month.ToString() + "." + $now.Day.ToString() + "."  + $time

$file = [System.Io.File]::ReadAllText("./gendarme/AssemblyStaticInfo.txt")
$newfile = $file.Replace("2011", $now.Year.ToString()).Replace("2.11.0.0", $version)
[System.Io.File]::WriteAllText("./gendarme/AssemblyStaticInfo.cs", $newfile)

$file = [System.Io.File]::ReadAllText("./gendarme/AssemblyStaticInfo.fs.txt")
$newfile = $file.Replace("-2010", "-" + $now.Year.ToString()).Replace("0.0.0.0", $version)
[System.Io.File]::WriteAllText("./gendarme/AssemblyStaticInfo.fs", $newfile)

$releaseNotes = [System.Io.File]::ReadAllText("./ReleaseNotes.md")
$nuspec = [Xml](Get-Content "./altcode.gendarme.xml")
$nuspec.package.metadata.version = $version + "-pre-release"
$nuspec.package.metadata.releaseNotes = $releaseNotes
$nuspec.Save("./altcode.gendarme.nuspec")
