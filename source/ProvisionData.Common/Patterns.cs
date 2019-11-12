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

namespace ProvisionData
{
    using System;
    using System.Text.RegularExpressions;

    public static class Ptn
    {
        private const String PortPattern = "([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])";
        private const String IPv4Pattern = @"((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)";

        public static readonly Regex PortNumber = new Regex($"^{PortPattern}$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        public static readonly Regex IPv4 = new Regex($"^{IPv4Pattern}$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        public static readonly Regex IPv4AndPort = new Regex($"^{IPv4Pattern}\\:{PortPattern}$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        public static readonly Regex IPv4AndOptionalPort = new Regex($"^{IPv4Pattern}(\\:{PortPattern})?$", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        public static readonly Regex Integer = new Regex("^[0-9]+$", RegexOptions.Compiled);
        public static readonly Regex Separator = new Regex(@"\s+", RegexOptions.Compiled);
        public static readonly Regex Newline = new Regex(@"\r?\n", RegexOptions.Compiled);
        public static readonly Regex LeadingWS = new Regex(@"^\s+", RegexOptions.Compiled);
        public static readonly Regex TXT = new Regex(@"\sTXT\s", RegexOptions.Compiled);
        public static readonly Regex ORIGIN = new Regex(@"\s$ORIGIN\s", RegexOptions.Compiled);
        public static readonly Regex TTL = new Regex(@"\s$TTL\s", RegexOptions.Compiled);
        public static readonly Regex SOA = new Regex(@"\sSOA\s", RegexOptions.Compiled);
        public static readonly Regex NS = new Regex(@"\sNS\s", RegexOptions.Compiled);
        public static readonly Regex A = new Regex(@"\sA\s", RegexOptions.Compiled);
        public static readonly Regex AAAA = new Regex(@"\sAAAA\s", RegexOptions.Compiled);
        public static readonly Regex CNAME = new Regex(@"\sCNAME\s", RegexOptions.Compiled);
        public static readonly Regex MX = new Regex(@"\sMX\s", RegexOptions.Compiled);
        public static readonly Regex PTR = new Regex(@"\sPTR\s", RegexOptions.Compiled);
        public static readonly Regex SRV = new Regex(@"\sSRV\s", RegexOptions.Compiled);
        public static readonly Regex SPF = new Regex(@"\sSPF\s", RegexOptions.Compiled);
    }
}
