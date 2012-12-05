@echo off
REM need to have MSBuild and 7zip in your path
if %1.==. goto noversion
msbuild ../SquishIt.sln /p:Configuration=Release
set packageFolder=SquishIt-%1
mkdir %packageFolder%

echo %packageFile%
xcopy /S ..\SquishIt.Framework\bin\Release %packageFolder%
xcopy /S ..\licenses %packageFolder%
del %packageFolder%\dotless-license.txt %packageFolder%\jurassic-license.txt

cd %packageFolder%
7z a ..\%packageFolder%.zip 
cd ..

rmdir /S/Q %packageFolder%

goto end

:noversion
	echo No version passed
:end
