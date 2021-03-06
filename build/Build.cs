using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using System;
using System.Linq;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.GitHub.ChangeLogExtensions;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    public static Int32 Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Explicit framework to build")] readonly String Framework;
    [Parameter("NuGet API Key")] readonly String NuGetApiKey;
    [Parameter("Pdsi API Key")] readonly String PdsiApiKey;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    String ChangeLogFile => RootDirectory / "CHANGELOG.md";

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.GetNormalizedAssemblyVersion())
                .SetFileVersion(GitVersion.GetNormalizedFileVersion())
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target Test => _ => _
           .DependsOn(Compile)
           .Executes(() =>
           {
               DotNetTest(s => s
                   .SetConfiguration(Configuration)
                   .EnableNoBuild()
                   .EnableNoRestore()
                   .SetLogger("trx")
                   .SetLogOutput(true)
                   .SetFramework(Framework)
                   .SetArgumentConfigurator(arguments => arguments.Add("/p:UseSourceLink={0}", "true"))
                   .SetResultsDirectory(ArtifactsDirectory / "tests"));
           });

    Target Pack => _ => _
        .DependsOn(Test)
        .Executes(() =>
        {
            var changeLog = GetCompleteChangeLog(ChangeLogFile)
               .EscapeStringPropertyForMsBuild();

            DotNetPack(s => s
                .SetConfiguration(Configuration)
                .EnableIncludeSymbols()
                .EnableNoBuild()
                .EnableNoRestore()
                .SetOutputDirectory(ArtifactsDirectory / "nuget")
                .SetPackageReleaseNotes(changeLog));
        });

    Target PublishToNuGet => _ => _
         .DependsOn(Pack)
         .Requires(() => NuGetApiKey)
         .Requires(() => Equals(Configuration, Configuration.Release))
         .Executes(() =>
         {
             GlobFiles(ArtifactsDirectory / "nuget", "*.nupkg")
             .NotEmpty()
             .Where(x => !x.EndsWith(".symbols.nupkg"))
             .ForEach(x => DotNetNuGetPush(s => s
                 .SetTargetPath(x)
                 .SetSource("https://api.nuget.org/v3/index.json")
                 .SetApiKey(NuGetApiKey)));
         });

    Target PublishToPdsi => _ => _
         .DependsOn(Pack)
         .Requires(() => PdsiApiKey)
         .Requires(() => Equals(Configuration, Configuration.Release))
         .Executes(() =>
         {
             GlobFiles(ArtifactsDirectory / "nuget", "*.nupkg")
                 .NotEmpty()
                 .ForEach(x => DotNetNuGetPush(s => s
                     .SetTargetPath(x)
                     .SetSource("https://baget.pdsint.net/v3/index.json")
                     .SetApiKey(PdsiApiKey)));
         });
}
