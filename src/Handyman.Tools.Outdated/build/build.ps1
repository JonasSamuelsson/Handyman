[CmdletBinding(PositionalBinding = $false)]
param(
    [string]$artifacts = $null,
    [string]$buildCounter
)

$ErrorActionPreference = 'Stop'

Import-Module -Force -Scope Local "$PSScriptRoot/../../../build/common.psm1"

$project = "$PSScriptRoot/../src/Handyman.Tools.Outdated.csproj"

if (!$artifacts) {
    $artifacts = [System.IO.Path]::Combine($project, 'bin', '.artifacts')
}

exec BuildTestPack -artifacts $artifacts -buildCounter $buildCounter -project $project

write-host -f green 'script completed'
