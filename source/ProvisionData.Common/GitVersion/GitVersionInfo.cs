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

namespace ProvisionData.GitVersion
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;

    public sealed class GitVersionInfo
    {
        private static readonly ConcurrentDictionary<Type, GitVersionInfo> Versions = new ConcurrentDictionary<Type, GitVersionInfo>();

        private readonly Type _type;
        private String _commitDate;
        private String _major;
        private String _minor;
        private String _patch;
        private String _preReleaseTag;
        private String _preReleaseTagWithDash;
        private String _preReleaseLabel;
        private String _preReleaseNumber;
        private String _weightedPreReleaseNumber;
        private String _buildMetaData;
        private String _buildMetaDataPadded;
        private String _fullBuildMetaData;
        private String _majorMinorPatch;
        private String _semVer;
        private String _legacySemVer;
        private String _legacySemVerPadded;
        private String _assemblySemVer;
        private String _assemblySemFileVer;
        private String _fullSemVer;
        private String _informationalVersion;
        private String _branchName;
        private String _sha;
        private String _shortSha;
        private String _nuGetVersionV2;
        private String _nuGetVersion;
        private String _nuGetPreReleaseTagV2;
        private String _nuGetPreReleaseTag;
        private String _versionSourceSha;
        private String _commitsSinceVersionSource;

        public static GitVersionInfo ForAssemblyContaining<T>()
        {
            var key = typeof(T);
            return Versions.GetOrAdd(key, type =>
            {
                var assembly = type.Assembly;
                var typeInfo = assembly.DefinedTypes.SingleOrDefault(t => String.Equals(t.Name, "GitVersionInformation", StringComparison.InvariantCulture));
                if (typeInfo is null)
                {
                    throw new InvalidOperationException($"The \"{assembly.FullName}\" does not contain a \"GitVersionInformation\" type.  Are you sure GitVersion has been setup correctly?");
                }
                return new GitVersionInfo(assembly, typeInfo.AsType());
            });
        }

        private GitVersionInfo(Assembly assembly, Type type)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));
            _type = type ?? throw new ArgumentNullException(nameof(type));

            AssemblyFullName = assembly.FullName;
            AssemblyName = AssemblyFullName.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
            AssemblyImageRuntimeVersion = assembly.ImageRuntimeVersion;
            AssemblyLocation = assembly.Location;
        }

        private String GetValue(String name)
            => _type.GetField(name).GetValue(null).ToString();

        public String AssemblyFullName { get; }

        public String AssemblyName { get; }

        public String AssemblyImageRuntimeVersion { get; }

        public String AssemblyLocation { get; }

        public String Major
            => _major ?? (_major = GetValue("Major"));

        public String Minor
            => _minor ?? (_minor = GetValue("Minor"));

        public String Patch
            => _patch ?? (_patch = GetValue("Patch"));

        public String PreReleaseTag
            => _preReleaseTag ?? (_preReleaseTag = GetValue("PreReleaseTag"));

        public String PreReleaseTagWithDash
            => _preReleaseTagWithDash ?? (_preReleaseTagWithDash = GetValue("PreReleaseTagWithDash"));

        public String PreReleaseLabel => _preReleaseLabel ?? (_preReleaseLabel = GetValue("PreReleaseLabel"));

        public String PreReleaseNumber
            => _preReleaseNumber ?? (_preReleaseNumber = GetValue("PreReleaseNumber"));

        public String WeightedPreReleaseNumber
            => _weightedPreReleaseNumber ?? (_weightedPreReleaseNumber = GetValue("WeightedPreReleaseNumber"));

        public String BuildMetaData
            => _buildMetaData ?? (_buildMetaData = GetValue("BuildMetaData"));

        public String BuildMetaDataPadded
            => _buildMetaDataPadded ?? (_buildMetaDataPadded = GetValue("BuildMetaDataPadded"));

        public String FullBuildMetaData
            => _fullBuildMetaData ?? (_fullBuildMetaData = GetValue("FullBuildMetaData"));

        public String MajorMinorPatch
            => _majorMinorPatch ?? (_majorMinorPatch = GetValue("MajorMinorPatch"));

        public String SemVer
            => _semVer ?? (_semVer = GetValue("SemVer"));

        public String LegacySemVer
            => _legacySemVer ?? (_legacySemVer = GetValue("LegacySemVer"));

        public String LegacySemVerPadded
            => _legacySemVerPadded ?? (_legacySemVerPadded = GetValue("LegacySemVerPadded"));

        public String AssemblySemVer
            => _assemblySemVer ?? (_assemblySemVer = GetValue("AssemblySemVer"));

        public String AssemblySemFileVer
            => _assemblySemFileVer ?? (_assemblySemFileVer = GetValue("AssemblySemFileVer"));

        public String FullSemVer
            => _fullSemVer ?? (_fullSemVer = GetValue("FullSemVer"));

        public String InformationalVersion
            => _informationalVersion ?? (_informationalVersion = GetValue("InformationalVersion"));

        public String BranchName
            => _branchName ?? (_branchName = GetValue("BranchName"));

        public String Sha
            => _sha ?? (_sha = GetValue("Sha"));

        public String ShortSha
            => _shortSha ?? (_shortSha = GetValue("ShortSha"));

        public String NuGetVersionV2
            => _nuGetVersionV2 ?? (_nuGetVersionV2 = GetValue("NuGetVersionV2"));

        public String NuGetVersion
            => _nuGetVersion ?? (_nuGetVersion = GetValue("NuGetVersion"));

        public String NuGetPreReleaseTagV2
            => _nuGetPreReleaseTagV2 ?? (_nuGetPreReleaseTagV2 = GetValue("NuGetPreReleaseTagV2"));

        public String NuGetPreReleaseTag
            => _nuGetPreReleaseTag ?? (_nuGetPreReleaseTag = GetValue("NuGetPreReleaseTag"));

        public String VersionSourceSha
            => _versionSourceSha ?? (_versionSourceSha = GetValue("VersionSourceSha"));

        public String CommitsSinceVersionSource
            => _commitsSinceVersionSource ?? (_commitsSinceVersionSource = GetValue("CommitsSinceVersionSource"));

        private String _commitsSinceVersionSourcePadded;

        public String CommitsSinceVersionSourcePadded
            => _commitsSinceVersionSourcePadded ?? (_commitsSinceVersionSourcePadded = GetValue("CommitsSinceVersionSourcePadded"));

        public String CommitDate
            => _commitDate ?? (_commitDate = GetValue("CommitDate"));
    }
}
