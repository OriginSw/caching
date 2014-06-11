:: nuget spec
set /p version=Version number:
nuget pack Sixeyed.Caching.csproj -Prop Configuration=Debug -IncludeReferencedProjects
nuget push Sixeyed.Caching-Bardock.%version%.nupkg
pause;