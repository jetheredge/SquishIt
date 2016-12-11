msbuild ..\SquishIt.sln /p:Configuration=Release /p:Platform="Any CPU" /p:VisualStudioVersion=14.0
for %%s in (..\nuspec\*.nuspec) do (
	nuget pack %%s
)
for %%p in (*.nupkg) do (
	nuget push %%p -Source https://www.nuget.org/api/v2/package
	del %%p
)