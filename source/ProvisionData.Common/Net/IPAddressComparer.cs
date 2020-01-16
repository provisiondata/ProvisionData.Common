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

namespace ProvisionData.Net
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class IPAddressComparer : IComparer<String>
    {
        Int32 IComparer<String>.Compare(String left, String right)
            => IPAddressComparer.Compare(left, right);

        public static Int32 Compare(String left, String right)
        {
            var l = Split(left);
            var r = Split(right);

            if (l[0] == r[0])
            {
                if (l[1] == r[1])
                {
                    if (l[2] == r[2])
                    {
                        return l[3].CompareTo(r[3]);
                    }
                    return l[2].CompareTo(r[2]);
                }
                return l[1].CompareTo(r[1]);
            }
            return l[0].CompareTo(r[0]);
        }

        private static Int32[] Split(String value)
            => value.Split('.').Select(Int32.Parse).ToArray();
    }
}
