@echo off
echo Copying...
set desktop=%windir%\Desktop
set copyto=%desktop%\From Shoddy Launcher
if not exist "%copyto%\NUL" goto dontdelete
deltree /y "%copyto%" >nul
:dontdelete
xcopy /e /y EMULATOR\ "%copyto%\Emulator\"
if not exist "ROMS\NUL" goto noroms
xcopy /e /y ROMS\ "%copyto%\ROMs\"
:noroms
start "%copyto%"
