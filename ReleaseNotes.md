# 2020.3.7.101xx-pre-release

* Improve the assembly resolution to 
 * distinguish different versions of the same assembly
 * look in the nuget cache if the assembly isn't local (means that .net core code needn't be published, at the cost of the extra time taken to search the cache.)

For previous releases, go here -- https://github.com/SteveGilham/Gendarme/blob/master/ReleaseNotes%20-%20Previously.md
Since this is a release from a fork, [issues should be reported at my related repo](https://github.com/SteveGilham/altcode.fake/issues) that contains a Fake driver for the Gendarme tool, but noted as being against the forked Gendarme tool itself.