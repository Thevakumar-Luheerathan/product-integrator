REM Accepts two arguments: ballerina.zip and integrator.zip
REM Extracts them to WixPackage\payload\Ballerina and WixPackage\payload\Integrator

@echo off
setlocal

REM Check for required arguments
if "%~1"=="" (
    echo Usage: build.bat ^<path-to-ballerina.zip^> ^<path-to-integrator.zip^>
    exit /b 1
)
if "%~2"=="" (
    echo Usage: build.bat ^<path-to-ballerina.zip^> ^<path-to-integrator.zip^>
    exit /b 1
)

REM Remove existing Ballerina and Integrator directories if they exist
powershell -Command "if (Test-Path '.\WixPackage\payload\Ballerina') { Remove-Item -Recurse -Force '.\WixPackage\payload\Ballerina' }"
powershell -Command "if (Test-Path '.\WixPackage\payload\Integrator') { Remove-Item -Recurse -Force '.\WixPackage\payload\Integrator' }"

REM Extract ballerina.zip
powershell -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::ExtractToDirectory('%~1', 'C:\'); }"
move "C:\ballerina-"* ".\WixPackage\payload\Ballerina"

REM Extract integrator.zip
powershell -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::ExtractToDirectory('%~2', '.\WixPackage\payload\Integrator'); }"

dotnet build .\CustomAction1\CustomAction1.csproj
dotnet build .\WixPackage\WixPackage.wixproj -p:Platform=x64
endlocal
