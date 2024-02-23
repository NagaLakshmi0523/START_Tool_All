$msgBoxInput =  [System.Windows.MessageBox]::Show('Would you like to close ?','Browser  input','YesNoCancel','Error')


  switch  ($msgBoxInput) {

 
  'Yes' {
  #Get-Process msedg -ErrorAction SilentlyContinue |
 #Where-Object { $_.StartTime -and (Get-Date).AddMinutes(-5) -gt $_.StartTime } |
#Stop-Process
Get-Process iexplore -ErrorAction SilentlyContinue |
Where-Object { $_.StartTime -and (Get-Date).AddMinutes(-5) -gt $_.StartTime } |
Stop-Process
rundll32.exe inetcpl.cpl ResetIEtoDefaults
Start-Sleep -Seconds 1
[System.Windows.Forms.SendKeys]::SendWait("R") 
rundll32.exe keymgr.dll,KRShowKeyMgr
Start-Sleep -Seconds 1
[System.Windows.Forms.SendKeys]::SendWait("K") 
   & rundll32.exe inetcpl.cpl,ClearMyTracksByProcess 4351
   Start-Sleep -Seconds 1
Get-Process chrome -ErrorAction SilentlyContinue |
Where-Object { $_.StartTime -and (Get-Date).AddMinutes(-5) -gt $_.StartTime } |
Stop-Process 
Start-Sleep -Seconds 1 
#$#######
 Get-Process msedg -ErrorAction SilentlyContinue |
 Where-Object { $_.StartTime -and (Get-Date).AddMinutes(-5) -gt $_.StartTime } |
Stop-Process

 

#Clearing Data
$Items = @('Archived History',
            'Cache\*',
            'Cookies',
            'History',
            'Login Data',
            'Top Sites',
            'Visited Links',
            'Web Data')
$Folder = "$($env:LOCALAPPDATA)\Google\Chrome\User Data\Default"
$Items | % { 
    if (Test-Path "$Folder\$_") {
        Remove-Item "$Folder\$_" 
    }
}
Clear-host
{

 

 

 

  ForEach($cred in $L)
  {

 

 

 

    Write-host "`tProcessing...`n"
    Write-host "$($cred.TargetName.ToString())`n"
    Write-host "$($cred.Type.ToString())`n`n"

 

 

 

    $R = Remove-CredManCredential -TargetName  $($cred.TargetName.ToString()) -Type $($cred.Type.ToString()) -Force

 

 

 

  }
  $L = Get-CredManCredentialList -ErrorAction SilentlyContinue -ErrorVariable $Cred_Error

 

 

 

  If($L -eq $null)
  {

 

 

 

    Write-host "All Cached Credentials removed, program Complete"
    $LASTEXITCODE = 0
    Write-host "The last exit code is $LASTEXITCODE"

 

 

 

  }
  Else
  {

 

 

 

    Write-host "WARNING: One or more Cached Credentials were not removed, program Complete"
    $LASTEXITCODE = 1

 

 

 


  }
}

 


 

 


  }

 

 

 

  'No' {

 

 

 

 

 

  }
  }
Get-Process Msedge | % { $_.CloseMainWindow() }
Get-ChildItem -Path $($env:LOCALAPPDATA)\Local\Microsoft\Edge -File -Recurse -Force| Remove-Item -Verbose
Get-AppXPackage -AllUsers -Name Microsoft.MicrosoftEdge | Foreach {Add-AppxPackage -DisableDevelopmentMode -Register “$($_.InstallLocation)\AppXManifest.xml” -verbose}