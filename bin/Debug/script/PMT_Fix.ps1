$PMT = Get-package | select Name, Version | Where {$_.Name -eq "Protect myTech"}
if($PMT.Name -eq 'Protect myTech' -and $PMT.Version -match '2.20.1008')
{
Write-host "Protect myTech Latest Version is available and it is Compliance" -ForegroundColor Green
}
################### install Latest Version PMT #############################
elseif($PMT.Version -notmatch '2.20.1008')
{
write-host "Protect myTech needs to be install to the latest version" -ForegroundColor Red
write-host "Updating Protect myTech..." -ForegroundColor Gray

start-process C:\ProgramData\Accenture\ProtectMyTech\unins000.exe -wait

&cmd /c  del "C:\ProgramData\Accenture\ProtectMyTech"
&cmd /c  del "C:\ProgramData\Accenture\ProtectMyTechWinService"

$Folder = "$($env:LOCALAPPDATA)\Accenture\ProtectMyTech"
$Items | % { 
    if (Test-Path "$Folder\$_") {
        Remove-Item "$Folder\$_" 
    }
}
start-process C:\PMT_FIX\ProtectMyTechInstaller.exe -Wait
Start-Sleep 15
 }
 else
 {
Write-host "Protect myTech is not available in the system" -ForegroundColor Red
log(echo "Protect myTech is not available in the system")
Write-Host "Installing Protect myTech ..." -ForegroundColor gray
start-process C:\PMT_FIX\ProtectMyTechInstaller.exe -Wait
start-sleep 15
  
}

####### If Latest Version is install and getting same error then countinue the with unstallation ########
Add-Type -AssemblyName PresentationFramework
$msgBoxInput =  [System.Windows.MessageBox]::Show('Would you like to Uninstall ?','PMT input','YesNoCancel','Error')
switch  ($msgBoxInput) {
'Yes' {
start-process C:\ProgramData\Accenture\ProtectMyTech\unins000.exe -Wait
&cmd /c  del "C:\ProgramData\Accenture\ProtectMyTech"
&cmd /c  del "C:\ProgramData\Accenture\ProtectMyTechWinService"

}
'No'
{ 
Exit 
}

}

$Folder = "$($env:LOCALAPPDATA)\Accenture\ProtectMyTech"
$Items | % { 
    if (Test-Path "$Folder\$_") {
        Remove-Item "$Folder\$_" 
    }
}
Add-Type -AssemblyName PresentationFramework
$msgBoxInput =  [System.Windows.MessageBox]::Show('Would you like to install ?','PMT input','YesNoCancel','Error')
switch  ($msgBoxInput) {
'Yes' {
start-process C:\PMT_FIX\ProtectMyTechInstaller.exe -Wait
}

}

