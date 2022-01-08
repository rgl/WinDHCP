# WinDHCP

[![Build status](https://github.com/rgl/WinDHCP/workflows/Build/badge.svg)](https://github.com/rgl/WinDHCP/actions?query=workflow%3ABuild)

A Windows DHCP Server Written in C#

## Overview

WinDHCP is windows service written in C#. It provides the basic DHCP functionality necessary to assign IP addresses on your LAN w/ subnet, gateway, and dns information. Currently it only processes DHCP Discover and Request messages, all others are ignored. WinDHCP was written using Visual Studio 2022 Community and has only been compiled and tested for .NET 4.8 on Windows 10. For more information on what DHCP is see http://en.wikipedia.org/wiki/DHCP.

## Motivation

Many of the cheap commodity routers currently available have very poor DHCP implementations. The freely available DHCP offerings on the internet primarily target Linux/Unix environments (ISC DHCP, for example, will not compile on Windows). WinDHCP makes it possible and easy to set up any Windows machine as a DHCP server.
