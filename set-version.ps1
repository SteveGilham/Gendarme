# set-version.ps1

$releaseNotes = [System.Io.File]::ReadAllText("./ReleaseNotes.md")
$nuspec = [Xml](Get-Content "./altcode.gendarme.xml")
$nuspec.package.metadata.version = $version + "-pre-release"
$nuspec.package.metadata.releaseNotes = $releaseNotes
$nuspec.Save("./altcode.gendarme.nuspec")

#dir -recurse .\gendarme\*.csproj | % {
#  $_.FullName
#  $doc =  [Xml](Get-Content $_.FullName)
#  $doc.Project.ItemGroup.Compile | ? {
#      $_.Include -like "*AssemblyStaticInfo.cs*"
#  } | % { $_.Include = "`$(SolutionDir)..\_Generated\AssemblyStaticInfo.cs" }
#  $doc.Save($_.FullName)
#}

#  Push-Location $_.DirectoryName
#  $relativePath = Get-Item "C:\Users\steve\Documents\GitHub\Gendarme\gendarme" | Resolve-Path -Relative
#  Pop-Location
#  $lines = [System.Io.File]::ReadAllLines($_.FullName)
#  $lines -like "*AssemblyStaticInfo.cs*"
#}  

#        "    <SolutionDir Condition=`"'`$(SolutionDir)' == ''`">`$(ProjectDir)$relativePath\</SolutionDir>"
#        "    <OutputPath>`$(SolutionDir)..\_Binaries/`$(AssemblyName)/`$(Configuration)+`$(Platform)/</OutputPath>"
#        "    <IntermediateOutputPath>`$(SolutionDir)..\_Intermediate/`$(AssemblyName)/`$(Configuration)+`$(Platform)/</IntermediateOutputPath>"

dir -recurse .\gendarme\*.csproj | % {
  $_.FullName
  $first = $true
  $lines = [System.Io.File]::ReadAllLines($_.FullName)
  $newlines = $lines | % {
    if ($first -and ($_ -like "*</PropertyGroup>")) {
        $first = $false
      "    <ContinuousIntegrationBuild Condition=`"'`$(APPVEYOR)'=='True'`">true</ContinuousIntegrationBuild>"
      "    <DeterministicSourcePaths Condition=`"'`$(APPVEYOR)'=='True'`">true</DeterministicSourcePaths>"
    }
    $_
  }
  [System.Io.File]::WriteAllLines($_.FullName,$newlines)
}


