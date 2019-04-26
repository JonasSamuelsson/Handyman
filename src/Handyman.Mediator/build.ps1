[CmdletBinding(PositionalBinding = $false)]
param(
    [string]$artifacts = $null,
    [string]$buildCounter = $null,
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

if ($buildCounter) {
    exec GetVsProjectVersion -path "$this/src/Handyman.Mediator/Handyman.Mediator.csproj" | ForEach-Object { SetAdoBuildNumber -buildNumber "$_ #$buildCounter" }
}

exec dotnet build -c $configuration `
    "$this/src/Handyman.Mediator/Handyman.Mediator.csproj"

[string[]] $testArgs = @()

if ($ci -Or $env:TF_BUILD) {
    $testArgs += "--logger", "trx"
}

exec dotnet test -c $configuration `
    "$this/test/Handyman.Mediator.Tests/Handyman.Mediator.Tests.csproj" `
    $testArgs

exec dotnet pack --no-restore --no-build -c $configuration -o $artifacts --include-symbols "-p:SymbolPackageFormat=snupkg" `
    "$this/src/Handyman.Mediator/Handyman.Mediator.csproj"
    
write-host -f green 'script completed'
