msbuild ..\SquishIt.sln /p:Configuration=Release
nuget pack ..\releases\squishit.nuspec
nuget pack ..\releases\squishit.mvc.nuspec
for %%p in (*.nupkg) do (
	nuget push %%p
	rm %%p
)