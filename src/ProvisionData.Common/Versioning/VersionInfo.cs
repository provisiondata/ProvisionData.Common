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

using System.Collections.Concurrent;
using System.Reflection;

namespace ProvisionData.Versioning;

public sealed class VersionInfo
{
    private static readonly ConcurrentDictionary<Type, VersionInfo> Versions = new();

    private readonly Type _type;

    public static VersionInfo ForAssemblyContaining<T>()
    {
        var key = typeof(T);
        return Versions.GetOrAdd(key, type =>
        {
            var assembly = type.Assembly;
            var typeInfo = assembly.DefinedTypes.SingleOrDefault(t => String.Equals(t.Name, "ThisAssembly", StringComparison.InvariantCulture))
                ?? throw new InvalidOperationException($"The \"{assembly.FullName}\" does not contain a \"ThisAssembly\" type.  Are you sure NerdBank.GitVersioning has been setup correctly?");
            return new VersionInfo(assembly, typeInfo.AsType());
        });
    }

    private VersionInfo(Assembly assembly, Type type)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        _type = type ?? throw new ArgumentNullException(nameof(type));

        AssemblyFullName = assembly.FullName!;
        AssemblyName = AssemblyFullName.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries)[0];
        AssemblyImageRuntimeVersion = assembly.ImageRuntimeVersion;
        AssemblyLocation = assembly.Location;
    }

    private T GetValue<T>(String name)
    {
        var field = _type.GetField(name, BindingFlags.NonPublic | BindingFlags.Static);
        return field is null
            ? default!
            : field.GetValue(null) is T value
                ? value
                : default!;
    }

    private Boolean? _isPrerelease;
    private Boolean? _isPublicRelease;
    private DateTime? _gitCommitAuthorDate;
    private DateTime? _gitCommitDate;

    public Boolean IsPrerelease => _isPrerelease ??= GetValue<Boolean>("IsPrerelease");
    public Boolean IsPublicRelease => _isPublicRelease ??= GetValue<Boolean>("IsPublicRelease");
    public DateTime GitCommitAuthorDate => _gitCommitAuthorDate ??= GetValue<DateTime>("GitCommitAuthorDate");
    public DateTime GitCommitDate => _gitCommitDate ??= GetValue<DateTime>("GitCommitDate");
    public String AssemblyConfiguration => field ??= GetValue<String>("AssemblyConfiguration");
    public String AssemblyFileVersion => field ??= GetValue<String>("AssemblyFileVersion");
    public String AssemblyFullName { get; }
    public String AssemblyImageRuntimeVersion { get; }
    public String AssemblyInformationalVersion => field ??= GetValue<String>("AssemblyInformationalVersion");
    public String AssemblyLocation { get; }
    public String AssemblyName => field ??= GetValue<String>("AssemblyName");
    public String AssemblyTitle => field ??= GetValue<String>("AssemblyTitle");
    public String AssemblyVersion => field ??= GetValue<String>("AssemblyVersion");
    public String GitCommitId => field ??= GetValue<String>("GitCommitId");
    public String NuGetPackageVersion { get => field ??= GetValue<String>("NuGetPackageVersion"); private set; }
    public String RootNamespace => field ??= GetValue<String>("RootNamespace");
}
