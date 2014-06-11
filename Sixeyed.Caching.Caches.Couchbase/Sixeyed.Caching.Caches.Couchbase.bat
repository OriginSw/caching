:: nuget spec
set /p version=Version number:
nuget pack Sixeyed.Caching.Caches.Couchbase.csproj -Prop Configuration=Debug -IncludeReferencedProjects
nuget push Sixeyed.Caching.Caches.Couchbase-Bardock.%version%.nupkg
pause;