<#Description :- The puprose of this script is to create Touchless Avecto Admin Access Provision for only ATCI employees.

last Modified date :- 07-12-2021 14:00pm
#>


function Get-TimeStamp {
    return "[{0:MM/dd/yy} {0:HH:mm:ss}]" -f (Get-Date)
}
$logfile = "C:\windows\temp\Avecto_Logs_$($env:COMPUTERNAME).txt"
"********************* Script Started ******************************" | Out-File $logfile -Append
$get_username = (Get-CimInstance -ClassName Win32_ComputerSystem | Select-Object -Property UserName).username
$username = $get_username.Split("\")[1]
$check_task = Get-ScheduledTask | Where-Object taskname -eq "Avecto_Admin_Access"
$script_status = "C:\Windows\Temp\Avecto_Script_State.txt"
if ($check_task.TaskName -ne "Avecto_Admin_Access") {
    $action = New-ScheduledTaskAction -Execute powershell.exe -Argument " -Executionpolicy Bypass -windowstyle hidden -File C:\Windows\Temp\BeyondTrust_Privilege\Avecto_Admin_Access_Automation.ps1"
    $principal = New-ScheduledTaskPrincipal -UserId "System" -LogonType ServiceAccount -RunLevel Highest
    $task_setting = New-ScheduledTaskSettingsSet -AllowStartIfOnBatteries
    $trigger1 = New-ScheduledTaskTrigger -Daily -At 12pm
    $trigger2 = New-ScheduledTaskTrigger -Daily -At 4pm
    $trigger3 = New-ScheduledTaskTrigger -Daily -At 7pm
    Register-ScheduledTask -Action $action -Trigger $trigger1, $trigger2, $trigger3 -Principal $principal -TaskName "Avecto_Admin_Access" -Description "This script will check for Avecto Admin Access Provision" -Settings $task_setting -Force    
    "$(Get-TimeStamp) Avecto Admin Script added in task Scheduler." | Out-File $logfile -Append
}
else {
    Write-Host "Avecto Admin Script is already in Task Schedular"
    "$(Get-TimeStamp) Avecto Admin Script is already Task Schedular." | Out-File $Logfile -Append 
}

#Getting ATCI / AO Entity
$params = @{uri = "https://rwm.accenture.com/GetUserInfo/api/UserAccess/?id=$($username)";
    Method      = 'Get'; 
    Headers     = @{Authorization = "Basic QTA1MTI2c2VsZnN1cHBvcnQ6REY4Nm9EQyFTeSMxcSFLZ0t1Zg=="; }
}
$Entity = invoke-restmethod @params
$strServiceName = "Avecto Defendpoint Service"
$appToMatch = '*Privilege Management*'
$appToMatch1 = '*Avecto*'

function Get-InstalledApps {
    if ([IntPtr]::Size -eq 4) {
        $regpath = 'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*'
    }
    else {
        $regpath = @(
            'HKLM:\Software\Microsoft\Windows\CurrentVersion\Uninstall\*'
            'HKLM:\Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\*'
        )
    }
    Get-ItemProperty $regpath | . { process { if ($_.DisplayName -and $_.UninstallString) { $_ } } } | Select DisplayName, Publisher, InstallDate, DisplayVersion, UninstallString | Sort DisplayName
}

function Install-BeyondTrust {
    
    $BeyondTrust_PrivilegeSource = 'https://mytechhelp.accenture.com/downloadSoftware/BeyondTrust_Privilege'
    $BeyondTrust_PrivilegeDestination = "C:\Windows\Temp\BeyondTrust_Privilege\BeyondTrust_Privilege.zip"
    $BeyondTrust_PrivilegeUnzip = 'C:\Windows\Temp\BeyondTrust_Privilege\BeyondTrust_Privilege'
    if (Test-Path $BeyondTrust_PrivilegeDestination) { Remove-Item -Path $BeyondTrust_PrivilegeDestination -Force -Recurse -ErrorAction SilentlyContinue}
    if (Test-Path $BeyondTrust_PrivilegeUnzip) { Remove-Item -Path $BeyondTrust_PrivilegeUnzip -Force -Recurse -ErrorAction SilentlyContinue}
    
    if((Invoke-WebRequest -Uri 'https://mytechhelp.accenture.com').statuscode -eq 200){
    "$(Get-TimeStamp) Downloading BT" | Out-File $logfile -Append
    Invoke-RestMethod -Uri $BeyondTrust_PrivilegeSource -OutFile $BeyondTrust_PrivilegeDestination 
    Expand-Archive -LiteralPath $BeyondTrust_PrivilegeDestination -DestinationPath $BeyondTrust_PrivilegeUnzip -force
    Write-Host "Installing BT" -ForegroundColor Cyan
    "$(Get-TimeStamp) Installing BT" | Out-File $logfile -Append 
    #Start-Process -FilePath C:\Windows\System32\cscript.exe -ArgumentList "C:\Windows\Temp\BeyondTrust_Privilege\BeyondTrust_Privilege_5.5.144.vbs", "/s" -Wait
    & cmd.exe /C "C:\Windows\Temp\BeyondTrust_Privilege\BeyondTrust_Privilege\BeyondTrust_Privilege\BeyondTrust_Privilege_5.5.144.vbs /s"  
    Start-Sleep -Seconds 10
    Check-services
    }else{
        Write-Host "Unable to reach Software repo" -ForegroundColor Cyan
        "$(Get-TimeStamp) Unable to reach Software repo" | Out-File $logfile -Append 
    }
}

function Check-services {
    $OutputVariable2 = (SC.EXE Config Winrm Start=auto) | Out-String
    $winrm = (Get-Service -Name WinRM).Status
    
    $winrm_startup = sc.exe qc "winrm" | findstr "START_TYPE"
    $winrm_startup = $winrm_startup.Split("2")[1]
    if ($winrm -eq "Running" -and $winrm_startup -like "*Auto*") {
        Write-Host "WINRM status is $winrm and StartupType is $winrm_startup and Service Config Auto State - $OutputVariable2"
        "$(Get-TimeStamp) WINRM status is $winrm and StartupType is $winrm_startup and Service Config Auto State - $OutputVariable2" | Out-File $logfile -Append 
    }
    else {
        Write-Host "WINRM status is $winrm and StartupType is $winrm_startup ."
        Start-Service -Name WinRM
        $OutputVariable2 = (SC.EXE Config Winrm Start=auto) | Out-String  
        $winrm = (Get-Service -Name WinRM).Status
        $winrm_startup = (Get-Service -Name winrm).StartType
        $winrm_startup = sc.exe qc "winrm" | findstr "START_TYPE"
        $winrm_startup = $winrm_startup.Split("2")[1]
        if ($winrm -eq "Running" -and $winrm_startup -like "*Auto*") {
            Write-Host "WINRM status is $winrm and StartupType is $winrm_startup and Service Config Auto State - $OutputVariable2."
            "$(Get-TimeStamp) WINRM status is $winrm and StartupType is $winrm_startup and Service Config Auto State - $OutputVariable2" | Out-File $logfile -Append
        }
        else {
            Write-Host "WINRM status is $winrm and StartupType is $winrm_startup and Service Config  Auto State - $OutputVariable2 ."
            Write-Host "Failed to start WinRM Service"
            "$(Get-TimeStamp) WINRM status is $winrm and StartupType is $winrm_startup and Service Config Auto State - $OutputVariable2" | Out-File $logfile -Append
        }
    }
    $OutputVariable = (SC.EXE Config $strServiceName Start= Delayed-Auto) | Out-String  
    $BTService = (Get-Service -Name $strServiceName).Status
    $BTService = (Get-Service -Name $strServiceName).Status
    $BTStartup = sc.exe qc "Avecto Defendpoint Service" | findstr "START_TYPE"
    $BTStartup = $BTStartup.Split("2")[1]    
    if ($BTService -eq "Running" -and $BTStartup -like "*Auto*") {
        Write-Host "BTService status is $BTService and StartupType is $BTStartup and Service Config Delayed Auto State - $OutputVariable"
        "$(Get-TimeStamp) BTService status is $BTService and StartupType is $BTStartup and Service Config Delayed Auto State - $OutputVariable" | Out-File $logfile -Append
    }
    else {
        Write-Host "BTService status is $BTService and StartupType is $BTStartup ."
        Start-Service -Name $strServiceName; Start-Sleep -Seconds 5
        $OutputVariable = (SC.EXE Config $strServiceName Start= Delayed-Auto) | Out-String       
        $BTService = (Get-Service -Name $strServiceName).Status
        $BTStartup = sc.exe qc "Avecto Defendpoint Service" | findstr "START_TYPE"
        $BTStartup = $BTStartup.Split("2")[1]
        Write-Host "SC Startup Service for BTService $OutputVariable"
        if ($BTService -eq "Running" -and $BTStartup -like "*Auto*") {
            
            Write-Host "BTService status is $BTService and StartupType is $BTStartup and Service Config Delayed Auto State - $OutputVariable"
            "$(Get-TimeStamp) BTService status is $BTService and StartupType is $BTStartup and Service Config  Delayed Auto State - $OutputVariable" | Out-File $logfile -Append
        }
        else {
            Write-Host "BTService status is $BTService and StartupType is $BTStartup and Service Config Delayed Auto State - $OutputVariable"
            Write-Host "Failed to Start BT Service"
            "$(Get-TimeStamp) BTService status is $BTService and StartupType is $BTStartup and Service Config Delayed Auto State - $OutputVariable" | Out-File $logfile -Append
        }
    }
}

function Checkloggedin-User {
    $UserGroups = Get-LocalGroup | Select-Object -ExpandProperty Name -ErrorAction SilentlyContinue
    $admincheck = Get-LocalGroupMember Administrators | select-Object -ExpandProperty Name
    "-------------------------------------------------------" | Out-File $logfile -Append
    "Below Usersgroups Already available in system :- " | Out-File $logfile -Append
    foreach($user in $UserGroups){
     
    "$user" | Out-File $logfile -Append
    }
    "-------------------------------------------------------" | Out-File $logfile -Append
    "Below users already available in Administrator group :-" | Out-File $logfile -Append
    foreach($admin in $admincheck){
    
    "$admin" | Out-File $logfile -Append
    }
    "-------------------------------------------------------" | Out-File $logfile -Append
    if ($admincheck -contains "$get_username") {
        Write-Host "Current user - $($get_username) is a part of Administrator group"
        "$(Get-TimeStamp) Current user - $($get_username) is a part of Administrator group " | Out-File $logfile -Append
        Remove-LocalGroupMember Administrators -Member $get_username -ErrorAction SilentlyContinue
        $admincheck = Get-LocalGroupMember Administrators | select-Object -ExpandProperty Name
        if (!($admincheck -contains $get_username)) {
            Write-Host "Removed Current user - $($get_username) from Administrator group"
            "$(Get-TimeStamp) Removed Current user - $($get_username) from Administrator group " | Out-File $logfile -Append
        }
        else {
            Write-Host "Failed to remove Current user - $($get_username) from Administrator group"
            "$(Get-TimeStamp) Failed to remove Current user - $($get_username) from Administrator group " | Out-File $logfile -Append
        }
    }
    else {
        Write-Host "Current user - $($get_username) is not a part of Administrator group"
        "$(Get-TimeStamp) Current user - $($get_username) is not a part of Administrator group " | Out-File $logfile -Append
    }


    if ($UserGroups -contains "AvectoUsers") {   
        Write-Host "AvectoUsers group is Already available" -ForegroundColor Green
        "$(Get-TimeStamp) AvectoUsers group is Already available" | Out-File $logfile -Append
        #Add-LocalGroupMember -Group "AvectoUsers" -Member $username -ErrorAction SilentlyContinue
        $Avectomembers = Get-LocalGroupMember AvectoUsers | select-Object -ExpandProperty Name
        if ($Avectomembers -contains "DIR\$username") {       
            Write-Host "The user $($get_username) is already part of AvectoUserGroup" -ForegroundColor Green
            "$(Get-TimeStamp) The user $($get_username) is already part of AvectoUserGroup" | Out-File $logfile -Append
            "Success" | Out-File $script_status
            $Script:state_ZT = "Failed"
            "$(Get-TimeStamp) The ZeroTouch State is $state_ZT" | Out-File $logfile -Append
        }
        else {
            Add-LocalGroupMember -Group "AvectoUsers" -Member $username -ErrorAction SilentlyContinue
            $Avectomembers = Get-LocalGroupMember AvectoUsers | select-Object -ExpandProperty Name
            if ($Avectomembers -contains "DIR\$username") {
                Write-Host "Successfully added $username to the AvectoUsersGroup"
                "$(Get-TimeStamp) Successfully added - $username to the AvectoUsersGroup" | Out-File $logfile -Append
                "Success" | Out-File $script_status
                $script:state_ZT = "Success"
                "$(Get-TimeStamp) The ZeroTouch State is $state_ZT" | Out-File $logfile -Append
            }
            else {
                Write-Host "Failed to add $username to the AvectoUsers group" -ForegroundColor Red
                "$(Get-TimeStamp) Failed to add - $username to the AvectoUsers group" | Out-File $logfile -Append
                "Failed" | Out-File $script_status
                $Script:state_ZT = "Failed"
                "$(Get-TimeStamp) The ZeroTouch State is $state_ZT" | Out-File $logfile -Append
            }
        }
    }
    else { 
        "$(Get-TimeStamp) AvectoUsers group is not available" | Out-File $logfile -Append
        New-LocalGroup -Description "Users allowed to invoke Avecto access" -Name "AvectoUsers" -ErrorAction SilentlyContinue
        $UserGroups = Get-LocalGroup | Select-Object -ExpandProperty Name -ErrorAction SilentlyContinue
        if ($UserGroups -contains "AvectoUsers") {
            Write-Host "AvectoUsers group is created successfully." -ForegroundColor Green
            "$(Get-TimeStamp) AvectoUsers group is created successfully" | Out-File $logfile -Append
            Add-LocalGroupMember -Group "AvectoUsers" -Member $username -ErrorAction SilentlyContinue
            $Avectomembers = Get-LocalGroupMember AvectoUsers | select-Object -ExpandProperty Name
            if ($Avectomembers -contains "DIR\$username") {
                Write-Host "Successfully added $username to the AvectoUsersGroup"
                "$(Get-TimeStamp) Successfully added $username to the AvectoUsersGroup" | Out-File $logfile -Append
                "Success" | Out-File $script_status
                $Script:state_ZT = "Success"
            "$(Get-TimeStamp) The ZeroTouch State is $state_ZT" | Out-File $logfile -Append

            }
            else {
                Write-Host "Failed to add $username to the AvectoUsers group" -ForegroundColor Red
                "$(Get-TimeStamp) Failed to add $username to the AvectoUsers group" | Out-File $logfile -Append
                "Failed" | Out-File $script_status
                $Script:state_ZT = "Failed"
            "$(Get-TimeStamp) The ZeroTouch State is $state_ZT" | Out-File $logfile -Append
            }
            
        }
        else {
            Write-Host "Failed to create the AvectoUsers group" -ForegroundColor Red
            "$(Get-TimeStamp) Failed to create the AvectoUsers group" | Out-File $logfile -Append
            "Failed" | Out-File $script_status
            $Script:state_ZT = "Failed"
            "$(Get-TimeStamp) The ZeroTouch State is $state_ZT" | Out-File $logfile -Append
            
        }  
    }
    
}

function Check-taskschedular {

    $SC_state = Get-Content C:\Windows\Temp\Avecto_Script_State.txt
    if ($SC_state -eq "Success") {
        "$(Get-TimeStamp) Unregistering task Schedular." | Out-File $logfile -Append
        Remove-Item -Path C:\Windows\Temp\BeyondTrust_Privilege -Recurse -Force -ErrorAction SilentlyContinue
        Unregister-ScheduledTask -TaskName "Avecto_Admin_Access" -Confirm:$false -ErrorAction SilentlyContinue
        $check_task = Get-ScheduledTask | Where-Object taskname -eq "Avecto_Admin_Access"
        if ($check_task.TaskName -ne "Avecto_Admin_Access") {
            "$(Get-TimeStamp) The Avecto_Admin_Access Task Schedular is removed." | Out-File $logfile -Append     
        }
    }
    else {

        "$(Get-TimeStamp) The Script will run at next trigger." | Out-File $logfile -Append
    }

}

if ($Entity) {
    Write-Host "Enterprise ID - $($username) belongs to ATCI."
    "$(Get-TimeStamp) The Entity is $($Entity)." | Out-File $logfile -Append
    "$(Get-TimeStamp) Enterprise ID - $($username) belongs to ATCI." | Out-File $logfile -Append
    $PrivilegeManagement = Get-InstalledApps | Where-Object { $_.DisplayName -like $appToMatch }
    $AvectoDefendpoint = Get-InstalledApps | Where-Object { $_.DisplayName -like $appToMatch1 }
    if (($null -eq $PrivilegeManagement.DisplayVersion) -and ($null -eq $AvectoDefendpoint.DisplayVersion)) {
        Install-BeyondTrust        
        Checkloggedin-User


    }
    else {
        "$(Get-TimeStamp) Beyond Privilege Already Installed $($PrivilegeManagement.DisplayVersion) $($AvectoDefendpoint.DisplayVersion) ." | Out-File $logfile -Append
        Check-services
        Checkloggedin-User
    }
    Check-taskschedular
    "$(Get-TimeStamp) The Zerotouch State is :- $state_ZT ." | Out-File $logfile -Append
}
elseif ($null -eq $Entity) {

    Write-Host "Unable to reach Server."
    "$(Get-TimeStamp) Unable to reach Server. The Script will run at next trigger." | Out-File $logfile -Append
    "Failed" | Out-File $script_status
}
else {
    Write-Host "Enterprise ID - $($username) does not belong to ATCI."
    "$(Get-TimeStamp) The Entity is $($Entity)." | Out-File $logfile -Append
    "$(Get-TimeStamp) Enterprise ID - $($username) does not belong to ATCI." | Out-File $logfile -Append
    "Success" | Out-File $script_status
    $Script:state_ZT = "Failed"
    Check-taskschedular
    "$(Get-TimeStamp) The Zerotouch State is :- $state_ZT." | Out-File $logfile -Append
}

"********************* Script Completed ******************************" | Out-File $logfile -Append
#############Adding ZeroTouchCode ######################################
# Configuring default paths and values.
$gbl_RootPath = Split-Path $MyInvocation.MyCommand.Path
$gbl_LogPath = $gbl_RootPath + '\Logs'
$gbl_OutputFilePath = "C:\Users\anthony.r.d.sanchez\Desktop"
$gbl_OutputFileDate = (Get-Date).ToString("MMddyyyy")
$gbl_OutputFileName = "OneTouch_$gbl_OutputFileDate.txt"

# For forcing the script to use TLS 1.2
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

# OAuth v2.00 authentication references.
$gbl_TokenUrl = 'https://login.microsoftonline.com/e0793d39-0939-496d-b129-198edd916feb/oauth2/v2.0/token'
$gbl_ClientId = 'c63510ad-4d48-406e-9ad9-8f57b844d489'
$gbl_ClientSecret = 'ZlxIQGsyYmU5QVQheTZ6Mg=='
$gbl_GrantType = 'client_credentials'
$gbl_Scope = 'api://8dab47a5-36da-48a2-8a76-18a3a0ea73c6/.default'

# Endpoint configuration settings.
$gbl_ZeroTouchApi = 'https://ind-pub.steam.accenture.com/WebServices/ZeroTouchAPI/ScriptExecutionLog/Add'

# ---
# Function definition and initialization segment.
# ---

Function Write-LocalLog
{
    Param
    (
        [String] $prm_Source,
        [String] $prm_Description
    )

    Try
    {
        $fnc_DateTimeStamp = (Get-Date).ToString("yyyy-MM-dd hh:mm:ss")
        $fnc_FileName = (Get-Date).ToString("yyyyMMdd") + "_ZeroTouch.log"
        If (!(Test-Path "$gbl_LogPath"))
        {
            New-Item "$gbl_LogPath" -ItemType Directory | Out-Null
        }

        $fnc_LogEntry = @"
[$prm_Source] $fnc_DateTimeStamp :
$prm_Description
---
"@
        $fnc_LogEntry | Out-File "$gbl_LogPath\$fnc_FileName" -Append
        Return 0
    }

    Catch
    {
        Return 1
    }
}

Function Get-Token
{
    Try
    {
        $fnc_TokenHeader = @{client_id=$gbl_ClientId; client_secret=$gbl_ClientSecret; grant_type=$gbl_GrantType; scope=$gbl_Scope;}
        $fnc_Token = Invoke-RestMethod -URI $gbl_TokenUrl `
            -Method POST `
            -ContentType 'application/x-www-form-urlencoded' `
            -Body $fnc_TokenHeader
        
        Return $fnc_Token
    }

    Catch
    {
        Write-LocalLog -prm_Source "Get-Token" -prm_Description $Error[0] | Out-Null
        Return 1
    }
}

Function Send-Result
{
    Param
    (
        [Parameter(Mandatory=$true)] [System.Object] $prm_Token,
        [Int32] $prm_ScriptId,
        [String] $prm_ScriptName,
        [String] $prm_ExecutionStatus,
        [String] $prm_ComputerName,
        [String] $prm_Username,
        [String] $prm_ExecutionDate
    )

    Try
    {
        If($prm_Token.error)
        {
            Throw $prm_Token.error_description
        }
        Else
        {
            $fnc_AccessToken = $prm_Token.access_token
        }

        $fnc_Json = @"
{
    "ScriptId": $prm_ScriptId,
    "ScriptName": "$prm_ScriptName",
    "ExecutionStatus": "$prm_ExecutionStatus",
    "ComputerName": "$prm_ComputerName",
    "Username": "$prm_Username",
    "ExecutionDate": "$prm_ExecutionDate"
}
"@
        Write-Host "Request Payload:`n$fnc_Json" -ForegroundColor Green
        $fnc_Result = Invoke-RestMethod -Headers @{'Authorization'=("Bearer "+ $fnc_AccessToken)} `
            -Uri $gbl_ZeroTouchApi `
            -Method POST `
            -ContentType 'application/json' `
            -Body $fnc_Json

        If($fnc_Result.IsSuccessful -eq 'true')
        {
            Return 0
        }
        Else
        {
            Throw $fnc_Result.Message
        }
    }

    Catch
    {
        Write-LocalLog -prm_Source "Send-Result" -prm_Description $Error[0] | Out-Null
        Return 1
    }
}

# ---
# Script execution starts here, this is the part where you guys create your own logic.
# This template only shows how you can use the functions above to consume the ZeroTouch API.
# ---

# Reading the Log file generated today, adjust the logic as you please. Update line number 17 accordingly.
$gbl_LogExists = Test-Path "$gbl_OutputFilePath\$gbl_OutputFileName"
If($gbl_LogExists)
{
    $gbl_OutputFile = [System.IO.File]::ReadAllText("$gbl_OutputFilePath\$gbl_OutputFileName")
    $gbl_MatchCount = 0
    ForEach($gbl_Line In $gbl_OutputFile)
    {
        # Pattern matching, modify/update this as you deem neccessary.
        if($gbl_Line -like "*Success System Restart needed to fix the issue*")
        {
            $gbl_MatchCount += 1
        }
    }

    If($gbl_MatchCount -gt 0)
    {
        $gbl_Result = "Success"
    }
    Else
    {
        $gbl_Result = "Failed"   
    }
}
Else
{
    # If log file cannot be found.
    $gbl_Result = "Failed"
}

# Ensure to obtain token first (via 'Get-Token' function) prior to calling the 'Send-Result' function.
$gbl_ActiveToken = Get-Token

If($gbl_ActiveToken -eq 1)
{
    Write-LocalLog -prm_Source "Main" -prm_Description "Invalid token was received, please see log for more details." | Out-Null
}
Else
{
    Send-Result -prm_Token $gbl_ActiveToken `
        -prm_ScriptId 28 `
        -prm_ScriptName $($MyInvocation.MyCommand.Name) `
        -prm_ExecutionStatus $state_ZT `
        -prm_ComputerName (hostname.exe) `
        -prm_Username $($username) `
        -prm_ExecutionDate $((Get-Date).ToString("yyyy-MM-dd"))
}

# SIG # Begin signature block
# MIIauAYJKoZIhvcNAQcCoIIaqTCCGqUCAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCDIVHthLQETrHWN
# qit5L31KYtRjl2CVrKXu7lzfVzRC4KCCClwwggUkMIIEDKADAgECAhAJUXOVxQ83
# NghnLgqmfhY1MA0GCSqGSIb3DQEBCwUAMHIxCzAJBgNVBAYTAlVTMRUwEwYDVQQK
# EwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xMTAvBgNV
# BAMTKERpZ2lDZXJ0IFNIQTIgQXNzdXJlZCBJRCBDb2RlIFNpZ25pbmcgQ0EwHhcN
# MjEwMjExMDAwMDAwWhcNMjMwMjE1MjM1OTU5WjBiMQswCQYDVQQGEwJVUzERMA8G
# A1UECBMISUxMSU5PSVMxEDAOBgNVBAcTB0NoaWNhZ28xFjAUBgNVBAoTDUFjY2Vu
# dHVyZSBMTFAxFjAUBgNVBAMTDUFjY2VudHVyZSBMTFAwggEiMA0GCSqGSIb3DQEB
# AQUAA4IBDwAwggEKAoIBAQDT02mHpQRJPHQ7uOqGaJLw9/M8oORH8clDSMFbcCcp
# OvVXianm1LVp+xz1GbETKs1zUir0gToATw9zFA9jeKWGz5JYET29Jzo76hn8chg1
# 11Kk5PZRONN2nP7ObAU0eDJRN0feuSSHL+cGTaGyRpsS+N7uwycJqQbrg+Wj3ndS
# vHHr/afl7d/8W8sq5LNP2N/Z9eFxFdLWCQLPGv1lgiGLzZtjV0Ap0/nLr9KkFBIY
# 67fW28lIKD9vI0A7hFnKBuKikZny8cEH9+q7i9a9iC7+Qaoa78CXR/iQZ7IKcFxr
# iEm9lzyWu2y7+TE9eKDArBsKa+Udf67e4E+F8fx87169AgMBAAGjggHEMIIBwDAf
# BgNVHSMEGDAWgBRaxLl7KgqjpepxA8Bg+S32ZXUOWDAdBgNVHQ4EFgQUe/tKxC7/
# 593qj/IfDU4XxNCZ8WIwDgYDVR0PAQH/BAQDAgeAMBMGA1UdJQQMMAoGCCsGAQUF
# BwMDMHcGA1UdHwRwMG4wNaAzoDGGL2h0dHA6Ly9jcmwzLmRpZ2ljZXJ0LmNvbS9z
# aGEyLWFzc3VyZWQtY3MtZzEuY3JsMDWgM6Axhi9odHRwOi8vY3JsNC5kaWdpY2Vy
# dC5jb20vc2hhMi1hc3N1cmVkLWNzLWcxLmNybDBLBgNVHSAERDBCMDYGCWCGSAGG
# /WwDATApMCcGCCsGAQUFBwIBFhtodHRwOi8vd3d3LmRpZ2ljZXJ0LmNvbS9DUFMw
# CAYGZ4EMAQQBMIGEBggrBgEFBQcBAQR4MHYwJAYIKwYBBQUHMAGGGGh0dHA6Ly9v
# Y3NwLmRpZ2ljZXJ0LmNvbTBOBggrBgEFBQcwAoZCaHR0cDovL2NhY2VydHMuZGln
# aWNlcnQuY29tL0RpZ2lDZXJ0U0hBMkFzc3VyZWRJRENvZGVTaWduaW5nQ0EuY3J0
# MAwGA1UdEwEB/wQCMAAwDQYJKoZIhvcNAQELBQADggEBAGxNYqxcAni3Dd0MwptO
# JRFawZBdsdA8NS3kJHIGA8WkS41vyJZmEWBNae9ZolXZo9dgWR/tv76NOiflbH5T
# LCoG0Xdko/XhURBJ9Kc0txWnp0UaCHHQ8kCiEcyw/WWETxPEDscW+ml+5Vhmp1xu
# nvhsuWSbRxgnfqCpw+kmtFjnRWDPkV7VGQe2Bo4D4zQDEly/9QQXxKyh6eSJRYQj
# KxaIfZ5Nm+/QSkdo0KZGCgnIxOTW309xEWspzx6bPws3XDOqewfV1ZlXBnvKDtPw
# 2GaOZa1RnWA6lgPPbfHAox3LBVjg42ZzRa1p4RAPE+h3xqharORivwshLhvg3lMb
# DBEwggUwMIIEGKADAgECAhAECRgbX9W7ZnVTQ7VvlVAIMA0GCSqGSIb3DQEBCwUA
# MGUxCzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsT
# EHd3dy5kaWdpY2VydC5jb20xJDAiBgNVBAMTG0RpZ2lDZXJ0IEFzc3VyZWQgSUQg
# Um9vdCBDQTAeFw0xMzEwMjIxMjAwMDBaFw0yODEwMjIxMjAwMDBaMHIxCzAJBgNV
# BAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdp
# Y2VydC5jb20xMTAvBgNVBAMTKERpZ2lDZXJ0IFNIQTIgQXNzdXJlZCBJRCBDb2Rl
# IFNpZ25pbmcgQ0EwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQD407Mc
# fw4Rr2d3B9MLMUkZz9D7RZmxOttE9X/lqJ3bMtdx6nadBS63j/qSQ8Cl+YnUNxnX
# tqrwnIal2CWsDnkoOn7p0WfTxvspJ8fTeyOU5JEjlpB3gvmhhCNmElQzUHSxKCa7
# JGnCwlLyFGeKiUXULaGj6YgsIJWuHEqHCN8M9eJNYBi+qsSyrnAxZjNxPqxwoqvO
# f+l8y5Kh5TsxHM/q8grkV7tKtel05iv+bMt+dDk2DZDv5LVOpKnqagqrhPOsZ061
# xPeM0SAlI+sIZD5SlsHyDxL0xY4PwaLoLFH3c7y9hbFig3NBggfkOItqcyDQD2Rz
# PJ6fpjOp/RnfJZPRAgMBAAGjggHNMIIByTASBgNVHRMBAf8ECDAGAQH/AgEAMA4G
# A1UdDwEB/wQEAwIBhjATBgNVHSUEDDAKBggrBgEFBQcDAzB5BggrBgEFBQcBAQRt
# MGswJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRpZ2ljZXJ0LmNvbTBDBggrBgEF
# BQcwAoY3aHR0cDovL2NhY2VydHMuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0QXNzdXJl
# ZElEUm9vdENBLmNydDCBgQYDVR0fBHoweDA6oDigNoY0aHR0cDovL2NybDQuZGln
# aWNlcnQuY29tL0RpZ2lDZXJ0QXNzdXJlZElEUm9vdENBLmNybDA6oDigNoY0aHR0
# cDovL2NybDMuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0QXNzdXJlZElEUm9vdENBLmNy
# bDBPBgNVHSAESDBGMDgGCmCGSAGG/WwAAgQwKjAoBggrBgEFBQcCARYcaHR0cHM6
# Ly93d3cuZGlnaWNlcnQuY29tL0NQUzAKBghghkgBhv1sAzAdBgNVHQ4EFgQUWsS5
# eyoKo6XqcQPAYPkt9mV1DlgwHwYDVR0jBBgwFoAUReuir/SSy4IxLVGLp6chnfNt
# yA8wDQYJKoZIhvcNAQELBQADggEBAD7sDVoks/Mi0RXILHwlKXaoHV0cLToaxO8w
# Ydd+C2D9wz0PxK+L/e8q3yBVN7Dh9tGSdQ9RtG6ljlriXiSBThCk7j9xjmMOE0ut
# 119EefM2FAaK95xGTlz/kLEbBw6RFfu6r7VRwo0kriTGxycqoSkoGjpxKAI8LpGj
# wCUR4pwUR6F6aGivm6dcIFzZcbEMj7uo+MUSaJ/PQMtARKUT8OZkDCUIQjKyNook
# Av4vcn4c10lFluhZHen6dGRrsutmQ9qzsIzV6Q3d9gEgzpkxYz0IGhizgZtPxpMQ
# BvwHgfqL2vmCSfdibqFT+hKUGIUukpHqaGxEMrJmoecYpJpkUe8xgg+yMIIPrgIB
# ATCBhjByMQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYD
# VQQLExB3d3cuZGlnaWNlcnQuY29tMTEwLwYDVQQDEyhEaWdpQ2VydCBTSEEyIEFz
# c3VyZWQgSUQgQ29kZSBTaWduaW5nIENBAhAJUXOVxQ83NghnLgqmfhY1MA0GCWCG
# SAFlAwQCAQUAoHwwEAYKKwYBBAGCNwIBDDECMAAwGQYJKoZIhvcNAQkDMQwGCisG
# AQQBgjcCAQQwHAYKKwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARUwLwYJKoZIhvcN
# AQkEMSIEIG+Bkzae7Wpzk3i5K5POr0fN+zYrz+tuAzrOt8hISFUCMA0GCSqGSIb3
# DQEBAQUABIIBAM7fzLnNtxqMt8lvJc2ZL1qwablkmLH6TxriOwqw645DdXbPEloK
# qP9ChS8T72/k2rPetTxKo7T+MkKlb3Rz7BdUuPqC14qQ9hy821nvtAVhXs88AF+6
# OuZpAFhLgOc9Sw5Giic8WYM36YiIi3j0xGNHaEtK3jH8na1APb3aiW1XvER3dxBK
# wWzJ0jibDJS7yLdoZM5c3NH0GcJi3vrfaBbTCEmAL3JgywBmQCb+f/uUgDlkJcN7
# i7P6Jx3tDI+Qsrq/zRYlG0mik5Y++UIyCgYaorx7w2b/jLcSvraye7vu2rPMDvig
# Z0IY9uBx3OE4J8fzeXR/afHKTkfH3jwr51Ghgg1+MIINegYKKwYBBAGCNwMDATGC
# DWowgg1mBgkqhkiG9w0BBwKggg1XMIINUwIBAzEPMA0GCWCGSAFlAwQCAQUAMHgG
# CyqGSIb3DQEJEAEEoGkEZzBlAgEBBglghkgBhv1sBwEwMTANBglghkgBZQMEAgEF
# AAQgvza7lZ7Zx4031Dkn5nIy5yZM8TOAnvXNTx50vyIf3NgCEQDhx0nhkv+fdPhp
# C6dOatynGA8yMDIxMTIwNzExNDE0MVqgggo3MIIE/jCCA+agAwIBAgIQDUJK4L46
# iP9gQCHOFADw3TANBgkqhkiG9w0BAQsFADByMQswCQYDVQQGEwJVUzEVMBMGA1UE
# ChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3d3cuZGlnaWNlcnQuY29tMTEwLwYD
# VQQDEyhEaWdpQ2VydCBTSEEyIEFzc3VyZWQgSUQgVGltZXN0YW1waW5nIENBMB4X
# DTIxMDEwMTAwMDAwMFoXDTMxMDEwNjAwMDAwMFowSDELMAkGA1UEBhMCVVMxFzAV
# BgNVBAoTDkRpZ2lDZXJ0LCBJbmMuMSAwHgYDVQQDExdEaWdpQ2VydCBUaW1lc3Rh
# bXAgMjAyMTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAMLmYYRnxYr1
# DQikRcpja1HXOhFCvQp1dU2UtAxQtSYQ/h3Ib5FrDJbnGlxI70Tlv5thzRWRYlq4
# /2cLnGP9NmqB+in43Stwhd4CGPN4bbx9+cdtCT2+anaH6Yq9+IRdHnbJ5MZ2djpT
# 0dHTWjaPxqPhLxs6t2HWc+xObTOKfF1FLUuxUOZBOjdWhtyTI433UCXoZObd048v
# V7WHIOsOjizVI9r0TXhG4wODMSlKXAwxikqMiMX3MFr5FK8VX2xDSQn9JiNT9o1j
# 6BqrW7EdMMKbaYK02/xWVLwfoYervnpbCiAvSwnJlaeNsvrWY4tOpXIc7p96AXP4
# Gdb+DUmEvQECAwEAAaOCAbgwggG0MA4GA1UdDwEB/wQEAwIHgDAMBgNVHRMBAf8E
# AjAAMBYGA1UdJQEB/wQMMAoGCCsGAQUFBwMIMEEGA1UdIAQ6MDgwNgYJYIZIAYb9
# bAcBMCkwJwYIKwYBBQUHAgEWG2h0dHA6Ly93d3cuZGlnaWNlcnQuY29tL0NQUzAf
# BgNVHSMEGDAWgBT0tuEgHf4prtLkYaWyoiWyyBc1bjAdBgNVHQ4EFgQUNkSGjqS6
# sGa+vCgtHUQ23eNqerwwcQYDVR0fBGowaDAyoDCgLoYsaHR0cDovL2NybDMuZGln
# aWNlcnQuY29tL3NoYTItYXNzdXJlZC10cy5jcmwwMqAwoC6GLGh0dHA6Ly9jcmw0
# LmRpZ2ljZXJ0LmNvbS9zaGEyLWFzc3VyZWQtdHMuY3JsMIGFBggrBgEFBQcBAQR5
# MHcwJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRpZ2ljZXJ0LmNvbTBPBggrBgEF
# BQcwAoZDaHR0cDovL2NhY2VydHMuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0U0hBMkFz
# c3VyZWRJRFRpbWVzdGFtcGluZ0NBLmNydDANBgkqhkiG9w0BAQsFAAOCAQEASBzc
# temaI7znGucgDo5nRv1CclF0CiNHo6uS0iXEcFm+FKDlJ4GlTRQVGQd58NEEw4bZ
# O73+RAJmTe1ppA/2uHDPYuj1UUp4eTZ6J7fz51Kfk6ftQ55757TdQSKJ+4eiRgNO
# /PT+t2R3Y18jUmmDgvoaU+2QzI2hF3MN9PNlOXBL85zWenvaDLw9MtAby/Vh/HUI
# AHa8gQ74wOFcz8QRcucbZEnYIpp1FUL1LTI4gdr0YKK6tFL7XOBhJCVPst/JKahz
# Q1HavWPWH1ub9y4bTxMd90oNcX6Xt/Q/hOvB46NJofrOp79Wz7pZdmGJX36ntI5n
# ePk2mOHLKNpbh6aKLzCCBTEwggQZoAMCAQICEAqhJdbWMht+QeQF2jaXwhUwDQYJ
# KoZIhvcNAQELBQAwZTELMAkGA1UEBhMCVVMxFTATBgNVBAoTDERpZ2lDZXJ0IElu
# YzEZMBcGA1UECxMQd3d3LmRpZ2ljZXJ0LmNvbTEkMCIGA1UEAxMbRGlnaUNlcnQg
# QXNzdXJlZCBJRCBSb290IENBMB4XDTE2MDEwNzEyMDAwMFoXDTMxMDEwNzEyMDAw
# MFowcjELMAkGA1UEBhMCVVMxFTATBgNVBAoTDERpZ2lDZXJ0IEluYzEZMBcGA1UE
# CxMQd3d3LmRpZ2ljZXJ0LmNvbTExMC8GA1UEAxMoRGlnaUNlcnQgU0hBMiBBc3N1
# cmVkIElEIFRpbWVzdGFtcGluZyBDQTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCC
# AQoCggEBAL3QMu5LzY9/3am6gpnFOVQoV7YjSsQOB0UzURB90Pl9TWh+57ag9I2z
# iOSXv2MhkJi/E7xX08PhfgjWahQAOPcuHjvuzKb2Mln+X2U/4Jvr40ZHBhpVfgsn
# fsCi9aDg3iI/Dv9+lfvzo7oiPhisEeTwmQNtO4V8CdPuXciaC1TjqAlxa+DPIhAP
# dc9xck4Krd9AOly3UeGheRTGTSQjMF287DxgaqwvB8z98OpH2YhQXv1mblZhJymJ
# hFHmgudGUP2UKiyn5HU+upgPhH+fMRTWrdXyZMt7HgXQhBlyF/EXBu89zdZN7wZC
# /aJTKk+FHcQdPK/P2qwQ9d2srOlW/5MCAwEAAaOCAc4wggHKMB0GA1UdDgQWBBT0
# tuEgHf4prtLkYaWyoiWyyBc1bjAfBgNVHSMEGDAWgBRF66Kv9JLLgjEtUYunpyGd
# 823IDzASBgNVHRMBAf8ECDAGAQH/AgEAMA4GA1UdDwEB/wQEAwIBhjATBgNVHSUE
# DDAKBggrBgEFBQcDCDB5BggrBgEFBQcBAQRtMGswJAYIKwYBBQUHMAGGGGh0dHA6
# Ly9vY3NwLmRpZ2ljZXJ0LmNvbTBDBggrBgEFBQcwAoY3aHR0cDovL2NhY2VydHMu
# ZGlnaWNlcnQuY29tL0RpZ2lDZXJ0QXNzdXJlZElEUm9vdENBLmNydDCBgQYDVR0f
# BHoweDA6oDigNoY0aHR0cDovL2NybDQuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0QXNz
# dXJlZElEUm9vdENBLmNybDA6oDigNoY0aHR0cDovL2NybDMuZGlnaWNlcnQuY29t
# L0RpZ2lDZXJ0QXNzdXJlZElEUm9vdENBLmNybDBQBgNVHSAESTBHMDgGCmCGSAGG
# /WwAAgQwKjAoBggrBgEFBQcCARYcaHR0cHM6Ly93d3cuZGlnaWNlcnQuY29tL0NQ
# UzALBglghkgBhv1sBwEwDQYJKoZIhvcNAQELBQADggEBAHGVEulRh1Zpze/d2nyq
# Y3qzeM8GN0CE70uEv8rPAwL9xafDDiBCLK938ysfDCFaKrcFNB1qrpn4J6Jmvwmq
# YN92pDqTD/iy0dh8GWLoXoIlHsS6HHssIeLWWywUNUMEaLLbdQLgcseY1jxk5R9I
# EBhfiThhTWJGJIdjjJFSLK8pieV4H9YLFKWA1xJHcLN11ZOFk362kmf7U2GJqPVr
# lsD0WGkNfMgBsbkodbeZY4UijGHKeZR+WfyMD+NvtQEmtmyl7odRIeRYYJu6DC0r
# baLEfrvEJStHAgh8Sa4TtuF8QkIoxhhWz0E0tmZdtnR79VYzIi8iNrJLokqV2PWm
# jlIxggKGMIICggIBATCBhjByMQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNl
# cnQgSW5jMRkwFwYDVQQLExB3d3cuZGlnaWNlcnQuY29tMTEwLwYDVQQDEyhEaWdp
# Q2VydCBTSEEyIEFzc3VyZWQgSUQgVGltZXN0YW1waW5nIENBAhANQkrgvjqI/2BA
# Ic4UAPDdMA0GCWCGSAFlAwQCAQUAoIHRMBoGCSqGSIb3DQEJAzENBgsqhkiG9w0B
# CRABBDAcBgkqhkiG9w0BCQUxDxcNMjExMjA3MTE0MTQxWjArBgsqhkiG9w0BCRAC
# DDEcMBowGDAWBBTh14Ko4ZG+72vKFpG1qrSUpiSb8zAvBgkqhkiG9w0BCQQxIgQg
# 01kPVHeh76vsyph/o4G4SDMfEKtml5Qt2T/s1IxjdQIwNwYLKoZIhvcNAQkQAi8x
# KDAmMCQwIgQgsxCQBrwK2YMHkVcp4EQDQVyD4ykrYU8mlkyNNXHs9akwDQYJKoZI
# hvcNAQEBBQAEggEAd/6+jCa4Ho8ovyWr/KwSLqwpAe8tGY4ZmF4T72rQR1vjiW/m
# sC8uHW+jPN4vxKO31BdIGzP8Z1UCNOXIXBj0+Z1do9h74ogBKJvAXaKC/QHNwBfC
# +Qm8Bi5zDrC6JpGD0aQpZisWIGHnR0jqmON/MJXKOfXIow1i/fDPqV0WNWfTdo8K
# WFRM1P1pRasUJ1Gu4yLbVclgPFXGOt+aA7uDlFphXzLQ2QmOZ5LD1j/m8N99xjeX
# li2kemrQEM0tZxMDkewOyK7wD51GVH6hJsOi/NCUy4Th6zJWRXNsmnKHeTy9JPrK
# OieYypOb/kqT3TGSQydWIq+N2t9XzN9bFripkw==
# SIG # End signature block
