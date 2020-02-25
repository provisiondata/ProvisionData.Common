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
    using ProvisionData.Net;
    using Shouldly;
    using Xunit;

    public class EndPointTests
    {
        [Fact]
        public void EndPoints_are_equatable()
        {
            new EndPoint("10.10.10.10").Equals(new EndPoint("10.10.10.10")).ShouldBe(true);
            new EndPoint("10.10.10.10").Equals(new EndPoint("10.10.10.11")).ShouldBe(false);

            new EndPoint("10.10.10.10", 10).Equals(new EndPoint("10.10.10.10", 10)).ShouldBe(true);
            new EndPoint("10.10.10.10", 10).Equals(new EndPoint("10.10.10.10", 11)).ShouldBe(false);

            (new EndPoint("10.10.10.10") == new EndPoint("10.10.10.10")).ShouldBe(true);
            (new EndPoint("10.10.10.10") != new EndPoint("10.10.10.11")).ShouldBe(true);

            (new EndPoint("10.10.10.10", 10) == new EndPoint("10.10.10.10", 10)).ShouldBe(true);
            (new EndPoint("10.10.10.10", 10) != new EndPoint("10.10.10.10", 11)).ShouldBe(true);
        }

        [Fact]
        public void EndPoints_with_same_Address_and_one_with_Port_0_are_equal()
        {
            new EndPoint("10.10.10.10", 0).Equals(new EndPoint("10.10.10.10", 1000)).ShouldBe(true);
        }

        [Fact]
        public void EndPoints_are_compareable()
        {
            new EndPoint("110.10.10.10").ShouldBeLessThan(new EndPoint("120.10.10.10"));
            new EndPoint("110.10.10.10").ShouldBeGreaterThan(new EndPoint("100.10.10.10"));

            new EndPoint("110.10.10.10").ShouldBeLessThan(new EndPoint("110.10.10.10", 100));
            new EndPoint("110.10.10.10", 100).ShouldBeGreaterThan(new EndPoint("110.10.10.10"));

            new EndPoint("110.10.10.10", 1000).ShouldBeLessThan(new EndPoint("110.10.10.10", 2000));
            new EndPoint("110.10.10.10", 2000).ShouldBeGreaterThan(new EndPoint("110.10.10.10", 1000));
        }
    }
}
