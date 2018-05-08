@echo off
set no_pause=yes
call BadPractice.bat
call Concurrency.bat
call Correctness.bat
call Design.bat
call Design-Generic.bat
call Design-Linq.bat
call Exceptions.bat
call Framework.bat
call Gendarme.bat
call Globalization.bat
call Interoperability.bat
call Interoperability-Com.bat
call Maintainability.bat
call Naming.bat
call NUnit.bat
call Performance.bat
call Portability.bat
call Security.bat
call Security-Cas.bat
call Serialization.bat
call Smells.bat
set no_pause=
echo.
echo.
echo.
echo.
echo ======================================================================
echo ======================================================================
echo ======================================================================
pause
