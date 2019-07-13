[CmdletBinding(PositionalBinding = $false)]
param(
    [string]$artifacts = $null,
    [string]$buildCounter = $null,
    [switch]$ci
)

$ErrorActionPreference = 'Stop'

Import-Module -Force -Scope Local "$PSScriptRoot/../../../build/common.psm1"

$root = "$PSScriptRoot/.."
$project = "$root/src/Handyman.AspNetCore.csproj"
$testProject = "$root/tests/Handyman.AspNetCore.Tests.csproj"

$expression = "exec BuildTestPack -project $project -testProject $testProject"

if ($artifacts) {
    $expression += " -artifacts $artifacts"
}

if ($buildCounter) {
    $expression += " -buildCounter $buildCounter"
}

invoke-expression $expression
    
write-host -f green 'script completed'