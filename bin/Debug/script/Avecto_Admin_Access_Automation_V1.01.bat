xcopy "%~dp0*" "C:\Windows\temp\BeyondTrust_Privilege" /q /s /e /y /i
PowerShell.exe -ExecutionPolicy Bypass -File "C:\Windows\temp\BeyondTrust_Privilege\Avecto_Admin_Access_Automation_V1.01.ps1"