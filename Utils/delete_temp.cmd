@echo off
echo Deleting user and windows temp files...
rem - delete user temp files
%SystemDrive%
cd %temp%
del /q /f /s *.*
set CAT=%tmp%
dir "%%CAT%%"/s/b/a | sort /r >> %TEMP%\files2del.txt
for /f "delims=;" %%D in (%TEMP%\files2del.txt) do (del /q "%%D" & rd "%%D")
del /q %TEMP%\files2del.txt
rem - delete windows temp files
cd %systemroot%\temp
del /q /f /s *.*
set CAT=%systemroot%\temp
dir "%%CAT%%"/s/b/a | sort /r >> %TEMP%\files2del.txt
for /f "delims=;" %%D in (%TEMP%\files2del.txt) do (del /q "%%D" & rd "%%D")
del /q %TEMP%\files2del.txt
rd /S /Q "%systemroot%\temp"
mkdir "%systemroot%\temp"
rem - delete user's internet temporary files
echo Deleting Temporary Internet Files...
rd /S /Q "%USERPROFILE%\Local Settings\Temporary Internet Files\"
mkdir "%USERPROFILE%\Local Settings\Temporary Internet Files\"
rd /S /Q "%USERPROFILE%\Local Settings\Microsoft\Windows\Temporary Internet Files\"
mkdir "%USERPROFILE%\Local Settings\Microsoft\Windows\Temporary Internet Files\"
echo Cleaning unecessary backup files...
net stop wuauserv
del %systemroot%\SoftwareDistribution\download /q /s
net start wuauserv
dism /online /cleanup-image /spsuperseded







echo Clean complete!
pause