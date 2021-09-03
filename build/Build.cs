using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
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

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[DotNetVerbosityMapping]
class Build : NukeBuild
{
	/// Support plugins are available for:
	///   - JetBrains ReSharper        https://nuke.build/resharper
	///   - JetBrains Rider            https://nuke.build/rider
	///   - Microsoft VisualStudio     https://nuke.build/visualstudio
	///   - Microsoft VSCode           https://nuke.build/vscode

	public static int Main() => Execute<Build>(x => x.Compile);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	readonly Configuration Configuration = Configuration.Release;  //IsLocalBuild ? Configuration.Debug : Configuration.Release;
	[Parameter("NuGet server URL.")]
	readonly String NugetSource = "https://api.nuget.org/v3/index.json";
	[Parameter("NuGet API Key.")]
	readonly String NugetApiKey;
	[Parameter("NuGet package version.")]
	readonly String PackageVersion;

	[Solution(GenerateProjects = true)] readonly Solution Solution;
	[GitRepository] readonly GitRepository GitRepository;
	[GitVersion(Framework = "net5.0")] readonly GitVersion GitVersion;

	AbsolutePath SourceDirectory => RootDirectory / "source";
	AbsolutePath TestsDirectory => RootDirectory / "tests";
	AbsolutePath OutputDirectory => RootDirectory / "output";


	Project Common => Solution.GetProject("ProvisionData.Common");
	Project EFCore => Solution.GetProject("ProvisionData.EntityFrameworkCore");
	Project GELF => Solution.GetProject("ProvisionData.GELF");
	Project HTML => Solution.GetProject("ProvisionData.Html");
	Project Serilog => Solution.GetProject("ProvisionData.Serilog");
	Project Specs => Solution.GetProject("ProvisionData.Specifications");
	Project SpecsEFCore => Solution.GetProject("ProvisionData.Specifications.EntityFrameworkCore");
	Project Testing => Solution.GetProject("ProvisionData.UnitTesting");

	Project[] Projects => new[] { Common, EFCore, GELF, HTML, Serilog, Specs, SpecsEFCore, Testing };

	Target Clean => _ => _
		.Before(Restore)
		.Executes(() =>
		{
			SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
			TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
			EnsureCleanDirectory(OutputDirectory);
		});

	Target Restore => _ => _
		.Executes(() =>
		{
			DotNetRestore(s => s
				.SetProjectFile(Solution)
			);
		});

	Target Compile => _ => _
		.DependsOn(Restore)
		.Executes(() =>
		{
			DotNetBuild(s => s
				.EnableNoRestore()
				.SetProjectFile(Solution)
				.SetConfiguration(Configuration)
				.SetAssemblyVersion(GitVersion.AssemblySemVer)
				.SetFileVersion(GitVersion.AssemblySemFileVer)
				.SetInformationalVersion(GitVersion.InformationalVersion)
			);

			DotNetPublish(s => s
				.EnableNoRestore()
				.EnableNoBuild()
				.SetConfiguration(Configuration)
				.SetAssemblyVersion(GitVersion.AssemblySemVer)
				.SetFileVersion(GitVersion.AssemblySemFileVer)
				.SetInformationalVersion(GitVersion.InformationalVersion)
				.CombineWith(
					from project in Projects
					from framework in project.GetTargetFrameworks()
					select new { project, framework }, (cs, v) => cs
						.SetProject(v.project)
						.SetFramework(v.framework)
				)
			);
		});

	Target Test => _ => _
		.DependsOn(Compile)
		.Executes(() =>
		{
			DotNetTest(s => s
				.SetProjectFile(Solution)
				.SetConfiguration(Configuration)
				.EnableNoRestore()
				.EnableNoBuild()
			);
		});

	Target Pack => _ => _
		.DependsOn(Clean, Test)
		.Requires(() => Configuration == Configuration.Release)
		.Executes(() =>
		{
			DotNetPack(s => s
				.EnableNoRestore()
				.EnableNoBuild()
				.SetConfiguration(Configuration)
				.SetOutputDirectory(OutputDirectory)
				.CombineWith(Projects, (s, p) => s
					.SetProject(p)
					.SetProperty("PackageVersion", PackageVersion ?? GitVersion.NuGetVersionV2))

			);
		});

	Target Push => _ => _
		.DependsOn(Pack)
		.Requires(() => Configuration == Configuration.Release)
		.Requires(() => !String.IsNullOrWhiteSpace(NugetApiKey))
		.Executes(() =>
		{
			DotNetNuGetPush(s => s
				.SetSource(NugetSource)
				.SetApiKey(NugetApiKey)
				.CombineWith(OutputDirectory.GlobFiles("*.nupkg"), (s, v) => s
					.SetTargetPath(v)
				)
			);
		});
}
