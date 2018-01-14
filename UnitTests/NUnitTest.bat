@echo off
if not exist "nunit3-console.lnk" goto LinkMissing
if _%1_ == __ goto ParamMissing
if _%2_ == __ goto ParamMissing

if not exist "Results" md "Results"

nunit3-console.lnk /result:%1.xml %2 %3 %4 %5 %6 %7 %8 %9
NUnit3summary.exe %1.xml %1.txt
goto End

:LinkMissing
echo.
echo Link to "nunit3-console" is missing!
echo.
echo Add link to "nunit3-console" application and
echo update the parameters to run in current folder.
echo (Set 'start in' folder to empty.)
echo.
goto End

:ParamMissing
echo.
echo No parameter!
echo.


:End
pause
