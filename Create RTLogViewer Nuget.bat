@echo off
set workingdir=./RTLogViewer
set nuspecfile=RTLogViewer.nuspec
set projectname=RTLogViewer

if NOT EXIST "%WORKINGDIR%/Publish" mkdir "%WORKINGDIR%/Publish"

if not exist "%WORKINGDIR%/Publish/%NUSPECFILE%" GOTO CREATE_NUSPEC
GOTO CREATE_PKG

:CREATE_NUSPEC
cd %WORKINGDIR%
"../nuget.exe" spec
MOVE %NUSPECFILE% "Publish/%NUSPECFILE%"
cd ..
echo ""
echo "Please edit spec file if needed. Then press any key to continue"
pause

:CREATE_PKG

cd "%WORKINGDIR%/Publish"
del *.nupkg
cd ../..

nuget.exe pack -OutputDirectory %WORKINGDIR%/Publish %WORKINGDIR%/%PROJECTNAME%.csproj -Prop Configuration=Release
echo ""
echo "Please edit nuget package file if needed. Then press any key to continue with pushing to server"
pause

for %%F IN (%WORKINGDIR%/Publish/*.nupkg) DO nuget.exe push %WORKINGDIR%/Publish/%%F -s http://froddannuget.azurewebsites.net/ 18736DBE-9827-3A4B-D80B-1115BE6347BD
