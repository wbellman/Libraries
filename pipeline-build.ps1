[CmdletBinding()]
param (
    [switch]$SkipTests,
    [switch]$GitHubCI,
    [string]$Version = "0.0.2",
    [string]$LibraryName = "Unspecified.Package",
    [string]$SHA = "111111"
)

# Debugging Output
Write-Output "🔹 Building $LibraryName with Version: $Version and SHA: $SHA"

# Set strict mode for better debugging
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

Set-Location -Path "./$LibraryName"
$workdir = (Get-Location).Path
Write-Host "🔹 Building in Directory: $workdir"

# Detect the branch name
$branchName = if ($GitHubCI) {
    $env:GITHUB_REF -replace 'refs/heads/', ''
} else {
    git rev-parse --abbrev-ref HEAD
}

# Determine version suffix based on branch
$versionSuffix = switch -Regex ($branchName) {
    "develop" { "beta" }
    "feature/.*" { "prerelease" }
    "hotfix/.*" { "prerelease" }
    "release/.*" { "rc" }
    default { "" }
}

# Construct full version including SHA
$packageVersion = if ($versionSuffix) {
    "$Version-$SHA-$versionSuffix"
} else {
    "$Version"
}

Write-Host "🔹 Building $LibraryName for branch: $branchName (Version: $packageVersion)"

try {
    # Restore dependencies
    Write-Host "🔄 Restoring dependencies..."
    dotnet restore
    if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed" }

    # Build project
    Write-Host "🔨 Building project..."
    dotnet build --no-restore --configuration Release /p:Version="$packageVersion"
    if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }

    # Run tests if not skipped
    if (-not $SkipTests) {
        Write-Host "🧪 Running tests..."
        dotnet test --no-build --configuration Release
        if ($LASTEXITCODE -ne 0) { throw "Tests failed" }
    }

    # Package NuGet
    Write-Host "📦 Packing NuGet package..."
    dotnet pack --no-build --configuration Release --output bin/Release /p:Version="$packageVersion"
    if ($LASTEXITCODE -ne 0) { throw "dotnet pack failed" }

    Write-Host "✅ Build process completed successfully!"
}
catch {
    Write-Error "❌ Build process failed: $_"
    exit 1
}
