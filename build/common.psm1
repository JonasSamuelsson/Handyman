function exec([string]$_cmd) {
    write-host -ForegroundColor DarkGray ">>> $_cmd $args"
    $ErrorActionPreference = 'Continue'
    & $_cmd @args
    $success = $?
    $ErrorActionPreference = 'Stop'
    if ($LASTEXITCODE -And $LASTEXITCODE -ne 0) {
        write-error "Failed with exit code $LASTEXITCODE"
        exit $LASTEXITCODE
    }
    if (!$success) {
        write-error "Execution failed."
        exit 1
    }
}

function BuildTestPack {
    param(
        [Parameter(Mandatory)][string] $project, 
        [string] $artifacts = $null, 
        [string] $buildCounter = $null,
        [string] $testProject = $null
    )

    if (![System.IO.File]::Exists("$project")) {
        throw "Project '$project' not found."
    }
    
    if (!$artifacts) {
        $artifacts = "$PSScriptRoot/../.artifacts/"
        Remove-Item -Recurse $artifacts -ErrorAction Ignore
    }

    if ($buildCounter) {
        exec GetVsProjectVersion -path $project | ForEach-Object { SetAdoBuildNumber -buildNumber "$_ #$buildCounter" }
    }

    exec dotnet build -c "release" $project
    
    if ($testProject) {
        exec dotnet test -c "release" $testProject "--logger" "trx"
    }
    
    exec dotnet pack --no-restore --no-build -c "release" -o $artifacts --include-symbols "-p:SymbolPackageFormat=snupkg" $project
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