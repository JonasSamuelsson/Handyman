[CmdletBinding(PositionalBinding = $false)]
param(
    [string]$artifacts = $null,
    [string]$buildCounter = $null,
    [switch]$ci
)

$ErrorActionPreference = 'Stop'

Import-Module -Force -Scope Local "$PSScriptRoot/../../../build/common.psm1"

$root = "$PSScriptRoot/.."
$project = "$root/src/Handyman.Azure.Functions.csproj"

$expression = "exec BuildTestPack -project $project"

if ($artifacts) {
    $expression += " -artifacts $artifacts"
}

if ($buildCounter) {
    $expression += " -buildCounter $buildCounter"
}

invoke-expression $expression
    
write-host -f green 'script completed'