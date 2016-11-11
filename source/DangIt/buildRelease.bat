@echo off

copy /Y "bin\Release\DangIt.dll" "..\..\GameData\DangIt\Plugins"
copy /Y ..\DangIt.version ..\..\GameData\DangIt
copy /Y ..\..\..\MiniAVC.dll ..\..\GameData\DangIt

set DEFHOMEDRIVE=d:
set DEFHOMEDIR=%DEFHOMEDRIVE%%HOMEPATH%
set HOMEDIR=
set HOMEDRIVE=%CD:~0,2%

set RELEASEDIR=d:\Users\jbb\release
set ZIP="c:\Program Files\7-zip\7z.exe"
echo Default homedir: %DEFHOMEDIR%

rem set /p HOMEDIR= "Enter Home directory, or <CR> for default: "

if "%HOMEDIR%" == "" (
set HOMEDIR=%DEFHOMEDIR%
) 
echo %HOMEDIR%

SET _test=%HOMEDIR:~1,1%
if "%_test%" == ":" (
set HOMEDRIVE=%HOMEDIR:~0,2%
)


type ..\DangIt.version
set /p VERSION= "Enter version: "

mkdir ..\..\GameData\DangIt\licenses
xcopy /y /s ..\..\licenses ..\..\GameData\DangIt

copy /Y ..\..\README.md ..\..\GameData\DangIt
 
cd ..\..

set FILE="%RELEASEDIR%\DangIt -%VERSION%.zip"
IF EXIST %FILE% del /F %FILE%
%ZIP% a -tzip %FILE% GameData
pause