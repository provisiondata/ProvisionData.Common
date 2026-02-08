# Build script for ErrorSourceGenerator development
# This ensures the generator is properly rebuilt and consuming projects pick up changes

Write-Host "Cleaning generator project..." -ForegroundColor Cyan
dotnet clean src\ProvisionData.ResultPattern.Generators\ProvisionData.ResultPattern.Generators.csproj

Write-Host "`nBuilding generator project..." -ForegroundColor Cyan
dotnet build src\ProvisionData.ResultPattern.Generators\ProvisionData.ResultPattern.Generators.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nGenerator build failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`nCleaning consuming projects..." -ForegroundColor Cyan
dotnet clean src\ProvisionData.ResultPattern\ProvisionData.ResultPattern.csproj

Write-Host "`nBuilding consuming projects with fresh generator..." -ForegroundColor Cyan
dotnet build src\ProvisionData.ResultPattern\ProvisionData.ResultPattern.csproj --no-incremental

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nBuild failed!" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`nBuild successful!" -ForegroundColor Green
Write-Host "To view generated files, run:" -ForegroundColor Yellow
Write-Host "dotnet build src\ProvisionData.ResultPattern\ProvisionData.ResultPattern.csproj /p:EmitCompilerGeneratedFiles=true /p:CompilerGeneratedFilesOutputPath=obj\GeneratedFiles" -ForegroundColor Yellow
