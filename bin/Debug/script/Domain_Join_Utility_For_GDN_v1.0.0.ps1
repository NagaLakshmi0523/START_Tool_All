#This script will automate offline domain Join for GDN Images via webservice 
#Author  : sounak.biswas@accenture.com
#Usage   : powershell.exe -executionpolicy bypass -file <path to File.ps1>
#Note    : By default the Computer Name Field shall contain current computer Name and Default OU as ou=Staging,ou=Workstations,ou=GLOBAL
#          please review and change these values as required

#------------------------------------GUI Section Start------------------------------------------#
$result = $null

[void][System.Reflection.Assembly]::LoadWithPartialName( “System.Windows.Forms”)
[void][System.Reflection.Assembly]::LoadWithPartialName( “Microsoft.VisualBasic”)

$InputForm                       = New-Object system.Windows.Forms.Form
$InputForm.ClientSize            = '600,300'
$InputForm.text                  = "Domain Join Utility for GDN"
$InputForm.TopMost               = $false
$InputForm.StartPosition         = [System.Windows.Forms.FormStartPosition]::CenterScreen;  
$InputForm.ControlBox            = $false

$TextBox1                        = New-Object system.Windows.Forms.TextBox
$TextBox1.multiline              = $false
$TextBox1.width                  = 425
$TextBox1.height                 = 20
$TextBox1.location               = New-Object System.Drawing.Point(160,117)
$TextBox1.Font                   = 'Andalé Mono,8.5'

$TextBox2                        = New-Object system.Windows.Forms.TextBox
$TextBox2.multiline              = $false
$TextBox2.width                  = 425
$TextBox2.height                 = 20
$TextBox2.location               = New-Object System.Drawing.Point(160,145)
$TextBox2.Font                   = 'Andalé Mono,8.5'

$Label1                          = New-Object system.Windows.Forms.Label
$Label1.text                     = "Computer Name"
$Label1.AutoSize                 = $true
$Label1.width                    = 25
$Label1.height                   = 10
$Label1.location                 = New-Object System.Drawing.Point(20,120)
$Label1.Font                     = 'Andalé Mono,8.5'

$Label2                          = New-Object system.Windows.Forms.Label
$Label2.text                     = "Full OU Address"
$Label2.AutoSize                 = $true
$Label2.width                    = 25
$Label2.height                   = 10
$Label2.location                 = New-Object System.Drawing.Point(20,148)
$Label2.Font                     = 'Andalé Mono,8.5'

$Groupbox2                       = New-Object system.Windows.Forms.Groupbox
$Groupbox2.height                = 87
$Groupbox2.width                 = 570
$Groupbox2.text                  = "Note:"
$Groupbox2.location              = New-Object System.Drawing.Point(15,14)

$Label3                          = New-Object system.Windows.Forms.Label
$Label3.text                     = "Please enter GDN Computer Name and OU info and click `"OK`" to Proceed"
$Label3.AutoSize                 = $true
$Label3.width                    = 25
$Label3.height                   = 10
$Label3.location                 = New-Object System.Drawing.Point(16,56)
$Label3.Font                     = New-Object System.Drawing.Font('Microsoft Sans Serif',7.5)

$Label4                          = New-Object system.Windows.Forms.Label
$Label4.text                     = "Click `"Close`" to exit the application window."
$Label4.AutoSize                 = $true
$Label4.width                    = 25
$Label4.height                   = 10
$Label4.location                 = New-Object System.Drawing.Point(18,78)
$Label4.Font                     = New-Object System.Drawing.Font('Microsoft Sans Serif',7.5)

$Label5                          = New-Object system.Windows.Forms.Label
$Label5.text                     = "Domain Join Utility for GDN Images"
$Label5.AutoSize                 = $true
$Label5.width                    = 25
$Label5.height                   = 10
$Label5.location                 = New-Object System.Drawing.Point(17,35)
$Label5.Font                     = New-Object System.Drawing.Font('Microsoft Sans Serif',7.5)

$defaultValue = ""
$textBox1.Text = "computername";
$textBox2.Text = "ou=Staging,ou=Workstations,ou=GLOBAL";

$cmdOk = New-Object “System.Windows.Forms.Button”;
$cmdOk.Left  = 375;
$cmdOk.Top   = 180;
$cmdOk.Width = 100;
$cmdOk.Height = 30;
$cmdOk.Text  = “Ok”;
$cmdOk.DialogResult = [System.Windows.Forms.DialogResult]::Abort

$cmdClose = New-Object “System.Windows.Forms.Button”;
$cmdClose.Left  = 485;
$cmdClose.Top   = 180;
$cmdClose.Width = 100;
$cmdClose.Height = 30;
$cmdClose.Text  = “Close”;
$cmdClose.DialogResult = [System.Windows.Forms.DialogResult]::Cancel

$InputForm.CancelButton = $cmdClose

$InputForm.controls.AddRange(@($cmdOk,$cmdClose,$TextBox1,$TextBox2,$Label1,$Label2,$Label3,$Label4,$Label5))
$InputForm.controls.AddRange(@($Groupbox2))

$result = $InputForm.ShowDialog();

if ($result –eq [System.Windows.Forms.DialogResult]::Cancel)
    {
    write-Host 'User pressed cancel';
    $result = $null
    exit;
    }
elseif($result –eq [System.Windows.Forms.DialogResult]::Abort)
    {
    Write-Host 'User Pressed OK'
    }

#--------------------------------------------GUI Section End---------------------------------------
#--------------------------------------------Logging Function start--------------------------------
function WriteTo-Log{
    param (
            [string]$String="*",
            [string]$Logfile = $Logfile
          )

            if ($LogFile -eq "") 
                {
                $LogFile = ('.\'+(Get-History -Id ($MyInvocation.HistoryId -1) | select StartExecutionTime).startexecutiontime.tostring('yyyyMMdd-HHmm')+'-'+[io.path]::GetFileNameWithoutExtension($MyInvocation.ScriptName)+'.log')
                }

            if (!(Test-Path $LogFile)) 
               {

                 # Write-Output "Creating log file $LogFile"
                 $LogFile = New-Item $LogFile -Type file
               }

            $strDate = (Get-Date).ToString('MM-dd-yyyy') # HH:mm:ss')
            $strTime = (Get-Date).ToString('HH:mm:ss.sss') + "-330"
    
            if ($String -eq "")
                {
                $StringToWrite = "$String"
                }
            else

                {
                $StringToWrite = "<![LOG[$String]LOG]!><time=""$strTime"" date=""$strDate"" component=""GDN Domain Join"" context="""" type=""1"" thread=""8086"" file=""Domain_Join_Utility_For_GDN_v1.0.0.ps1"">"
                }

            Add-Content -Path $LogFile -Value $StringToWrite
}
#--------------------------------------------Logging Function End----------------------------------
#--------------------------------------------Logging Start-----------------------------------------
$strLogFilePath = “c:\windows\temp\domainJoin-GDN.log”
WriteTo-Log -String "*** Staring logging ***`r" -Logfile $strLogFilePath 
WriteTo-Log -String "Script version 1.0.0.2020.07.30 `r" -Logfile $strLogFilePath 
WriteTo-Log -String "Initializing input from user" -Logfile $strLogFilePath 

$Computer_name = $TextBox1.Text
$ou_name = $TextBox2.Text

<#
$Computer_name = M2C-L-F2811111
$ou_name = OU=BUILD,OU=STAGING,OU=B2C,OU=B2,OU=IND,OU=APAC
ou=Staging,ou=Workstations,ou=GLOBAL
#>

$postParams = @{GDNMachine='Yes';InputMachineName=$Computer_name;OUName=$ou_name}

try{
    
    WriteTo-Log -String "Invoking Web Request to https://cpwinxwebservices.accenture.com/GDNDomainJoinService/WebServiceAccess.asmx/ProvisionBlob" -Logfile $strLogFilePath 
    Write-Host "Invoking Web Request to https://cpwinxwebservices.accenture.com/GDNDomainJoinService/WebServiceAccess.asmx/ProvisionBlob"
    
    $Error.Clear
    Invoke-RestMethod -Uri "https://cpwinxwebservices.accenture.com/GDNDomainJoinService/WebServiceAccess.asmx/ProvisionBlob" -Method POST  -Body $postParams -UseDefaultCredentials -ErrorAction Stop

    WriteTo-Log -String "Invoking Web Request for downloading Text file" -Logfile $strLogFilePath 
    Write-Host "Invoking Web Request for downloading Text file"
    
    $Error.Clear
    Invoke-WebRequest -Uri "https://cpwinxwebservices.accenture.com/GDNDomainJoinService/ProvisionedMachines/$Computer_name.txt" -OutFile "C:\temp\$Computer_name.txt" -UseDefaultCredentials -ErrorAction Stop

    #WriteTo-Log -String "Initiating Domain Join DOS Command" -Logfile $strLogFilePath 
    #Write-Host "Initiating Domain Join DOS Command"
    
    #cmd /c djoin /requestODJ /loadfile "C:\Temp\$Computer_name.txt" /windowspath %SystemRoot% /localos

    WriteTo-Log -String "Script End" -Logfile $strLogFilePath 
    Write-Host "Script Ends"
    }
catch
    {

     Write-Error -Message "Fatal error: $Error"
     WriteTo-Log -String  "Fatal error: $Error `r" -Logfile $strLogFilePath

    }

