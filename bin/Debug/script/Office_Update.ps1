   if($CmbChannel.Text -ne $nameDevMain){ 
        $build = "16.0."+(($CmbBuild.text -split "Build ")[1] -split "\)")[0] 
        & "C:\Program Files\Common Files\microsoft shared\ClickToRun\OfficeC2RClient.exe" /update user updatetoversion=$build} 
    
else{& "C:\Program Files\Common Files\microsoft shared\ClickToRun\OfficeC2RClient.exe" /update user} 
 
     if($ChkUpdate.Checked -eq $true){ 
        Write-Host "Disable updates......." 
        Start-Process powershell.exe -Verb runAs{ 
        Set-ItemProperty -Path Registry::HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\ClickToRun\Configuration -Name UpdatesEnabled -Value "False" 
        } 
 
    } 
         
{
$CmbChannel.add_SelectedIndexChanged($CmbChannel_SelectedIndexChanged) 
$BtnUpdate.add_Click($BtnUpdate_Click) 
$CmbBuild.add_SelectedIndexChanged($CmbBuild_SelectedIndexChanged) 
$Form.ShowDialog() 
} 
 
