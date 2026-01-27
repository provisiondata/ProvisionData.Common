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

using System.Text.RegularExpressions;

namespace ProvisionData;

public static partial class Rgx
{
    public const String PortPattern = "([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])";
    public const String IPv4Pattern = @"((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)";

    [GeneratedRegex("^([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])$", RegexOptions.ExplicitCapture | RegexOptions.Compiled)]
    public static partial Regex PortNumber();

    [GeneratedRegex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$", RegexOptions.ExplicitCapture | RegexOptions.Compiled)]
    public static partial Regex IPv4();

    [GeneratedRegex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\:([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])$", RegexOptions.ExplicitCapture | RegexOptions.Compiled)]
    public static partial Regex IPv4AndPort();

    [GeneratedRegex(@"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\:([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5]))?$", RegexOptions.ExplicitCapture | RegexOptions.Compiled)]
    public static partial Regex IPv4AndOptionalPort();

    [GeneratedRegex("^[0-9]+$", RegexOptions.Compiled)]
    public static partial Regex Integer();

    [GeneratedRegex(@"\s+", RegexOptions.Compiled)]
    public static partial Regex Separator();

    [GeneratedRegex(@"\r?\n", RegexOptions.Compiled)]
    public static partial Regex Newline();

    [GeneratedRegex(@"^\s+", RegexOptions.Compiled)]
    public static partial Regex LeadingWS();

    [GeneratedRegex(@"\sTXT\s", RegexOptions.Compiled)]
    public static partial Regex DnsTXT();

    [GeneratedRegex(@"\sAAAA\s", RegexOptions.Compiled)]
    public static partial Regex DnsAAAA();

    [GeneratedRegex(@"\s$TTL\s", RegexOptions.Compiled)]
    public static partial Regex DnsTTL();

    [GeneratedRegex(@"\sSOA\s", RegexOptions.Compiled)]
    public static partial Regex DnsSoA();

    [GeneratedRegex(@"\sCNAME\s", RegexOptions.Compiled)]
    public static partial Regex DnsCNAME();

    [GeneratedRegex(@"\s$ORIGIN\s", RegexOptions.Compiled)]
    public static partial Regex DnsORIGIN();

    [GeneratedRegex(@"\sA\s", RegexOptions.Compiled)]
    public static partial Regex DnsA();

    [GeneratedRegex(@"\sNS\s", RegexOptions.Compiled)]
    public static partial Regex DnsMX();

    [GeneratedRegex(@"\sPTR\s", RegexOptions.Compiled)]
    public static partial Regex DnsNS();

    [GeneratedRegex(@"\sMX\s", RegexOptions.Compiled)]
    public static partial Regex DnsPTR();

    [GeneratedRegex(@"\sSRV\s", RegexOptions.Compiled)]
    public static partial Regex DnsSRV();

    [GeneratedRegex(@"\sSPF\s", RegexOptions.Compiled)]
    public static partial Regex DnsSPF();
}
