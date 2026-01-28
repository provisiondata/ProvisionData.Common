// ProvisionData.Common
// Copyright (C) 2026 Provision Data Systems Inc.
//
// This program is free software: you can redistribute it and/or modify it under the terms of
// the GNU Affero General Public License as published by the Free Software Foundation, either
// version 3 of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License along with this
// program. If not, see <https://www.gnu.org/licenses/>.

using ProvisionData.Versioning;

namespace ProvisionData.UnitTests;

public class VersionInfoTests(ITestOutputHelper output)
{
    [Fact]
    public void ForAssemblyContaining_ShouldReturn_VersionInfo()
    {
        var version = VersionInfo.ForAssemblyContaining<VersionInfo>();

        output.WriteLine($"AssemblyConfiguration: {version.AssemblyConfiguration}");

#if DEBUG
        version.IsPrerelease.Should().BeTrue();
        version.IsPublicRelease.Should().BeFalse();
#else
        version.IsPrerelease.Should().BeFalse();
        version.IsPublicRelease.Should().BeTrue();
#endif
        version.AssemblyConfiguration.Should().BeOneOf("Debug", "Release");
        version.AssemblyFileVersion.Should().StartWith("4");
        version.AssemblyFullName.Should().NotBeEmpty();
        version.AssemblyImageRuntimeVersion.Should().StartWith("v4");
        version.AssemblyInformationalVersion.Should().StartWith("4");
        version.AssemblyLocation.Should().NotBeEmpty();
        version.AssemblyName.Should().Be("ProvisionData.Common");
        version.AssemblyTitle.Should().Be("ProvisionData.Common");
        version.AssemblyVersion.Should().StartWith("4");
        version.GitCommitAuthorDate.Should().BeAfter(new DateTime(2026, 1, 1));
        version.GitCommitDate.Should().BeAfter(new DateTime(2026, 1, 1));
        version.GitCommitId.Should().NotBeEmpty();
        version.NuGetPackageVersion.Should().StartWith("4");
        version.RootNamespace.Should().Be("ProvisionData");
    }
}
