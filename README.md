# Core.Blazorise
A project focused on Core.Blazorise.

## Tags

- dotnet
- core
- core-blazorise
- docs
- blazorise
- ui

## Overview
This repository contains the Core.Blazorise project. It is designed to provide robust functionality and seamless integration within its ecosystem.

## 🚀 Key Features
- General Purpose Utility
- Core Application Logic
- Standardized Project Layout
- Core Capabilities
- Sub Module Articles
- Sub Module Images
- Sub Module Api
- [Feature 3 (Beyond the App capability)]

## 🛠 Technology Stack
- C# / .NET

## 📖 Documentation
Detailed documentation can be found in the following sections:
- [Feature Index](./docs/features/README.md)
- [Core Capabilities](./docs/features/core-capabilities.md)

## Docs

- [Roadmap Index](./docs/roadmaps/README.md)

## 🚦 Getting Started
```bash
dotnet restore PolyhydraGames.BlazorComponents.sln
dotnet build PolyhydraGames.BlazorComponents.sln --configuration Release --no-restore
dotnet test PolyhydraGames.BlazorComponents.sln --configuration Release --no-restore --no-build --verbosity minimal
```

## Validation and artifacts

The native validation path is:

```bash
dotnet test PolyhydraGames.BlazorComponents.sln --configuration Release --verbosity minimal
dotnet pack src/PolyhydraGames.BlazorComponents.csproj --configuration Release --no-build --output artifacts/package
dotnet list PolyhydraGames.BlazorComponents.sln package --outdated
```

GitHub Actions restores, builds, tests, packs, and uploads the package outputs as the `core-blazorise-nuget` workflow artifact from `artifacts/package`. Tags matching `v*` publish `.nupkg` files to GitHub Packages using the built-in `GITHUB_TOKEN`; no extra repository secret is required.

Current dependency drift is limited to `MudBlazor` 9.5.0 -> 9.6.0 and is tracked for a package-maintenance slice.
