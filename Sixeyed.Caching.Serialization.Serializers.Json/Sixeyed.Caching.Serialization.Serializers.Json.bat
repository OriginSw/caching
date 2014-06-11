:: nuget spec
set /p version=Version number:
nuget pack Sixeyed.Caching.Serialization.Serializers.Json.csproj -Prop Configuration=Debug -IncludeReferencedProjects
nuget push Sixeyed.Caching.Serialization.Serializers.Json-Bardock.%version%.nupkg
pause;