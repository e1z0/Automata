@echo off

if _%1_==__ (
echo.
echo Usage: %0 folderpath
echo Deletes specified folder and all subfolders, even if the path depth
echo is larger than what CMD/Explorer will manage.
echo   folderpath - the folder to delete
echo.
goto end
)
 
setlocal

rem ** The full path of the folder to delete
set target_folder="%~f1"
set target_drive=%~d1

rem ** Create a temp folder on the same drive as the target folder (since moves within the same drive does not involve copying and traversing paths)
set tmp=%3
set del_temp_folder=nope
if {%tmp%}=={} set tmp=%target_drive%\temp_%RANDOM%
if not exist %tmp% (
md %tmp%
echo Will delete folder: %target_folder%
echo Will use temp folder: %tmp%
set del_temp_folder=yepp
pause
)

rem ** Create a prefix to use on moved folders to avoid name clashes
set i=%2
if {%i%}=={} set i=0
set /A i=%i% + 1

echo Level %i%, folder %target_folder%

rem ** Delete all files (non-directories) from the folder
del /Q /F %target_folder%\*

rem ** Move all subfolders to the tempfolder, and delete the now empty folder
for /D %%d in (%target_folder%\*) do (
move "%%d" "%tmp%\%i%_%%~nd"
)
rd /Q %target_folder%
 
rem ** Recursively repeat the above for all the moved subfolders
for /D %%d in (%tmp%\%i%_*) do (
call %0 "%%d" %i% %tmp%
)

rem ** Clean upp
if {%del_temp_folder%}=={yepp} (
rd /Q %tmp%
)

endlocal
:end
pause