using Fallout.Common;
using Fallout.Common.Execution;
using Fallout.Common.IO;
using Fallout.Common.Tooling;
using Fallout.Common.Tools.DotNet;
using Fallout.Common.Utilities.Collections;
using Fallout.Solutions;
using static Fallout.Common.EnvironmentInfo;
using static Fallout.Common.Tools.DotNet.DotNetTasks;

// Showcases the full pipeline: Clean -> Restore -> Compile -> Test -> PackBundle.
// `Compile` builds everything WixDotnetStarter.slnx references by default - App.Core,
// App.WinForms, App.Core.Tests and App.Installer.msi (App.Bundle is excluded from the
// solution's default build via <Build Project="false" />, see the .slnx). `PackBundle`
// builds it explicitly, needing the .NET Desktop Runtime installer downloaded first -
// see installer/App.Bundle/Redist/README.md - so it's not part of the default target.
class Build : FalloutBuild
{
    public static int Main() => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

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

    Target Compile => _ => _
        .DependsOn(Restore)
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
}
