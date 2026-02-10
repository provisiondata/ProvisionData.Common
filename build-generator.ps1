param(
    [switch] $BuildTests,
    [switch] $RunTests,
    [switch] $Tests,
    [switch] $Publish,
    [switch] $DryRun,
    [switch] $Dogfood,
    [switch] $Release,
    [switch] $Integration,
    [switch] $VerboseTests,
    [switch] $NoClean
)

$ErrorActionPreference = 'Stop'

# Paths
$NuGetConfigPath = ".\nuget.config"
$SrcRoot = "src"
$TestRoot = "tests"
$LocalPackages = ".\LocalPackages"

$buildFirst = @(
    'ProvisionData.ResultPattern.Generators.csproj',
    'ProvisionData.ResultPattern.Generators.CodeFixes.csproj',
    'ProvisionData.ResultPattern.csproj'
)

$doNotBuild = @(
    'ProvisionData.ResultPattern.Generators.Shared.csproj',
    'ProvisionData.WebApi.csproj'
)

#-----------------------------
# NuGet source configuration
#-----------------------------
[xml]$config = Get-Content -Raw -Path $NuGetConfigPath
$packageSources = $config.configuration.packageSources

function Ensure-Source {
    param($Key, $Value)
    if (-not ($packageSources.add | Where-Object key -eq $Key)) {
        $new = $config.CreateElement("add")
        $new.SetAttribute("key", $Key)
        $new.SetAttribute("value", $Value)
        $packageSources.AppendChild($new) | Out-Null
    }
}

function Remove-Source {
    param($Key)
    $node = $packageSources.add | Where-Object key -eq $Key
    if ($node) {
        $packageSources.RemoveChild($node) | Out-Null
    }
}

if ($Dogfood) {
    Write-Host "NuGet mode: Dogfood (LocalPackages only)" -ForegroundColor Cyan
    Remove-Source "nuget.org"
    Ensure-Source "LocalPackages" $LocalPackages
}
elseif ($Release) {
    Write-Host "NuGet mode: Release (nuget.org only)" -ForegroundColor Cyan
    Remove-Source "LocalPackages"
    Ensure-Source "nuget.org" "https://api.nuget.org/v3/index.json"
}
else {
    Write-Host "NuGet mode: Hybrid (LocalPackages + nuget.org)" -ForegroundColor Cyan
    Ensure-Source "LocalPackages" $LocalPackages
    Ensure-Source "nuget.org" "https://api.nuget.org/v3/index.json"
}

$config.Save($NuGetConfigPath)

#-----------------------------
# Clean & build core projects
#-----------------------------
if (-not $NoClean -or $Release) {
    Write-Host "Cleaning projects..." -ForegroundColor Cyan
    dotnet clean
}

Write-Host "Cleaning LocalPackages..." -ForegroundColor Cyan
if (Test-Path $LocalPackages) {
    Remove-Item -Force (Join-Path $LocalPackages "*.nupkg") -ErrorAction SilentlyContinue
}
else {
    New-Item -ItemType Directory -Path $LocalPackages | Out-Null
}

ForEach ($projName in $buildFirst) {
    $projPath = Get-ChildItem -Path $SrcRoot -Recurse -Filter $projName | Select-Object -First 1
    if ($projPath) {
        Write-Host "`nBuilding $($projPath.FullName)..." -ForegroundColor Cyan
        dotnet build $projPath.FullName --no-incremental
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
    else {
        Write-Host "Project $projName not found!" -ForegroundColor Red
        exit 1
    }
}

Write-Host "`nPacking ProvisionData.ResultPattern..." -ForegroundColor Cyan
dotnet pack src\ProvisionData.ResultPattern\ProvisionData.ResultPattern.csproj -c Debug -o $LocalPackages

# Write-Host "`nBuilding Generators project..." -ForegroundColor Cyan
# dotnet build src\ProvisionData.ResultPattern.Generators\ProvisionData.ResultPattern.Generators.csproj
# if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
# 
# Write-Host "`nBuilding CodeFixes project..." -ForegroundColor Cyan
# dotnet build src\ProvisionData.ResultPattern.Generators.CodeFixes\ProvisionData.ResultPattern.Generators.CodeFixes.csproj
# if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
# 
# Write-Host "`nBuilding ResultPattern project..." -ForegroundColor Cyan
# dotnet build src\ProvisionData.ResultPattern\ProvisionData.ResultPattern.csproj --no-incremental
# if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

#-----------------------------
# Discover all projects (except core generators)
#-----------------------------
Write-Host "`nDiscovering all projects to build..." -ForegroundColor Cyan

$allProjects = Get-ChildItem -Path $SrcRoot, $TestRoot -Recurse -Filter *.csproj |
Where-Object { $_.Name -notin $buildFirst -and $_.Name -notin $doNotBuild }

if (-not $allProjects) {
    Write-Host "No projects found to build." -ForegroundColor Yellow
}
else {
    Write-Host "Projects to build:" -ForegroundColor Cyan
    $allProjects | ForEach-Object { Write-Host "  - $($_.FullName)" -ForegroundColor Gray }
}

#-----------------------------
# Build all discovered projects
#-----------------------------
foreach ($proj in $allProjects) {
    Write-Host "`nBuilding $($proj.FullName)..." -ForegroundColor Cyan
    dotnet build $proj.FullName
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`nBuild failed for $($proj.FullName)!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
}

#-----------------------------
# Discover packable projects
#-----------------------------
Write-Host "`nDiscovering packable projects..." -ForegroundColor Cyan

$packableProjects = Get-ChildItem -Path $SrcRoot -Recurse -Filter *.csproj |
Where-Object {
    $xml = [xml](Get-Content -Raw -Path $_.FullName)
    $xml.Project.PropertyGroup.IsPackable -contains 'true'
}

if (-not $packableProjects) {
    Write-Host "No packable projects found (IsPackable=true)." -ForegroundColor Yellow
}
else {
    Write-Host "Packable projects:" -ForegroundColor Cyan
    $packableProjects | ForEach-Object { Write-Host "  - $($_.FullName)" -ForegroundColor Gray }
}

#-----------------------------
# Pack all packable projects
#-----------------------------
foreach ($proj in $packableProjects) {
    Write-Host "`nPacking $($proj.FullName)..." -ForegroundColor Cyan
    dotnet pack $proj.FullName -c Debug -o $LocalPackages
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`nPack failed for $($proj.FullName)!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
}

#-----------------------------
# Embed checksum/provenance banner
#-----------------------------
if ($packableProjects) {
    Write-Host "`nEmbedding checksum/provenance banners..." -ForegroundColor Cyan

    Add-Type -AssemblyName System.IO.Compression.FileSystem

    Get-ChildItem $LocalPackages -Filter *.nupkg | ForEach-Object {
        $pkg = $_
        $checksum = Get-FileHash $pkg.FullName -Algorithm SHA256
        $gitHash = (git rev-parse HEAD)

        $banner = @"
ProvisionData — Build Provenance
Git Hash: $gitHash
Package: $($pkg.Name)
SHA256: $($checksum.Hash)
Timestamp: $(Get-Date -Format o)
"@

        $zip = [System.IO.Compression.ZipFile]::Open($pkg.FullName, 'Update')
        try {
            $existing = $zip.GetEntry("CHECKSUM.txt")
            if ($existing) { $existing.Delete() }

            $entry = $zip.CreateEntry("CHECKSUM.txt")
            $stream = $entry.Open()
            $writer = New-Object System.IO.StreamWriter($stream)
            $writer.Write($banner)
            $writer.Dispose()
        }
        finally {
            $zip.Dispose()
        }
    }
}

#-----------------------------
# Discover test projects
#-----------------------------
$allTestProjects = Get-ChildItem -Path $TestRoot -Recurse -Filter *.csproj

$integrationTests = $allTestProjects | Where-Object { $_.Name -match "Integration" }
$unitTests = $allTestProjects | Where-Object { $_.Name -notmatch "Integration" }

$shouldRunIntegration = $Integration -or $DryRun -or $Publish

$testsToRun = if ($shouldRunIntegration) { $allTestProjects } else { $unitTests }

#-----------------------------
# Test runner helper
#-----------------------------
function Invoke-TestProject {
    param(
        [string] $ProjectPath,
        [switch] $Verbose
    )

    if ($Verbose) {
        dotnet test $ProjectPath --no-build
    }
    else {
        dotnet test $ProjectPath --no-build --logger "console;verbosity=minimal"
    }

    if ($LASTEXITCODE -ne 0) {
        Write-Host "`nTest failure in $ProjectPath" -ForegroundColor Red
        exit $LASTEXITCODE
    }
}

#-----------------------------
# Build & run tests
#-----------------------------
if ($BuildTests -or $RunTests -or $Tests) {
    Write-Host "`nBuilding test projects..." -ForegroundColor Cyan

    foreach ($proj in $testsToRun) {
        Write-Host "  Building $($proj.FullName)" -ForegroundColor Gray
        dotnet build $proj.FullName --no-incremental
        if ($LASTEXITCODE -ne 0) {
            Write-Host "`nTest project build failed!" -ForegroundColor Red
            exit $LASTEXITCODE
        }
    }
}

if ($RunTests -or $Tests) {
    Write-Host "`nRunning tests..." -ForegroundColor Cyan

    foreach ($proj in $testsToRun) {
        Write-Host "  Testing $($proj.FullName)" -ForegroundColor Gray
        Invoke-TestProject -ProjectPath $proj.FullName -Verbose:$VerboseTests
    }

    Write-Host "`nAll tests passed!" -ForegroundColor Green
}

#-----------------------------
# Publish / DryRun
#-----------------------------
if ($Publish -or $DryRun) {
    Write-Host "`nPreparing release artifacts..." -ForegroundColor Cyan

    if (-not $env:NUGET_API_KEY -and -not $DryRun) {
        Write-Host "NUGET_API_KEY environment variable is missing." -ForegroundColor Red
        exit 1
    }

    $certThumbprint = $env:NUGET_CERT_THUMBPRINT
    if ($certThumbprint) {
        Write-Host "Signing packages..." -ForegroundColor Cyan

        Get-ChildItem $LocalPackages -Filter *.nupkg | ForEach-Object {
            nuget sign $_.FullName `
                -CertificateFingerprint $certThumbprint `
                -Timestamper "http://timestamp.digicert.com"
        }
    }
    else {
        Write-Host "Skipping signing (no NUGET_CERT_THUMBPRINT set)." -ForegroundColor Yellow
    }

    if ($DryRun) {
        Write-Host "`nDry run complete. Packages were built, signed (if configured), and validated, but NOT pushed." -ForegroundColor Yellow
        exit 0
    }

    Write-Host "Publishing packages to nuget.org..." -ForegroundColor Cyan

    dotnet nuget push (Join-Path $LocalPackages "*.nupkg") `
        --api-key $env:NUGET_API_KEY `
        --source https://api.nuget.org/v3/index.json `
        --skip-duplicate

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Publishing failed!" -ForegroundColor Red
        exit $LASTEXITCODE
    }

    Write-Host "`nPublish complete!" -ForegroundColor Green
}
