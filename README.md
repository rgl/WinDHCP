# About

[![Build status](https://github.com/rgl/WinDHCP/workflows/Build/badge.svg)](https://github.com/rgl/WinDHCP/actions?query=workflow%3ABuild)

This is an incomplete DHCP Server that can run in any Windows edition.

It only supports DHCPv4 Discover/Request messages on Ethernet networks.

For more information about DHCP see http://en.wikipedia.org/wiki/DHCP.

## Motivation

This was put together because the Hyper-V Default Switch comes with a DHCP service (managed by the Internet Connection Sharing service aka ICS) that cannot be configured and does not play well with Packer/Vagrant and VMs reboots.

This can be used as a DHCP server for the Packer/Vagrant at [rgl/infra-toolbox](https://github.com/rgl/infra-toolbox), [rgl/debian-vagrant](https://github.com/rgl/debian-vagrant), [rgl/ubuntu-vagrant](https://github.com/rgl/ubuntu-vagrant), and [rgl/windows-vagrant](https://github.com/rgl/windows-vagrant).

## Service Installation

In a elevated PowerShell session install the latest release of WinDHCP with, for example:

```powershell
$releasesUrl = 'https://github.com/rgl/WinDHCP/releases'
$latestResponse = Invoke-RestMethod -Headers @{Accept='application/json'} "$releasesUrl/latest"
$latestTag = $latestResponse.tag_name
$artifactUrl = "$releasesUrl/download/$latestTag/WinDHCP.zip"
$artifactPath = "$env:TEMP\WinDHCP.zip"
$installPath = "$env:ProgramFiles\WinDHCP"
Add-Type -AssemblyName System.IO.Compression.FileSystem
(New-Object Net.WebClient).DownloadFile($artifactUrl, $artifactPath)
[System.IO.Compression.ZipFile]::ExtractToDirectory($artifactPath, $installPath)
&"$installPath\install.ps1" `
    -networkInterface 'vEthernet (Vagrant)' `
    -gateway '192.168.192.1' `
    -startAddress '192.168.192.100' `
    -endAddress '192.168.192.250' `
    -subnet '255.255.255.0' `
    -leaseDuration '0.01:00:00' `
    -dnsServers @('1.1.1.1', '1.0.0.1')
```
