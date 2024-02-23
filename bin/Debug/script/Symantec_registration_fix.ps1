
#Symantec Authentication issue or Unable to accenture sites and struck with symatec registration
#Description- check java compliant if not compliant install the latest java , Reset network socket, Reset IE and Chrome, Remove plugin installed if already installed in current logged in user profile
#Created on : 15/04/2020
#Last Modified : 05/05/2020
#Created by ISA.India.automationteam

Write-Host "Symantec_registration_fix script is executed, please wait to get completed status" -ForegroundColor Green

#Registry Path
$PATHS = @("HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
           "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall")
           
$productspath = "HKCR:\Installer\Products\*"
$Java_Cversion = "8.0.2510.8"
$javaname = "Java 8 Update*"
$PATH1 = "HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
$PATH2 = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
$Logfile = "C:\symantecFix_$env:COMPUTERNAME.log"
if (Test-Path $Logfile) { Remove-Item -Path $Logfile -Force -Confirm:$false}
           function Get-TimeStamp
{
    return "[{0:MM/dd/yy} {0:HH:mm:ss}]" -f (Get-Date) 
    }
#JAVA Check
  if (((Get-ChildItem -Path $PATH1 | ForEach { Get-ItemProperty $_.PSPath } | Where-Object { $_.Displayversion -match "8.0.2510.8" }) -eq $null ) -and ((Get-ChildItem -Path $PATH2 | ForEach { Get-ItemProperty $_.PSPath } | Where-Object { $_.Displayversion -match "8.0.2510.7" }) -eq $null)) {
            
# working directory path
$workd = "c:\Windows\temp\Oracle" 

# Check if work directory exists if not create it
If (!(Test-Path -Path $workd -PathType Container))
{ 
New-Item -Path $workd  -ItemType directory 
} 

#create config file for silent install
$text = '
INSTALL_SILENT=Enable
AUTO_UPDATE=Enable
SPONSORS=Disable
REMOVEOUTOFDATEJRES=1
'
$text | Set-Content "$workd\jreinstall.cfg"
    
#download executable, this is the small online installer
$javafile = [pscustomobject]@{
    JavaVersion = ''
    FileName = ''
    DownloadURL =  $((Invoke-WebRequest 'http://www.java.com/en/download/manual.jsp').links | where innerHTML -like "Windows Offline" | select href).href
} 
$source = $JavaFile.DownloadURL
$destination = "$workd\jreInstall.exe"
$client = New-Object System.Net.WebClient
$client.DownloadFile($source, $destination) 

#install silently
Start-Process -FilePath "$workd\jreInstall.exe" -ArgumentList INSTALLCFG="$workd\jreinstall.cfg" 

# Wait 120 Seconds for the installation to finish
Start-Sleep -s 180 

# Remove the installer
rm -Force $workd\jre* -ErrorAction SilentlyContinue
    
}
    ForEach ($path in $PATHS) {
        #Get installed Java versions
        $Javainstalled = Get-ChildItem -Path $path | ForEach { Get-ItemProperty $_.PSPath } | Where-Object { $_.DisplayName -match "$javaname" } | Select-Object -Property DisplayName,DisplayVersion,UninstallString,pschildname
        ForEach ( $java in $Javainstalled ) {
            if ( $java.DisplayVersion -ge $Java_Cversion ) {
                Write-Host "JAVA is Compliant" -ForegroundColor Green
"$(Get-TimeStamp) JAVA is Compliant" | Out-File $Logfile -Append

                $javaversion = $java.DisplayVersion
                Add-Content $Logfile "Java is compliant version : $javaversion"
            } else {
                #Remove Java From registry
                $childname = $java.pschildname
                Remove-Item -Path "$path\$childname" -Confirm:$false -Recurse
                $javadisplayname = $java.DisplayName
                $javaproducts = Get-ItemProperty -Path $productspath | ForEach { Get-ItemProperty $_.PSPath } | Where-Object { $_.ProductName -match "$javadisplayname" } | Select-Object -Property ProductName,pschildname
                $productchild = $javaproducts.pschildname
                Remove-Item -Path "HKCR:\Installer\Products\$productchild" -Confirm:$false -Recurse -ErrorAction SilentlyContinue
                Add-Content $Logfile "Java older version has been removed $javadisplayname"
            }
        }
    }

    #Symantec authentication pluguin issue 

    #get juniper network service and disbale

    C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe Get-NetAdapterBinding -Name * -DisplayName "Juniper Network Service" -ErrorAction SilentlyContinue

    C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe disable-NetAdapterBinding -Name * -DisplayName "Juniper Network Service" -ErrorAction SilentlyContinue

    #flush DNS
    ipconfig /flushdns 
    #register DNS
    ipconfig /registerdns 

    #network IP reset
    netsh int ip reset 

    #windows sock reset
    netsh winsock reset 

    arp -d *

    nbtstat -R

    nbtstat -RR

    ipconfig /release
    ipconfig /renew


  write-host "Network reset and DNS flush done" -ForegroundColor Green
"$(Get-TimeStamp) Network reset and DNS flush done" | Out-File $Logfile -Append

    #close IE
    
     get-process -name iexplore | stop-process -ErrorAction SilentlyContinue
     
        "`n"
Write-Host  "  ============================================" -ForegroundColor 'Green'
Write-Host  "   [ Resetting IE To Default Settings]" -ForegroundColor 'Red' 
Write-Host  "  ============================================" -ForegroundColor 'Green'
"`n"
### 
"`n"

$arrOfficeProcs = "iexplore"
$continue = $false
cls

#Check for open Office apps
do {
    $arrRunning = @()
    
    foreach ($proc in $arrofficeProcs) {
        if(((get-process $proc -ea SilentlyContinue) -ne $Null)){ $arrRunning += $proc }       
    }
        
    if ($arrRunning.length -gt 0 ) {  
        $d = [System.Windows.Forms.MessageBox]::Show(
        "There are currently one or more Internet Explorer windows Open.`n`nYou must close down all Internet explorer windows before reset it to default.", 
        "Reset IE Settings to Default...", 
        [System.Windows.Forms.MessageBoxButtons]::RetryCancel, 
        [System.Windows.Forms.MessageBoxIcon]::Warning )

        if ($d -eq [Windows.Forms.DialogResult]::Cancel) { exit }

    } else { 
        $continue = $true
        write-host "  No IE process are currently running"  -ForegroundColor 'Green'
		"`n"
		Write-Host "  Please TICK on `"Delete personal Settings`" and then click on `"Reset`" button   <====" -ForegroundColor 'Yellow' 
		"`n"
		& RunDll32.exe InetCpl.cpl,ResetIEtoDefaults | Out-Null
		"`n"
		Write-Host "  ====> Please Launch Internet Explorer Now" -ForegroundColor 'Magenta' 
		
"`n"
Write-Host  "  ============================================" -ForegroundColor 'Green'
Write-Host  "   [ Resetting Done]" -ForegroundColor 'Red' 
Write-Host  "  ============================================" -ForegroundColor 'Green'
"`n"
"$(Get-TimeStamp) Internet explorer resetting is done $env:Computername" | Out-File $logfile -Append 
		}

} while ( $continue -eq $false )

# Get current user
$CurrentUser = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
# Set the variable to the first string before the "\" character
$CurrentDomainName = $CurrentUser.split("\")[0]
# Set the variable to the second string after the "\" character
$CurrentUserName = $CurrentUser.split("\")[1]
#Close chrome , reset chrome and delete existing installed symantec plugin
 get-process -name chrome | stop-process -ErrorAction SilentlyContinue
Remove-Item –path "C:\Users\$CurrentUserName\AppData\Local\Google\Chrome" –recurse -force -ErrorAction SilentlyContinue
Remove-Item –path "C:\Users\$CurrentUserName\AppData\Local\AuthClient-4-VIP" –recurse -force -ErrorAction SilentlyContinue
Remove-Item –path "C:\Users\$CurrentUserName\AppData\LocalLow\AuthClient-4-VIP" –recurse -force -ErrorAction SilentlyContinue

  write-host "Chrome restting done and removed symantec plugin" -ForegroundColor Green
"$(Get-TimeStamp) Chrome restting done and removed symantec plugin " | Out-File $Logfile -Append

Invoke-Item -Path "C:\Symantecfix_$env:COMPUTERNAME.log"

$wshell = New-Object -ComObject Wscript.Shell 
$Output = $wshell.Popup("Script finished and system will restart in two minute")
notepad.exe $Logfile
Start-Sleep 120 Restart-Computer
