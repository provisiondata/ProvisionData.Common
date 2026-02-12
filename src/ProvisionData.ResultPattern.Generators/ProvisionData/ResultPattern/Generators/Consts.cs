// Provision Data Application Framework
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

namespace ProvisionData.ResultPattern.Generators;

/// <summary>
/// Constants shared between the runtime library and code generators.
/// </summary>
internal class Consts
{
    public const String Company = "ProvisionData";
    public const String Pattern = "ResultPattern";
    public const String Error = "Error";
    public const String ErrorCode = "ErrorCode";
    public const String Instance = "Instance";
    public const String Initializer = "Initializer";
    public const String Registry = "Registry";
    public const String Polymorphism = "Polymorphism";
    public const String Constructor = "Constructors";
    public const String Infrastructure = "Infrastructure";
    public const String Extension = ".g.cs";
    public const String Register = "Register";

    public const String PatternNamespace = Company + "." + Pattern;
    public const String FqPatternNamespace = "global::" + PatternNamespace;

    public const String FqErrorType = FqPatternNamespace + "." + Error;
    public const String FqErrorCodeType = FqPatternNamespace + "." + ErrorCode;
    public const String FqInfrastructureNamespace = FqPatternNamespace + "." + Infrastructure;
    public const String FqRegistryType = FqInfrastructureNamespace + "." + Registry;
    public const String FqPolymorphismType = FqInfrastructureNamespace + "." + Polymorphism;
    public const String PolymorphismInitializer = Polymorphism + Initializer;

    public const String RegistryInitializerType = Pattern + "_" + Registry + Initializer;

    public const String ConstructorFilename = Pattern + "_" + Constructor + Extension;
    public const String InitializerFilename = Pattern + "_" + Initializer + Extension;
    public const String ErrorCodeFilename = Pattern + "_" + ErrorCode + Extension;

    public const String TypeDiscriminatorPropertyName = "TypeDiscriminatorPropertyName";
    public const String ErrorPolymorphismInitializerFilename = "ErrorPolymorphism" + Extension;

    public const String SerivceCollectionExtensionMethod = "Add" + Pattern;
    public const String ResultPatternServiceCollectionExtensions = Pattern + "ServiceCollectionExtensions";
}
