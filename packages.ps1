<#
.SYNOPSIS
    Builds and/or publishes every LocalizationProvider NuGet package
    (common, aspnetcore, optimizely) from the solution root.

.DESCRIPTION
    Replaces the per-area build-packages.ps1 / push-packages.ps1 scripts.
    All packages are packed into the root .nuget folder and pushed from there.

    Publishing is split into two phases on purpose:
      -Push        uploads the .nupkg files only (with --no-symbols).
      -PushSymbols uploads the .snupkg files, run *after* the packages have
                   been validated/indexed by the feed.
    nuget.org rejects a symbol package until its parent package version has
    finished server-side validation, so pushing both at once returns a 403 on
    the symbol server. Decoupling the two avoids that race.

    Both phases are resilient: a package that already exists on the feed, a
    missing artifact, or a transient symbol-server error is reported as a
    warning and the run moves on to the next package instead of aborting.

.PARAMETER Build
    Pack the selected areas into .nuget. This is the default when no switch is given.

.PARAMETER Push
    Push the packed .nupkg files (without symbols) from .nuget to their feed.

.PARAMETER PushSymbols
    Push the packed .snupkg files to feeds that have a symbol server (nuget.org).
    Run this once the matching packages are live on the feed.

.PARAMETER Area
    Limit the run to one or more areas: common, aspnetcore, optimizely. Default: all.

.PARAMETER Version
    Package version to pack/push. Must match <PackageVersion> in the csproj files. Default: 9.0.0.

.PARAMETER ApiKey
    NuGet API key used by -Push / -PushSymbols. If omitted, the feed's pre-configured credentials are used.

.EXAMPLE
    ./packages.ps1
    Builds all packages into .nuget (no push).

.EXAMPLE
    ./packages.ps1 -Build -Push -ApiKey $env:NUGET_KEY
    Builds everything, then publishes the packages (symbols come later).

.EXAMPLE
    ./packages.ps1 -PushSymbols -ApiKey $env:NUGET_KEY
    Publishes the symbol packages once the packages are indexed on the feed.

.EXAMPLE
    ./packages.ps1 -Push -Area common -ApiKey $env:NUGET_KEY
    Publishes only the already-built common packages.
#>
[CmdletBinding()]
param(
    [switch]$Build,
    [switch]$Push,
    [switch]$PushSymbols,
    [ValidateSet('common', 'aspnetcore', 'optimizely')]
    [string[]]$Area,
    [string]$Version = '9.0.0',
    [string]$ApiKey
)

$ErrorActionPreference = 'Stop'

$nugetOrg  = 'https://api.nuget.org/v3/index.json'
$episerver = 'https://nuget.episerver.com/feed/packages.svc'

# Each area declares its feed, whether the feed has a symbol server, and the
# projects to pack/push as (source folder under <area>/src) -> (NuGet PackageId).
# Push iterates these same lists, so the package set always matches what is built.
$areas = @(
    [pscustomobject]@{
        Name     = 'common'
        Feed     = $nugetOrg
        Symbols  = $true
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
        Symbols  = $true
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
        Symbols  = $false
        Projects = @(
            @{ Project = 'DbLocalizationProvider.EPiServer';           Id = 'DbLocalizationProvider.EPiServer' }
            @{ Project = 'DbLocalizationProvider.AdminUI.EPiServer';    Id = 'DbLocalizationProvider.AdminUI.EPiServer' }
        )
    }
)

$root     = $PSScriptRoot
$nugetDir = Join-Path $root '.nuget'

# Default action: build only. Publishing always has to be requested explicitly.
if (-not $Build -and -not $Push -and -not $PushSymbols) { $Build = $true }

$selected = if ($Area) { $areas | Where-Object { $_.Name -in $Area } } else { $areas }

# Failures collected across the whole run so one bad package never aborts the rest.
$script:pushFailures   = @()
$script:symbolFailures = @()

# Note: the per-area config variable is deliberately NOT named $area - that
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

# Pushes a single .nupkg/.snupkg without throwing. Returns 'ok', 'duplicate'
# (the version is already on the feed) or 'failed' so the caller can aggregate.
function Push-NugetFile {
    param(
        [Parameter(Mandatory)][string]$Path,
        [Parameter(Mandatory)][string]$Feed,
        [switch]$NoSymbols
    )

    # Relax error handling locally: we inspect $LASTEXITCODE ourselves and must
    # not let a non-zero dotnet exit auto-throw (PSNativeCommandUseErrorActionPreference).
    $ErrorActionPreference = 'Continue'

    $pushArgs = @('nuget', 'push', $Path, '--source', $Feed, '--skip-duplicate')
    if ($NoSymbols) { $pushArgs += '--no-symbols' }
    if ($ApiKey)    { $pushArgs += '--api-key', $ApiKey }

    $log  = & dotnet @pushArgs 2>&1
    $text = ($log | Out-String).Trim()
    if ($text) { Write-Host $text }

    if ($LASTEXITCODE -eq 0) { return 'ok' }
    # Feeds that don't honour --skip-duplicate (e.g. the EPiServer v2 feed) return
    # a non-zero "already exists" / 409 Conflict instead - treat it as a duplicate.
    if ($text -match 'already exists|409|[Cc]onflict') { return 'duplicate' }
    return 'failed'
}

function Invoke-PushArea {
    param([Parameter(Mandatory)] $AreaConfig)

    foreach ($p in $AreaConfig.Projects) {
        $package = Join-Path $nugetDir "$($p.Id).$Version.nupkg"
        if (-not (Test-Path $package)) {
            Write-Warning "package not found: $package (run with -Build first) - skipping"
            continue
        }
        Write-Host "==> push $($p.Id) -> $($AreaConfig.Feed)" -ForegroundColor Green

        switch (Push-NugetFile -Path $package -Feed $AreaConfig.Feed -NoSymbols) {
            'duplicate' { Write-Warning "$($p.Id) $Version already exists on the feed - skipping" }
            'failed'    { Write-Warning "push failed for $($p.Id) - continuing"; $script:pushFailures += $p.Id }
        }
    }
}

function Invoke-PushSymbolsArea {
    param([Parameter(Mandatory)] $AreaConfig)

    if (-not $AreaConfig.Symbols) {
        Write-Host "==> skip symbols for $($AreaConfig.Name) (feed has no symbol server)" -ForegroundColor DarkGray
        return
    }

    foreach ($p in $AreaConfig.Projects) {
        $symbols = Join-Path $nugetDir "$($p.Id).$Version.snupkg"
        if (-not (Test-Path $symbols)) {
            Write-Warning "symbol package not found: $symbols (run with -Build first) - skipping"
            continue
        }
        Write-Host "==> push symbols $($p.Id) -> $($AreaConfig.Feed)" -ForegroundColor Green

        switch (Push-NugetFile -Path $symbols -Feed $AreaConfig.Feed) {
            'duplicate' { Write-Warning "symbols for $($p.Id) $Version already exist - skipping" }
            'failed'    { Write-Warning "symbol push failed for $($p.Id) - retry -PushSymbols once the package is live"; $script:symbolFailures += $p.Id }
        }
    }
}

if ($Build) {
    foreach ($areaConfig in $selected) { Invoke-PackArea $areaConfig }
    Write-Host "Build complete -> $nugetDir" -ForegroundColor Yellow
}

if ($Push) {
    foreach ($areaConfig in $selected) { Invoke-PushArea $areaConfig }
    if ($script:pushFailures.Count) {
        Write-Warning "Packages that failed to push: $($script:pushFailures -join ', ')"
    } else {
        Write-Host "Push complete." -ForegroundColor Yellow
    }
}

if ($PushSymbols) {
    foreach ($areaConfig in $selected) { Invoke-PushSymbolsArea $areaConfig }
    if ($script:symbolFailures.Count) {
        Write-Warning "Symbols not published for: $($script:symbolFailures -join ', '). Re-run -PushSymbols once the packages are live."
    } else {
        Write-Host "Symbol push complete." -ForegroundColor Yellow
    }
}
