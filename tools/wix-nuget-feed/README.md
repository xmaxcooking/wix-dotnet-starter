# wix-nuget-feed

Local NuGet feed of WiX Toolset v7.0.0 packages, **self-compiled from source**
(https://github.com/wixtoolset/wix, tag `v7.0.0`) rather than downloaded pre-built from
nuget.org.

## Why

Starting with WiX v6, the pre-built NuGet packages ("Binary Release") are covered by an
[Open Source Maintenance Fee](https://opensourcemaintenancefee.org/) - see `OSMFEULA.txt` in
the WiX repo. It only applies to users with annual gross revenue >= $10,000 who consume the
binary release; self-compiled binaries are explicitly exempt under the EULA's own terms
("User may independently compile binaries from the Software's source code without this
Agreement"). Building from source avoids the fee entirely.

`NuGet.Config` at the repo root routes `WixToolset.*`, `WixInternal.*`, and `wix` package IDs
to this folder instead of nuget.org; everything else still comes from nuget.org.

## Rebuilding this feed

Building WiX from source requires:

- Visual Studio 2026 with the **Desktop development with C++** workload (MSVC + ATL) and the
  **.NET Framework 4.6.2 targeting pack** - not installed by default with just .NET workloads.
- The exact MSVC toolset your VS install ships may not match what the WiX source hardcodes
  (`v143`). Check `MSBuild\Microsoft\VC\<version>\Platforms\x64\PlatformToolsets\` for what's
  actually available and adjust `Directory.Build.props`'s `PlatformToolset` accordingly.
- A side-loaded .NET SDK matching whatever version `global.json` (generated during the build)
  pins - install via Microsoft's `dotnet-install.ps1` into an isolated folder, don't touch your
  system-wide SDK.
- `nuget.exe` (classic CLI) on PATH, for a couple of native-test-support packages.Config restores.

High-level steps (all from an **isolated clone**, not this repo):

1. `git clone --branch v7.0.0 https://github.com/wixtoolset/wix.git`
2. Set `NUGET_PACKAGES` to a throwaway folder before building anything - `clean.cmd` and several
   per-stage `.cmd` scripts delete `WixToolset.*`/`WixInternal.*` packages from
   `%USERPROFILE%\.nuget\packages` (the real global cache) as part of their own cleanup, which
   will wipe cached packages other projects on your machine depend on if you don't isolate it.
3. Patch `-tl` (MSBuild's terminal logger) to `-tl:off` throughout `src/**/*.cmd` - it breaks
   when output is redirected to a file/log rather than a live console.
4. Add `-p:NuGetAudit=false` after every `-nologo` in `src/**/*.cmd` - NuGet's vulnerability
   audit treats some of WiX's own transitive dependencies as hard build errors on a current SDK,
   even though they weren't flagged when v7.0.0 was tagged.
5. Remove ARM64 wherever it appears - this checkout builds x86/x64 only. ARM64 support needs
   the separate VS C++ ARM64 cross-compilation component this environment didn't have, and even
   with it installed, WiX's ARM64 authoring is threaded through the source in more places than
   you'd expect: per-project `ProjectReference`/`Platforms` metadata in native traversal
   `_t.proj` files AND `.wixproj` files, nuspec `<file>` entries, `Directory.Build.targets`
   `NativeLibrary` items, `RuntimeIdentifiers` on `wix.csproj`, and - easy to miss - `*_arm64.wxs`
   source files that get auto-globbed into every extension's `.wixproj` regardless of its
   `Platforms` property, plus a couple of hardcoded `<?foreach PLATFORM in x86;x64;arm64?>` loops
   inside shared `.wxi` files. Search broadly (`arm64`/`ARM64`, case-insensitive) across
   `*.proj`, `*.csproj`, `*.vcxproj`, `*.wixproj`, `*.nuspec`, `*.wxs`, `*.wxi`, `*.targets`,
   `*.props` - a single missed reference surfaces as a build failure several stages later.
6. Remove `Microsoft.SourceLink.GitHub` package references (`Directory.csproj.targets` and a
   couple of individual `.csproj`/`.wixproj` files) if building from anything other than a real
   git clone with full history (e.g. a downloaded source zip) - SourceLink needs actual git
   metadata to resolve and fails hard without it. Building from a real `git clone` (not a zip
   download) avoids this entirely.
7. Remove test-project references that aren't needed to produce packages, and skip actually
   *running* tests (only building them, or skipping test invocation entirely) - this repo's copy
   doesn't need a passing WiX test suite, just working binaries.
8. Run each stage script directly in order - `dtf`, `internal`, `libs`, `api`, `burn`, `wix`,
   `tools`, `ext` - rather than the top-level `devbuild.cmd`/`build_all.cmd`, which also runs
   `test.cmd` and `finalize.cmd` (not needed here, and `test.cmd` unconditionally runs real
   `dotnet test` invocations that can fail for reasons unrelated to whether the packages
   themselves are good).
9. Copy every `.nupkg` from `build\artifacts\` in the clone into this folder.

None of the patches above are committed anywhere in the WiX source itself - they're just what
was needed to get a working local build with this environment's specific tool versions (VS2026,
.NET 9/10 SDKs). A different environment may hit a different subset of these, or none at all.
