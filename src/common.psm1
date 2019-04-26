function exec([string]$_cmd) {
    write-host -ForegroundColor DarkGray ">>> $_cmd $args"
    $ErrorActionPreference = 'Continue'
    & $_cmd @args
    $ErrorActionPreference = 'Stop'
    if ($LASTEXITCODE -And $LASTEXITCODE -ne 0) {
        write-error "Failed with exit code $LASTEXITCODE"
        exit 1
    }
}

function GetVsProjectVersion([string] $path) {
    $found = $false
    $pattern = '<Version>(.*)</Version>'

    foreach ($line in (Get-Content $path)) {
        if (($line -match $pattern)) {
            $found = $true
            Write-Output $matches[1]
            return
        }
    }

    if (!$found) {
        throw "Could not find version tag in " + $path
    }
}

function SetAdoBuildNumber([parameter(ValueFromPipeline)] [string] $buildNumber) { 
    Write-Host "##vso[build.updatebuildnumber]$buildNumber"
}