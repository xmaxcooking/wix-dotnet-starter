# wix-dotnet-starter

Template repository for .NET 10 projects packaged with a WiX v5 MSI installer.

## Layout

```
WixDotnetStarter.slnx
src/
  App.Core/        class library - sample "Customer" model + in-memory sample data
  App.WinForms/     WinForms app (net10.0-windows) - shows the sample data in a DataGridView
tests/
  App.Core.Tests/  xunit tests for App.Core
installer/
  App.Installer/   WiX v5 MSI packaging App.WinForms (installer/App.Installer/App.Installer.wixproj)
  App.Bundle/      WiX v5 Burn bootstrapper chaining the .NET 10 Desktop Runtime + App.Installer.msi
                    into one .exe (installer/App.Bundle/App.Bundle.wixproj) - see below, not part
                    of the default solution build
```

## Prerequisites

- .NET 10 SDK (pinned via `global.json`)
- Windows, for the WinForms project and for building/running the MSI

## Build

```
dotnet build
```

Builds the class library, the WinForms app, the tests, and the MSI installer.

## Run the sample app

```
dotnet run --project src/App.WinForms
```

## Run tests

```
dotnet test
```

## Build the installer only

```
dotnet build installer/App.Installer/App.Installer.wixproj -c Release
```

Produces `installer/App.Installer/bin/x64/Release/App.Installer.msi`. `WixDotnetStarter.slnx`
pins `App.Installer` to the `x64` platform via a per-project `<Platform>` override, so
`dotnet build` at the solution root also produces the MSI without extra flags.

## Build the bundle (bootstrapper .exe)

`App.Bundle` chains the .NET 10 Desktop Runtime installer ahead of `App.Installer.msi`
into a single self-contained-looking `.exe`, using WiX Burn. It is **not** referenced
by `WixDotnetStarter.slnx` and isn't built by plain `dotnet build`, because it needs a
prerequisite that isn't checked into the repo:

1. Download the runtime installer into `installer/App.Bundle/Redist/` (see
   `installer/App.Bundle/Redist/README.md` for the exact filename/URL).
2. Build it explicitly:
   ```
   dotnet build installer/App.Bundle/App.Bundle.wixproj -c Release
   ```
   Produces `installer/App.Bundle/bin/x64/Release/App.Bundle.exe`.

## Adapting this template

- `installer/App.Installer/Components.wxs` lists the App.WinForms build output explicitly,
  with a fixed GUID per file. When you add/remove files from App.WinForms (new project
  references, new dependencies, ...), update this list to match — and never change an
  existing file's GUID once shipped, or MSI upgrades for that file will break.
- Replace the placeholder `Manufacturer`, product name, and `UpgradeCode` in
  `installer/App.Installer/Product.wxs` and `installer/App.Bundle/Bundle.wxs` before shipping
  anything built from this template. `UpgradeCode` in particular must never change after the
  first release.
- `installer/App.Bundle/Installers/DotNetDesktopRuntime.wxs` has no `DetectCondition`, so it
  always runs the runtime installer (harmless - it's idempotent) rather than probing the
  registry for an exact installed version. See the comment in that file for how to tighten it.
- `App.Core` is the seam for real domain code / data access; `App.WinForms` is the seam for UI.
