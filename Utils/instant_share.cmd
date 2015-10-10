@echo off
rem (c) 2015 \dev\null && EofNET Networks.
if _%1_==__ (
echo.
echo Usage: %0 folderpath
echo Shares specified folder and all subfolders, with read permissions for everyone
echo   folderpath - the folder to share
echo.
goto end
)
 
setlocal

rem ** The full path of the folder to delete
set target_folder="%~f1"
set target_drive=%~d1
for /F %%i in ("%1") do @set FN=%%~nxi
net share %FN%="%1"
icacls "%1" /grant Everyone:(OI)(CI)R

endlocal
:end
pause