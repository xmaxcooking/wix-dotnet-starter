# wix-dotnet-starter

Template repository for .NET 10 projects packaged with a WiX v7 MSI installer and Burn
bootstrapper.

## Layout

```
WixDotnetStarter.slnx
NuGet.Config       routes WixToolset.*/wix/WixInternal.* packages to tools/wix-nuget-feed
                    instead of nuget.org - see "Why a local WiX NuGet feed?" below
assets/            shared icon/logo/license assets used by the installer and bundle
src/
  App.Core/        class library - sample "Customer" model + in-memory sample data
  App.WinForms/    WinForms app (net10.0-windows) - shows the sample data in a DataGridView
tests/
  App.Core.Tests/  MSTest tests for App.Core
installer/
  App.Actions/     managed custom actions (net48) for the MSI - see CustomActions.wxs
  App.InstallTool/ .NET 10 console app run as a Burn ExePackage - the pattern for install-time
                    code that needs modern .NET, since App.Actions (an MSI custom action) can't
  App.Installer/   WiX v7 MSI packaging App.WinForms (installer/App.Installer/App.Installer.wixproj)
  App.Bundle/      WiX v7 Burn bootstrapper chaining the .NET 10 Desktop Runtime + App.Installer.msi
                    into one .exe (installer/App.Bundle/App.Bundle.wixproj) - see below, listed in
                    the solution but excluded from its default build (<Build Project="false" />)
build/
  _build.csproj    Fallout (NUKE successor) build pipeline - see "Full pipeline" below
tools/
  wix-nuget-feed/  self-compiled WiX v7 NuGet packages - see "Why a local WiX NuGet feed?" below
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

`build/Properties/launchSettings.json` has one profile per target (`Test`, `PackBundle`, a
`PackBundle (Release)` variant, ...), so you can run/debug any of them from an IDE's run
configuration dropdown instead of the command line - Visual Studio and Rider both pick these
up automatically for `build/_build.csproj`. Verified with `dotnet run --project build/_build.csproj
--launch-profile Test` (and `--launch-profile "PackBundle (Release)"`, which correctly
produced `installer/App.Bundle/bin/x64/Release/App.Bundle.exe`).

## CI: release on push to master

`.github/workflows/release.yml` is generated from the `[GitHubActions]` attribute on `Build` in
`build/Build.cs` - don't hand-edit the `.yml`; change the attribute and run the build once to
regenerate it. On every push to `master` it runs `PublishRelease` (which depends on
`PackBundle`) on `windows-latest`, creating a GitHub Release tagged `build-<run number>` -
always unique, no collisions across re-runs - marked as a prerelease, with `App.Bundle.exe`
attached. Auth is `GITHUB_TOKEN`, provided automatically by Actions; no secrets to configure.
`global.json` pins the SDK version `actions/setup-dotnet` installs.

## Why a local WiX NuGet feed?

WiX v6+ pre-built NuGet packages (`WixToolset.Sdk`, every `WixToolset.*.wixext`) are covered by
an [Open Source Maintenance Fee](https://opensourcemaintenancefee.org/): required if annual
gross revenue is ≥ US$10,000. Terms:
[`OSMFEULA.txt`](https://github.com/wixtoolset/wix/blob/v7.0.0/OSMFEULA.txt). Honor-system,
not a license key or network check.

The EULA exempts self-compiled binaries. `tools/wix-nuget-feed/` holds all 47 WiX v7.0.0
packages built from source (https://github.com/wixtoolset/wix, tag `v7.0.0`) instead of
downloaded from nuget.org.

`NuGet.Config` at the repo root maps `WixToolset.*`, `WixInternal.*`, and `wix` package IDs to
`tools/wix-nuget-feed/` exclusively via `packageSourceMapping` - no fallback to nuget.org for
those IDs. Everything else (SDK packages, `coverlet.collector`, `MSTest.*`, ...) still resolves
from nuget.org normally.

Notes:

- The WiX v7 EULA still needs a one-time local acceptance regardless of self-compiling - separate
  concern from the fee. First build fails with `error WIX7015` until `wix eula accept wix7` is
  run once (writes a marker file under `%USERPROFILE%\.wix\`, no network/payment). No `wix.exe`
  on PATH by default - see `tools/wix-nuget-feed/README.md` for a self-compiled one, or install
  the [WiX CLI](https://wixtoolset.org/docs/tools/net-tools/) separately.
- To use the official nuget.org packages instead: delete `NuGet.Config` and bump the
  `WixToolset.Sdk`/`WixToolset.*.wixext` versions in the three `.wixproj`/`.csproj` files under
  `installer/` to whatever's current.
- To rebuild this feed (e.g. for a newer WiX version): `tools/wix-nuget-feed/README.md`.

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
- MSI custom actions (`App.Actions`) can only ever be .NET Framework - `msiexec` loads them via
  `mscoree.dll`, which has no path to modern .NET, and this is a hard limitation of Windows
  Installer itself, not something WiX can fix (still an open, unresolved WiX feature request:
  [wixtoolset/issues#7932](https://github.com/wixtoolset/issues/issues/7932)). If install-time
  code needs a modern .NET library, run it as a Burn `ExePackage` instead - Burn just launches
  it as a normal process, no `mscoree` involved. `App.InstallTool` is a worked example of this,
  wired into `Bundle.wxs`'s `Chain` via `Installers/InstallTool.wxs`; it receives the same
  `SampleBundleVariable` the MSI custom action does, as a command-line argument instead of an
  MSI property. Trade-off: no direct access to the MSI session, only what's passed via args and
  the exit code back.
