param(
    [switch] $Dogfood,
    [switch] $DryRun,
    [switch] $Integration,
    [switch] $NoClean,
    [switch] $NoTests,
    [switch] $Publish,
    [switch] $Release,
    [switch] $VerboseTests
)

$ErrorActionPreference = 'Stop'

# Paths
$NuGetConfigPath = ".\nuget.config"
$SrcRoot = "src"
$TestRoot = "tests"
$LocalPackages = ".\LocalPackages"
$Configuration = if ($Release -or $Publish -or $DryRun) { "Release" } else { "Debug" }
$treatWarningsAsErrors = if ($Configuration -eq "Release") { '/p:TreatWarningsAsErrors=true' } else { "" }

$ignoredProjects = @(
    'ProvisionData.WebApi.csproj'
)

#-----------------------------
# NuGet source configuration
#-----------------------------
[xml]$config = Get-Content -Raw -Path $NuGetConfigPath
$packageSources = $config.configuration.packageSources

function Set-Source {
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
    Set-Source "LocalPackages" $LocalPackages
}
elseif ($Release) {
    Write-Host "NuGet mode: Release (nuget.org only)" -ForegroundColor Cyan
    Remove-Source "LocalPackages"
    Set-Source "nuget.org" "https://api.nuget.org/v3/index.json"
}
else {
    Write-Host "NuGet mode: Hybrid (LocalPackages + nuget.org)" -ForegroundColor Cyan
    Set-Source "LocalPackages" $LocalPackages
    Set-Source "nuget.org" "https://api.nuget.org/v3/index.json"
}

$config.Save($NuGetConfigPath)

#-----------------------------
# Clean & build core projects
#-----------------------------
function Remove-BinObjFolders {
    [CmdletBinding(SupportsShouldProcess = $true)]
    param(
        [string]$Path = '.'
    )

    Get-ChildItem -Path $Path -Directory -Recurse -Force |
    Where-Object {
        ($_.Name -eq 'bin' -or $_.Name -eq 'obj') -and
        ($_.FullName -notmatch '\\node_modules\\')
    } |
    ForEach-Object {
        if ($PSCmdlet.ShouldProcess($_.FullName, 'Remove directory')) {
            Remove-Item -LiteralPath $_.FullName -Recurse -Force
        }
    }
}

if (-not $NoClean -or $Release -or $DryRun -or $Publish) {
    Write-Host "Cleaning projects..." -ForegroundColor Cyan
    Remove-BinObjFolders
}

Write-Host "Cleaning LocalPackages..." -ForegroundColor Cyan
if (Test-Path $LocalPackages) {
    Remove-Item -Force (Join-Path $LocalPackages "*.nupkg") -ErrorAction SilentlyContinue
    Remove-Item -Force (Join-Path $LocalPackages "*.snupkg") -ErrorAction SilentlyContinue
}
else {
    New-Item -ItemType Directory -Path $LocalPackages | Out-Null
}

function Copy-PackageOutputs {
    param(
        [string] $ProjectPath,
        [string] $Configuration,
        [string] $LocalPackages
    )

    $projectDir = Split-Path -Parent $ProjectPath
    $packageOutput = Join-Path $projectDir "bin\$Configuration"

    if (-not (Test-Path $packageOutput)) {
        Write-Host "Package output not found: $packageOutput" -ForegroundColor Yellow
        return
    }

    Get-ChildItem -Path $packageOutput -Filter *.nupkg -Recurse | ForEach-Object {
        Copy-Item -Force $_.FullName -Destination $LocalPackages
    }

    Get-ChildItem -Path $packageOutput -Filter *.snupkg -Recurse | ForEach-Object {
        Copy-Item -Force $_.FullName -Destination $LocalPackages
    }
}

#-----------------------------
# Discover all projects (except build first)
#-----------------------------
Write-Host "`nDiscovering projects to build..." -ForegroundColor Cyan

$projectsToBuild = Get-ChildItem -Path $SrcRoot, $TestRoot -Recurse -Filter *.csproj |
Where-Object { $_.Name -notin $ignoredProjects -and $_.Name -notmatch 'Tests' }

if ($projectsToBuild.Count -gt 0) {
    #-----------------------------
    # Build all discovered projects
    #-----------------------------
    if ($projectsToBuild.Count -gt 0) {
        Write-Host "`nBuilding projects..." -ForegroundColor Cyan

        foreach ($proj in $allProjects) {
            Write-Host "`nBuilding $($proj.FullName)..." -ForegroundColor Cyan
            dotnet build $proj.FullName
            if ($LASTEXITCODE -ne 0) {
                Write-Host "`nBuild failed for $($proj.FullName)!" -ForegroundColor Red
                exit $LASTEXITCODE
            }
        }
    }
}

#-----------------------------
# Discover packable projects
#-----------------------------
if ($Publish -or $DryRun) {
    Write-Host "`nDiscovering packable projects..." -ForegroundColor Cyan

    $packableProjects = Get-ChildItem -Path $SrcRoot -Recurse -Filter *.csproj |
    Where-Object {
        $xml = [xml](Get-Content -Raw -Path $_.FullName)
        $xml.Project.PropertyGroup.IsPackable -contains 'true'
    }
    
    $projectsToPack = $packableProjects | Where-Object { $_.Name -in $projectsToBuild }

    if ($packableProjects.Count -gt 0) {
        
        #-----------------------------
        # Pack all packable projects
        #-----------------------------
        foreach ($proj in $projectsToPack) {
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
        if ($projectsToPack) {
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
    }
}

if ($NoTests -and -not $Publish -and -not $DryRun) {
    Write-Host "`nSkipping tests as requested." -ForegroundColor Yellow
    exit 0
}

#-----------------------------
# Discover test projects
#-----------------------------
Write-Host "`nDiscovering test projects..." -ForegroundColor Cyan
$allTestProjects = Get-ChildItem -Path $TestRoot -Recurse -Filter *.csproj
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
if ($testsToRun.Count -gt 0) {

    Write-Host "`nBuilding test projects..." -ForegroundColor Cyan

    foreach ($proj in $testsToRun) {
        Write-Host "  Building $($proj.FullName)" -ForegroundColor Gray
        dotnet build $proj.FullName --no-incremental
        if ($LASTEXITCODE -ne 0) {
            Write-Host "`nTest project build failed!" -ForegroundColor Red
            exit $LASTEXITCODE
        }
    }

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
