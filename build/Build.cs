using App.Updates.Feed;
using Fallout.Common;
using Fallout.Common.CI.GitHubActions;
using Fallout.Common.Git;
using Fallout.Common.IO;
using Fallout.Common.Tools.DotNet;
using Fallout.Common.Tools.GitHub;
using Fallout.Common.Utilities.Collections;
using Fallout.Solutions;
using Octokit;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static Fallout.Common.EnvironmentInfo;
using static Fallout.Common.Tools.DotNet.DotNetTasks;

// [GitHubActions] generates .github/workflows/release.yml from the attribute below; never hand-edit the .yml.
[GitHubActions(
    "release",
    GitHubActionsImage.WindowsLatest,
    OnPushBranches = new[] { "master" },
    InvokedTargets = new[] { nameof(publishRelease) },
    EnableGitHubToken = true,
    WritePermissions = new[] { GitHubActionsPermissions.Contents },
    PublishArtifacts = false)]
internal class Build : FalloutBuild
{
    public static int Main() => Execute<Build>(x => x.test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)", Name = "Configuration")]
    private readonly Configuration _configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution(Name = "Solution")] private readonly Solution _solution;
    [GitRepository] private readonly GitRepository _gitRepository;

    [Parameter("GitHub token used to create the release - set via GITHUB_TOKEN, provided automatically in Actions", Name = "GitHubToken")]
    [Secret]
    private readonly string _gitHubToken = GitHubActions.Instance?.Token;

    private AbsolutePath artifactsDirectory => RootDirectory / "artifacts";
    private AbsolutePath bundleProject => RootDirectory / "installer" / "App.Installer.Bundle" / "App.Installer.Bundle.wixproj";

    private string releaseTag => BuildTag.Format(GitHubActions.Instance?.RunNumber ?? 0);

    private string gitHubOwner => _gitRepository.GetGitHubOwner();
    private string gitHubRepoName => _gitRepository.GetGitHubName();

    private Target clean => _ => _
        .Before(restore)
        .Executes(() =>
        {
            RootDirectory.GlobDirectories("src/**/bin", "src/**/obj", "tests/**/bin", "tests/**/obj", "installer/**/bin", "installer/**/obj").ForEach(x => x.DeleteDirectory());
            artifactsDirectory.CreateOrCleanDirectory();
        });

    private Target restore => _ => _
        .Executes(() => DotNetRestore(s => s
            .SetProjectFile(_solution)));

    private Target acceptWixEula => _ => _
        .DependsOn(restore)
        .Executes(() =>
        {
            var nugetPackages = GetVariable("NUGET_PACKAGES")
                ?? Path.Combine(GetVariable("USERPROFILE") ?? GetVariable("HOME"), ".nuget", "packages");

            var wixExe = Directory.GetFiles(nugetPackages, "wix.exe", SearchOption.AllDirectories)
                .FirstOrDefault(x => x.Contains("wixtoolset.sdk", StringComparison.OrdinalIgnoreCase));

            if (wixExe == null)
            {
                throw new InvalidOperationException(
                    "wix.exe not found under the NuGet packages folder after restore - was WixToolset.Sdk restored?");
            }

            using (var process = Process.Start(new ProcessStartInfo(wixExe, "eula accept wix7") { UseShellExecute = false }))
            {
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException("'wix eula accept wix7' failed");
                }
            }
        });

    private Target verifyFormat => _ => _
        .DependsOn(restore)
        .Executes(() => DotNetFormat(s => s
            .SetProject(_solution)
            .EnableVerifyNoChanges()));

    private Target compile => _ => _
        .DependsOn(acceptWixEula, verifyFormat)
        .Executes(() => DotNetBuild(s => s
            .SetProjectFile(_solution)
            .SetConfiguration(_configuration)
            .SetProperty("InformationalVersion", releaseTag)
            .SetProperty("WixDotnetStarterGitHubOwner", gitHubOwner)
            .SetProperty("WixDotnetStarterGitHubRepo", gitHubRepoName)
            .EnableNoRestore()));

    private Target test => _ => _
        .DependsOn(compile)
        .Executes(() => DotNetTest(s => s
            .SetProjectFile(_solution)
            .SetConfiguration(_configuration)
            .EnableNoBuild()));

    private Target packBundle => _ => _
        .DependsOn(compile)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(bundleProject)
                .SetConfiguration(_configuration));

            artifactsDirectory.CreateOrCleanDirectory();
            bundleProject.Parent.GlobFiles($"bin/x64/{_configuration}/*.exe")
                .ForEach(x => x.CopyToDirectory(artifactsDirectory, ExistsPolicy.FileOverwriteIfNewer));
        });

    private Target publishRelease => _ => _
        .DependsOn(packBundle)
        .Requires(() => _gitHubToken)
        .Executes(async () =>
        {
            GitHubTasks.GitHubClient.Credentials = new Credentials(_gitHubToken);

            var tag = releaseTag;
            var release = await GitHubTasks.GitHubClient.Repository.Release.Create(
                gitHubOwner,
                gitHubRepoName,
                new NewRelease(tag)
                {
                    Name = tag,
                    TargetCommitish = GitHubActions.Instance.Sha,
                    Prerelease = true,
                    Body = $"Automated build from commit {GitHubActions.Instance.Sha}."
                });

            foreach (var file in artifactsDirectory.GlobFiles("*.exe"))
            {
                await using (var stream = File.OpenRead(file))
                {
                    await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(
                        release,
                        new ReleaseAssetUpload
                        {
                            FileName = file.Name,
                            ContentType = "application/octet-stream",
                            RawData = stream
                        }
                    );
                }
            }
        });
}