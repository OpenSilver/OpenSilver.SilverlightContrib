@ECHO off

SET BUILD_DIR=%~dp0
rem SET SRC_DIR=%~dp0..\OpenSilver.SilverlightContrib
SET SRC_DIR=%~dp0..
SET CFG=Release

REM Define the escape character for colored text
FOR /F %%a IN ('"prompt $E$S & echo on & for %%b in (1) do rem"') DO SET "ESC=%%a"

REM Define the PackageVersion and OpenSilverPkgVersion variables
IF "%~1" == "" (
	SET /P PackageVersion="%ESC%[92mOpenSilver.SilverlightContrib version:%ESC%[0m "
	SET /P OpenSilverPkgVersion="%ESC%[92mOpenSilver version:%ESC%[0m "
) ELSE (
	SET PackageVersion=%1
	if "%~2" == "" (
		SET OpenSilverPkgVersion=%1
	) ELSE (
		SET OpenSilverPkgVersion=%2
	)
)

ECHO. 
ECHO %ESC%[95mBuilding %ESC%[0m%CFG% %ESC%[95mconfiguration%ESC%[0m
ECHO. 
msbuild %SRC_DIR%\SilverlightContrib.Opensilver.sln -p:Configuration=%CFG% -clp:ErrorsOnly -restore

ECHO. 
ECHO %ESC%[95mPacking %ESC%[0mOpenSilver.SilverlightContrib %ESC%[95mNuGet package%ESC%[0m
ECHO. 
%BUILD_DIR%\nuget.exe pack %BUILD_DIR%\nuspec\OpenSilver.SilverlightContrib.nuspec -OutputDirectory "%BUILD_DIR%\output" -Properties "PackageVersion=%PackageVersion%;OpenSilverDependencyVersion=%OpenSilverPkgVersion%;Configuration=%CFG%"
