msbuild ..\SquishIt.sln /p:Configuration=Release
for %%s in (..\nuspec\*.nuspec) do (
	nuget pack %%s
)
for %%p in (*.nupkg) do (
	nuget push %%p
	del %%p
)