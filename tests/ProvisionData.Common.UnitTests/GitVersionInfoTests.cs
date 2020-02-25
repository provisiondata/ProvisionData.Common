/*******************************************************************************
 * MIT License
 *
 * Copyright 2020 Provision Data Systems Inc.  https://provisiondata.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 *******************************************************************************/

namespace ProvisionData.UnitTests
{
    using FluentAssertions;
    using ProvisionData.GitVersion;
    using Xunit;

    public class GitVersionInfoTests
    {
        [Fact]
        public void Works()
        {
            var gvi = GitVersionInfo.ForAssemblyContaining<GitVersionInfo>();

            gvi.Major.Should().NotBeEmpty();
            gvi.Minor.Should().NotBeEmpty();
            gvi.Patch.Should().NotBeEmpty();
            gvi.FullBuildMetaData.Should().NotBeEmpty();
            gvi.MajorMinorPatch.Should().NotBeEmpty();
            gvi.SemVer.Should().NotBeEmpty();
            gvi.LegacySemVer.Should().NotBeEmpty();
            gvi.LegacySemVerPadded.Should().NotBeEmpty();
            gvi.AssemblySemVer.Should().NotBeEmpty();
            gvi.AssemblySemFileVer.Should().NotBeEmpty();
            gvi.FullSemVer.Should().NotBeEmpty();
            gvi.InformationalVersion.Should().NotBeEmpty();
            gvi.BranchName.Should().NotBeEmpty();
            gvi.Sha.Should().NotBeEmpty();
            gvi.ShortSha.Should().NotBeEmpty();
            gvi.NuGetVersionV2.Should().NotBeEmpty();
            gvi.NuGetVersion.Should().NotBeEmpty();
            gvi.VersionSourceSha.Should().NotBeEmpty();
            gvi.CommitsSinceVersionSource.Should().NotBeEmpty();
            gvi.CommitsSinceVersionSourcePadded.Should().NotBeEmpty();
            gvi.CommitDate.Should().NotBeEmpty();
        }
    }
}
