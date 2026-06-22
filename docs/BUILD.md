# Build and CI

This repository builds .NET projects through GitHub Actions workflows in
`/.github/workflows` and a shared PowerShell script at `/pipeline-build.ps1`.

## Build script: `pipeline-build.ps1`

`pipeline-build.ps1` is the common build entry point for a single project.

What it does:
- accepts `LibraryName`, `Version`, `SHA`, and switches like `GitHubCI` and `SkipTests`
- changes working directory to `./<LibraryName>`
- derives branch name from `GITHUB_REF` in CI or `git rev-parse --abbrev-ref HEAD` locally
- computes a package version suffix by branch pattern:
  - `develop` → `beta`
  - `feature/*` or `hotfix/*` → `prerelease`
  - `release/*` → `rc`
  - all others → no suffix
- runs `dotnet restore`, `dotnet build --configuration Release`, `dotnet test` (unless skipped), and `dotnet pack`
- writes a failure and exits non-zero on any failed step

> TODO(wbellman): why — explain the intended branch-to-version-suffix strategy and release process constraints.

## Reusable workflows

### `/.github/workflows/_build.yml`

Reusable workflow (`workflow_call`) that:
- requires `library_name` and `package_url` inputs
- checks out the repo and installs .NET 9 SDK
- adds the GitHub Packages NuGet source with `${{ secrets.GITHUB_TOKEN }}`
- calculates a timestamped version (`1.<yy>.<dayOfYear>.<hour><minute>`) and short SHA
- invokes `pipeline-build.ps1` with `-GitHubCI` and the computed values

> TODO(wbellman): why — explain why CI uses .NET 9 SDK while projects currently target `net8.0`.

### `/.github/workflows/_build_publish.yml`

Reusable workflow (`workflow_call`) with the same setup/build steps as `_build.yml`,
plus publish steps:
- pushes generated `.nupkg` files on `feature/*` branches as prerelease
- pushes on `develop` branches as beta
- pushes on `main` as stable

All pushes use the configured `package_url` and `${{ secrets.GITHUB_TOKEN }}`.

## Repository workflows

### `/.github/workflows/library-operations.yml`

Triggered on pushes to `main`, `develop`, `feature/**`, `release/**`, and `hotfix/**`
when files change under:
- `Library.Operations/**`
- `/.github/workflows/library-operations.yml`
- `/.github/workflows/_build_publish.yml`

It calls `/_build_publish.yml` with:
- `library_name: "Library.Operations"`
- GitHub Packages URL for `wbellman`

### `/.github/workflows/library-authentication.yml`

Triggered on pushes to `main`, `develop`, `feature/**`, `release/**`, and `hotfix/**`
when files change under:
- `Library.Operations/**` (as currently written in the workflow)
- `/.github/workflows/library-authentication.yml`
- `/.github/workflows/_build_publish.yml`

It calls `/_build_publish.yml` with:
- `library_name: "Library.Authentication"`
- GitHub Packages URL for `wbellman`

> TODO(wbellman): why — confirm whether the path filter in `library-authentication.yml` should target `Library.Authentication/**`.

## How the pieces fit

1. A library-specific workflow (`library-operations.yml` or `library-authentication.yml`) triggers on matching push events.
2. That workflow delegates to the shared publish-capable reusable workflow (`_build_publish.yml`).
3. The reusable workflow computes version metadata and invokes `pipeline-build.ps1`.
4. The script restores, builds, tests, and packs the selected project.
5. The reusable workflow publishes the resulting package based on branch rules.
