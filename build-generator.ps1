param(
    [switch]$Tests
)

# Build script for ErrorSourceGenerator development
# This ensures the generator is properly rebuilt and consuming projects pick up changes
Clear-Host

Write-Host "Cleaning projects..." -ForegroundColor Cyan
dotnet clean tests\ProvisionData.ResultPattern.UnitTests\ProvisionData.ResultPattern.UnitTests.csproj
dotnet clean src\ProvisionData.ResultPattern\ProvisionData.ResultPattern.csproj
dotnet clean src\ProvisionData.ResultPattern.CodeFixes\ProvisionData.ResultPattern.CodeFixes.csproj
dotnet clean src\ProvisionData.ResultPattern.Generators\ProvisionData.ResultPattern.Generators.csproj
dotnet clean src\ProvisionData.ResultPattern.Shared\ProvisionData.ResultPattern.Shared.csproj

Write-Host "Cleaning LocalPackages..." -ForegroundColor Cyan
Remove-Item -Force .\LocalPackages\*.nupkg

Write-Host "`nBuilding Generators project..." -ForegroundColor Cyan
dotnet build src\ProvisionData.ResultPattern.Generators\ProvisionData.ResultPattern.Generators.csproj
if ($LASTEXITCODE -ne 0) {
    Write-Host "`nGenerators build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`nBuilding CodeFixes project..." -ForegroundColor Cyan
dotnet build src\ProvisionData.ResultPattern.CodeFixes\ProvisionData.ResultPattern.CodeFixes.csproj
if ($LASTEXITCODE -ne 0) {
    Write-Host "`nCodeFixes build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`nBuilding ResultPattern project..." -ForegroundColor Cyan
dotnet build src\ProvisionData.ResultPattern\ProvisionData.ResultPattern.csproj --no-incremental
if ($LASTEXITCODE -ne 0) {
    Write-Host "`nResultPattern build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

dotnet build tests\ProvisionData.ResultPattern.UnitTests\ProvisionData.ResultPattern.UnitTests.csproj --no-incremental
if ($LASTEXITCODE -ne 0) {
    Write-Host "`nUnit tests build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`nBuild successful!" -ForegroundColor Green

if ($Tests) {
    Write-Host "`nBuilding UnitTests project..." -ForegroundColor Cyan
    dotnet test tests\ProvisionData.ResultPattern.UnitTests\ProvisionData.ResultPattern.UnitTests.csproj
    if ($LASTEXITCODE -ne 0) {
        Write-Host "`nOne or more tests failed!" -ForegroundColor Red
        exit $LASTEXITCODE
    }

    Write-Host "`nAll tests passed!" -ForegroundColor Green
}
else {
    Write-Host "`nYou can run the tests by using the -Tests switch." -ForegroundColor White
}
