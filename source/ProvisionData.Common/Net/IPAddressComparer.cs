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
        Int32 IComparer<String>.Compare(String x, String y)
        {
            return IPAddressComparer.Compare(x, y);
        }

        public static Int32 Compare(String x, String y)
        {
            var xT = Split(x);
            var yT = Split(y);

            if (xT[0] == yT[0])
            {
                if (xT[1] == yT[1])
                {
                    if (xT[2] == yT[2])
                    {
                        return xT[3].CompareTo(yT[3]);
                    }
                    return xT[2].CompareTo(yT[2]);
                }
                return xT[1].CompareTo(yT[1]);
            }
            return xT[0].CompareTo(yT[0]);
        }

        private static Int32[] Split(String value) => value.Split('.').Select(token => Int32.Parse(token)).ToArray();
    }
}
