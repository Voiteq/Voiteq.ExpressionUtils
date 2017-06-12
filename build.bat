@echo off

.paket\paket.bootstrapper.exe
.paket\paket.exe restore

call :check

"packages\FAKE\tools\Fake.exe" build.fsx %*

call :check

:check
if errorlevel 1 (
  exit /b %errorlevel%
)