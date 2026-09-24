using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Fallout.Common;
using Fallout.Common.CI.GitHubActions;
using Fallout.Common.Execution;
using Fallout.Common.Git;
using Fallout.Common.IO;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.DotNet;
using Fallout.Common.Tools.GitHub;
using Fallout.Common.Utilities.Collections;
using Fallout.Solutions;
using Octokit;
using static Fallout.Common.EnvironmentInfo;
using static Fallout.Common.Tools.DotNet.DotNetTasks;

// Showcases the full pipeline: Clean -> Restore -> Compile -> Test -> PackBundle.
// `Compile` builds everything WixDotnetStarter.slnx references by default - App.Core,
// App.WinForms, App.Core.Tests and App.Installer.msi (App.Bundle is excluded from the
// solution's default build via <Build Project="false" />, see the .slnx). `PackBundle`
// builds it explicitly, needing the .NET Desktop Runtime installer downloaded first -
// see installer/App.Bundle/Redist/README.md - so it's not part of the default target.
//
// [GitHubActions] generates .github/workflows/release.yml from the attribute below - edit
// here and run the build once (locally, or let CI's first run rewrite it) to regenerate;
// never hand-edit the .yml. windows-latest because the whole pipeline (WinForms, WiX,
// MSI/Burn) is Windows-only.
[GitHubActions(
    "release",
    GitHubActionsImage.WindowsLatest,
    OnPushBranches = new[] { "master" },
    InvokedTargets = new[] { nameof(PublishRelease) },
    EnableGitHubToken = true,
    WritePermissions = new[] { GitHubActionsPermissions.Contents },
    PublishArtifacts = false)]
class Build : FalloutBuild
{
    public static int Main() => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;

    [Parameter("GitHub token used to create the release - set via GITHUB_TOKEN, provided automatically in Actions")]
    [Secret]
    readonly string GitHubToken = GitHubActions.Instance?.Token;

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath BundleProject => RootDirectory / "installer" / "App.Bundle" / "App.Bundle.wixproj";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            RootDirectory.GlobDirectories("src/**/bin", "src/**/obj", "tests/**/bin", "tests/**/obj",
                    "installer/**/bin", "installer/**/obj")
                .ForEach(x => x.DeleteDirectory());
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .Executes(() => DotNetRestore(s => s
            .SetProjectFile(Solution)));

    // WiX v7's Open Source Maintenance Fee EULA gate (error WIX7015) needs a one-time
    // per-machine acceptance marker before any project referencing WixToolset.Sdk will
    // compile - see tools/wix-nuget-feed/README.md. A fresh clone or a CI runner (a brand
    // new VM every run) never has that marker, so this can't be a one-off a developer runs
    // locally once; it has to run every time, right after Restore puts WixToolset.Sdk's
    // wix.exe in the NuGet cache and before Compile needs it. Cheap when already accepted -
    // wix.exe itself no-ops on a second acceptance.
    Target AcceptWixEula => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            var nugetPackages = GetVariable("NUGET_PACKAGES")
                ?? Path.Combine(GetVariable("USERPROFILE") ?? GetVariable("HOME")!, ".nuget", "packages");

            var wixExe = Directory.GetFiles(nugetPackages, "wix.exe", SearchOption.AllDirectories)
                .FirstOrDefault(x => x.Contains("wixtoolset.sdk", StringComparison.OrdinalIgnoreCase));
            if (wixExe == null)
                throw new InvalidOperationException(
                    "wix.exe not found under the NuGet packages folder after restore - was WixToolset.Sdk restored?");

            using var process = Process.Start(new ProcessStartInfo(wixExe, "eula accept wix7") { UseShellExecute = false });
            process!.WaitForExit();
            if (process.ExitCode != 0)
                throw new InvalidOperationException("'wix eula accept wix7' failed");
        });

    Target Compile => _ => _
        .DependsOn(AcceptWixEula)
        .Executes(() => DotNetBuild(s => s
            .SetProjectFile(Solution)
            .SetConfiguration(Configuration)
            .EnableNoRestore()));

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() => DotNetTest(s => s
            .SetProjectFile(Solution)
            .SetConfiguration(Configuration)
            .EnableNoBuild()));

    Target PackBundle => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(BundleProject)
                .SetConfiguration(Configuration));

            ArtifactsDirectory.CreateOrCleanDirectory();
            BundleProject.Parent.GlobFiles($"bin/x64/{Configuration}/*.exe")
                .ForEach(x => x.CopyToDirectory(ArtifactsDirectory, ExistsPolicy.FileOverwriteIfNewer));
        });

    // Runs on every push to master (see the [GitHubActions] attribute above): creates a GitHub
    // Release tagged by the CI run number - always unique, never collides across re-runs - and
    // uploads App.Bundle.exe to it. Marked Prerelease since it's an unattended per-push build,
    // not a curated version; flip that (and the tag scheme) if you want deliberate releases
    // instead.
    Target PublishRelease => _ => _
        .DependsOn(PackBundle)
        .Requires(() => GitHubToken)
        .Executes(async () =>
        {
            GitHubTasks.GitHubClient.Credentials = new Credentials(GitHubToken);

            var tag = $"build-{GitHubActions.Instance.RunNumber}";
            var release = await GitHubTasks.GitHubClient.Repository.Release.Create(
                GitRepository.GetGitHubOwner(),
                GitRepository.GetGitHubName(),
                new NewRelease(tag)
                {
                    Name = tag,
                    TargetCommitish = GitHubActions.Instance.Sha,
                    Prerelease = true,
                    Body = $"Automated build from commit {GitHubActions.Instance.Sha}."
                });

            foreach (var file in ArtifactsDirectory.GlobFiles("*.exe"))
            {
                await using var stream = File.OpenRead(file);
                await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(release, new ReleaseAssetUpload
                {
                    FileName = file.Name,
                    ContentType = "application/octet-stream",
                    RawData = stream
                });
            }
        });
}
