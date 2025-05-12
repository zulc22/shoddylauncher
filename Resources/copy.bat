@echo off
echo Copying...
set desktop=%windir%\Desktop
set copyto=%desktop%\From Shoddy Launcher
if not exist "%copyto%" goto dontdelete
echo Delet
deltree /y "%copyto%" >nul
:dontdelete
xcopy /e /y EMULATOR\ "%copyto%\Emulator\" >nul
start "%copyto%"
