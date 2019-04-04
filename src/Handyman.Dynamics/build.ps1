[CmdletBinding(PositionalBinding = $false)]
param(
    [string]$artifacts = $null,
    [switch]$ci
)

$ErrorActionPreference = 'Stop'

$this = $PSScriptRoot
$root = "$this/.."
$configuration = "release"

if (!$artifacts) {
    $artifacts = "$this/.artifacts/"
    Remove-Item -Recurse $artifacts -ErrorAction Ignore
}

Import-Module -Force -Scope Local "$root/common.psm1"

exec dotnet build -c $configuration `
    "$this/src/Handyman.Dynamics/Handyman.Dynamics.csproj"

[string[]] $testArgs=@()

if ($ci) {
    $testArgs += "--logger", "trx"
}

exec dotnet test --configuration $configuration `
    "$this/test/Handyman.Dynamics.Tests/Handyman.Dynamics.Tests.csproj" `
    $testArgs

exec dotnet pack --no-restore --no-build -c $configuration -o $artifacts --include-symbols "-p:SymbolPackageFormat=snupkg" `
    "$this/src/Handyman.Dynamics/Handyman.Dynamics.csproj"
    
write-host -f green 'script completed'
