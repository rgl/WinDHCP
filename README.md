# About

[![Build status](https://github.com/rgl/WinDHCP/workflows/Build/badge.svg)](https://github.com/rgl/WinDHCP/actions?query=workflow%3ABuild)

This is an incomplete DHCP Server that can run in any Windows edition.

It only supports DHCPv4 Discover/Request messages on Ethernet networks.

For more information about DHCP see http://en.wikipedia.org/wiki/DHCP.

## Motivation

This was put together because the Hyper-V Default Switch comes with a DHCP service (managed by the Internet Connection Sharing service aka ICS) that cannot be configured and does not play well with Packer/Vagrant and VMs reboots.

This can be used as a DHCP server for the Packer/Vagrant VM images at [rgl/debian-vagrant](https://github.com/rgl/debian-vagrant), [rgl/ubuntu-vagrant](https://github.com/rgl/ubuntu-vagrant), and [rgl/windows-vagrant](https://github.com/rgl/windows-vagrant).

