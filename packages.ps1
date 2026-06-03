<#
.SYNOPSIS
    Builds and/or publishes every LocalizationProvider NuGet package
    (common, aspnetcore, optimizely) from the solution root.

.DESCRIPTION
    Replaces the per-area build-packages.ps1 / push-packages.ps1 scripts.
    All packages are packed into the root .nuget folder and pushed from there.

    Symbol packages (.snupkg) are produced for every package in a single Release
    pack step (each project sets IncludeSymbols=true + SymbolPackageFormat=snupkg).
    dotnet nuget push then uploads the .snupkg alongside the .nupkg to feeds that
    support a symbol server (e.g. nuget.org); feeds without one simply skip it.

.PARAMETER Build
    Pack the selected areas into .nuget. This is the default when no switch is given.

.PARAMETER Push
    Push the packed packages from .nuget to their feed. Must be requested explicitly.

.PARAMETER Area
    Limit the run to one or more areas: common, aspnetcore, optimizely. Default: all.

.PARAMETER Version
    Package version to pack/push. Must match <PackageVersion> in the csproj files. Default: 9.0.0.

.PARAMETER ApiKey
    NuGet API key used by -Push. If omitted, the feed's pre-configured credentials are used.

.EXAMPLE
    ./packages.ps1
    Builds all packages into .nuget (no push).

.EXAMPLE
    ./packages.ps1 -Build -Push -ApiKey $env:NUGET_KEY
    Builds everything, then publishes it.

.EXAMPLE
    ./packages.ps1 -Push -Area common -ApiKey $env:NUGET_KEY
    Publishes only the already-built common packages.
#>
[CmdletBinding()]
param(
    [switch]$Build,
    [switch]$Push,
    [ValidateSet('common', 'aspnetcore', 'optimizely')]
    [string[]]$Area,
    [string]$Version = '9.0.0',
    [string]$ApiKey
)

$ErrorActionPreference = 'Stop'

$nugetOrg  = 'https://api.nuget.org/v3/index.json'
$episerver = 'https://nuget.episerver.com/feed/packages.svc'

# Each area declares its feed, whether symbol packages are produced, and the
# projects to pack/push as (source folder under <area>/src) -> (NuGet PackageId).
# Push iterates these same lists, so the package set always matches what is built.
$areas = @(
    [pscustomobject]@{
        Name     = 'common'
        Feed     = $nugetOrg
        Projects = @(
            @{ Project = 'DbLocalizationProvider.Abstractions';        Id = 'LocalizationProvider.Abstractions' }
            @{ Project = 'DbLocalizationProvider';                     Id = 'LocalizationProvider' }
            @{ Project = 'DbLocalizationProvider.AdminUI.Models';      Id = 'LocalizationProvider.AdminUI.Models' }
            @{ Project = 'DbLocalizationProvider.Csv';                 Id = 'LocalizationProvider.Csv' }
            @{ Project = 'DbLocalizationProvider.Xliff';               Id = 'LocalizationProvider.Xliff' }
            @{ Project = 'DbLocalizationProvider.Storage.SqlServer';   Id = 'LocalizationProvider.Storage.SqlServer' }
            @{ Project = 'DbLocalizationProvider.Storage.PostgreSQL';  Id = 'LocalizationProvider.Storage.PostgreSql' }
            @{ Project = 'DbLocalizationProvider.Storage.MongoDb';     Id = 'LocalizationProvider.Storage.MongoDb' }
            @{ Project = 'DbLocalizationProvider.Storage.AzureTables'; Id = 'LocalizationProvider.Storage.AzureTables' }
            @{ Project = 'DbLocalizationProvider.Translator.Azure';    Id = 'LocalizationProvider.Translator.Azure' }
        )
    }
    [pscustomobject]@{
        Name     = 'aspnetcore'
        Feed     = $nugetOrg
        Projects = @(
            @{ Project = 'DbLocalizationProvider.AspNetCore';          Id = 'LocalizationProvider.AspNetCore' }
            @{ Project = 'DbLocalizationProvider.AdminUI.AspNetCore';  Id = 'LocalizationProvider.AdminUI.AspNetCore' }
            @{ Project = 'DbLocalizationProvider.AdminUI.AspNetCore.Csv';   Id = 'LocalizationProvider.AdminUI.AspNetCore.Csv' }
            @{ Project = 'DbLocalizationProvider.AdminUI.AspNetCore.Xliff'; Id = 'LocalizationProvider.AdminUI.AspNetCore.Xliff' }
        )
    }
    [pscustomobject]@{
        Name     = 'optimizely'
        Feed     = $episerver
        Projects = @(
            @{ Project = 'DbLocalizationProvider.EPiServer';           Id = 'DbLocalizationProvider.EPiServer' }
            @{ Project = 'DbLocalizationProvider.AdminUI.EPiServer';    Id = 'DbLocalizationProvider.AdminUI.EPiServer' }
        )
    }
)

$root     = $PSScriptRoot
$nugetDir = Join-Path $root '.nuget'

# Default action: build only. Publishing always has to be requested explicitly.
if (-not $Build -and -not $Push) { $Build = $true }

$selected = if ($Area) { $areas | Where-Object { $_.Name -in $Area } } else { $areas }

# Note: the per-area config variable is deliberately NOT named $area — that
# would collide (case-insensitively) with the validated $Area parameter, and
# Windows PowerShell 5.1 re-enforces [ValidateSet] on every assignment to it.
function Invoke-PackArea {
    param([Parameter(Mandatory)] $AreaConfig)

    New-Item -ItemType Directory -Force -Path $nugetDir | Out-Null

    foreach ($p in $AreaConfig.Projects) {
        $projectDir = Join-Path $root "$($AreaConfig.Name)/src/$($p.Project)"
        Write-Host "==> pack $($p.Id)  ($($AreaConfig.Name))" -ForegroundColor Cyan

        # Every project sets <IncludeSymbols>true</IncludeSymbols> + snupkg in its
        # csproj, so a .snupkg is always produced and collected alongside the .nupkg.
        dotnet pack $projectDir -c Release --include-symbols -p:SymbolPackageFormat=snupkg
        if ($LASTEXITCODE -ne 0) { throw "pack failed for $($p.Id)" }

        $outDir = Join-Path $projectDir 'bin/Release'
        Copy-Item (Join-Path $outDir "$($p.Id).$Version.nupkg")  $nugetDir -Force
        Copy-Item (Join-Path $outDir "$($p.Id).$Version.snupkg") $nugetDir -Force
    }
}

function Invoke-PushArea {
    param([Parameter(Mandatory)] $AreaConfig)

    foreach ($p in $AreaConfig.Projects) {
        $package = Join-Path $nugetDir "$($p.Id).$Version.nupkg"
        if (-not (Test-Path $package)) {
            throw "package not found: $package (run with -Build first)"
        }
        Write-Host "==> push $($p.Id) -> $($AreaConfig.Feed)" -ForegroundColor Green

        # dotnet nuget push automatically pushes the matching .snupkg alongside the .nupkg.
        $pushArgs = @('nuget', 'push', $package, '--source', $AreaConfig.Feed, '--skip-duplicate')
        if ($ApiKey) { $pushArgs += '--api-key', $ApiKey }
        dotnet @pushArgs
        if ($LASTEXITCODE -ne 0) { throw "push failed for $($p.Id)" }
    }
}

if ($Build) {
    foreach ($areaConfig in $selected) { Invoke-PackArea $areaConfig }
    Write-Host "Build complete -> $nugetDir" -ForegroundColor Yellow
}

if ($Push) {
    foreach ($areaConfig in $selected) { Invoke-PushArea $areaConfig }
    Write-Host "Push complete." -ForegroundColor Yellow
}
