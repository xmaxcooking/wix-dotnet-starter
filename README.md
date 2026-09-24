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
                    into one .exe (installer/App.Bundle/App.Bundle.wixproj) - see below, listed in
                    the solution but excluded from its default build (<Build Project="false" />)
build/
  _build.csproj    Fallout (NUKE successor) build pipeline - see "Full pipeline" below
```

## Prerequisites

- .NET 10 SDK
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
into a single self-contained-looking `.exe`, using WiX Burn. It's listed in
`WixDotnetStarter.slnx` (so it shows up in an IDE) but marked `<Build Project="false" />`
there, so plain `dotnet build` skips it — it embeds a ~60 MB redistributable
(`installer/App.Bundle/Redist/windowsdesktop-runtime-win-x64.exe`, checked in — see
`installer/App.Bundle/Redist/README.md`), which would make every routine `dotnet build`
slower for no benefit during day-to-day app development. Build it explicitly instead:

```
dotnet build installer/App.Bundle/App.Bundle.wixproj -c Release
```

Produces `installer/App.Bundle/bin/x64/Release/App.Bundle.exe`.

## Full pipeline (Fallout)

`build/` is a [Fallout](https://fallout.build/) build (the maintained successor to NUKE -
a C#-first build automation tool: your pipeline is a real console app, not YAML). It
showcases the pipeline end to end - Clean, Restore, Compile, Test, and `PackBundle`,
which builds `App.Bundle.wixproj` and copies the resulting `.exe` into `artifacts/`.

```
dotnet tool restore
dotnet fallout Test          # Restore -> Compile -> Test (the default if you just run `dotnet fallout`)
dotnet fallout PackBundle    # -> also builds the bundle, needs Redist/ populated (see above)
dotnet fallout Clean
```

`dotnet tool restore` pulls in the pinned `fallout.globaltool` from `.config/dotnet-tools.json`
so `dotnet fallout` works without a machine-wide install; `./build.sh` / `.\build.ps1` do the
same plus provision the .NET SDK itself if it's missing, which is what a CI runner would call.
`build/Build.cs` is the pipeline definition - add targets there the same way you'd add any
other C# method, e.g. a `Push` target that publishes `App.Bundle.exe` somewhere.

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
