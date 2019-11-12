using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.GitHub.ChangeLogExtensions;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = Configuration.Release;
    [Parameter("Explicit framework to build")] readonly String Framework;
    [Parameter("NuGet API Key")] readonly String NuGetApiKey;
    [Parameter("Pdsi API Key")] readonly String PdsiApiKey;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "source";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    String ChangeLogFile => RootDirectory / "CHANGELOG.md";

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Publish => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPublish(_ => _
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore());
        });

    Target Test => _ => _
               .DependsOn(Publish)
               .Executes(() =>
               {
                   DotNetTest(s => s
                       .SetConfiguration(Configuration)
                       .EnableNoBuild()
                       .EnableNoRestore()
                       .SetLogger("trx")
                       .SetLogOutput(true)
                       .SetArgumentConfigurator(arguments => arguments.Add("/p:UseSourceLink={0}", "true"))
                       .SetResultsDirectory(TestsDirectory / "results"));
               });

    Target Pack => _ => _
       .DependsOn(Publish)
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
