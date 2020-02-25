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

namespace ProvisionData.UnitTests.Net
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using ProvisionData.Net;
    using Xunit;

    public class IPAddressComparerTests
    {
        [Fact]
        public void IPAddressComparer_works_as_expected()
        {
            var a = new[] { "204.63.46.18", "110.249.212.46", "51.15.144.131", "8.8.8.8", "204.63.42.225", "193.200.164.173" };
            var b = new[] { "8.8.8.8", "51.15.144.131", "110.249.212.46", "193.200.164.173", "204.63.42.225", "204.63.46.18" };

            var list = new List<String>(a);

            list.Sort(new IPAddressComparer());

            list.Should().ContainInOrder(b);
        }
    }
}
