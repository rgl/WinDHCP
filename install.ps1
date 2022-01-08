param(
    [string]$networkInterface = 'vEthernet (Vagrant)',
    [string]$gateway = '192.168.192.1',
    [string]$startAddress = '192.168.1.100',
    [string]$endAddress = '192.168.1.250',
    [string]$subnet = '255.255.255.0',
    [string]$leaseDuration = '0.01:00:00',
    [string[]]$dnsServers = @('1.1.1.1', '1.0.0.1')
)

Set-StrictMode -Version Latest
$ProgressPreference = 'SilentlyContinue'
$ErrorActionPreference = 'Stop'
trap {
    Write-Output "ERROR: $_"
    Write-Output (($_.ScriptStackTrace -split '\r?\n') -replace '^(.*)$','ERROR: $1')
    Write-Output (($_.Exception.ToString() -split '\r?\n') -replace '^(.*)$','ERROR EXCEPTION: $1')
    Exit 1
}

# set the configuration.
Set-Content -Encoding UTF8 "$PSScriptRoot\WinDHCP.exe.config" @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="dhcpServer" type="WinDHCP.Library.Configuration.DhcpServerConfigurationSection, WinDHCP.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"/>
  </configSections>
  <dhcpServer networkInterface="$networkInterface" startAddress="$startAddress" endAddress="$endAddress" subnet="$subnet" gateway="$gateway" leaseDuration="$leaseDuration">
    <macAllowList>
      <add physicalAddress="*"/>
    </macAllowList>
    <macReservationList>
      <!--<add physicalAddress="ff:ff:ff:ff:ff:ff" ipAddress="192.168.192.100"/>-->
    </macReservationList>
    <dnsServers>
      $($dnsServers | ForEach-Object {"<add ipAddress=`"$_`"/>"})
    </dnsServers>
  </dhcpServer>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.8"/>
  </startup>
</configuration>
"@

# install the windows service.
$serviceName = 'WinDHCP'
$serviceUsername = "NT SERVICE\$serviceName"
if (Get-Service -ErrorAction SilentlyContinue $serviceName) {
    Stop-Service $serviceName
    $result = sc.exe delete $serviceName
    if ($result -ne '[SC] DeleteService SUCCESS') {
        throw "sc.exe delete failed with $result"
    }
}
$result = sc.exe create $serviceName start= auto binPath= "$PSScriptRoot\WinDHCP.exe"
if ($result -ne '[SC] CreateService SUCCESS') {
    throw "sc.exe sidtype failed with $result"
}
$result = sc.exe sidtype $serviceName unrestricted
if ($result -ne '[SC] ChangeServiceConfig2 SUCCESS') {
    throw "sc.exe sidtype failed with $result"
}
$result = sc.exe config $serviceName obj= $serviceUsername
if ($result -ne '[SC] ChangeServiceConfig SUCCESS') {
    throw "sc.exe config failed with $result"
}
$result = sc.exe failure $serviceName reset= 0 actions= restart/60000
if ($result -ne '[SC] ChangeServiceConfig2 SUCCESS') {
    throw "sc.exe failure failed with $result"
}

# start the windows service.
Start-Service $serviceName
