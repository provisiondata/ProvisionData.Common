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

#pragma warning disable CS0618 // Type or member is obsolete
namespace ProvisionData
{
    using FluentAssertions;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Xunit;

    public class SystemDateTimeTests
    {
        [Fact]
        public void SystemDateTime_can_override_the_current_date()
        {
            var expected = new DateTime(1974, 10, 16);

            SystemDateTime.DateIs(expected);
            SystemDateTime.UtcNow.Should().Be(expected);

            SystemDateTime.Reset();
            SystemDateTime.UtcNow.Should().NotBe(expected);
        }

        [Fact]
        public void SystemDateTime_can_auto_reset()
        {
            using (var timeProvider = SystemDateTime.GetTestTimeProvider())
            {
                timeProvider.DateIs(1974, 10, 16);
                SystemDateTime.UtcNow.Should().Be(new DateTime(1974, 10, 16));
            }
            SystemDateTime.UtcNow.Should().NotBe(new DateTime(1974, 10, 16));
        }

        [Fact]
        public async Task SystemDateTime_DateIs_prevents_conflicting_tests()
        {
            var tasks = new Task[12];
            for (var i = 0; i < tasks.Length; i++)
            {
                tasks[i] = ThreadProc(new DateTime(i + 1001, i + 1, i + 1), i + 1);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            SystemDateTime.UtcNow.Date.Should().Be(DateTime.UtcNow.Date);

            static async Task ThreadProc(DateTime dateTime, Int32 worker)
            {
                using var timeProvider = SystemDateTime.GetTestTimeProvider();

                Debug.WriteLine($"Worker {worker} wants exclusive access to the clock!");
                timeProvider.DateIs(dateTime);
                Debug.WriteLine($"Worker {worker} got exclusive access to the clock!");

                for (var i = 0; i < 5; i++)
                {
                    Debug.WriteLine($"Worker {worker} date is: {SystemDateTime.UtcNow}");
                    SystemDateTime.UtcNow.Should().Be(dateTime);
                    await Task.Delay(25).ConfigureAwait(false);
                }
            }
        }

        [Fact]
        public void SystemDateTime_DateIs_can_reenter()
        {
            using var timeProvider = SystemDateTime.GetTestTimeProvider();

            timeProvider.DateIs(1975, 11, 11);
            SystemDateTime.UtcNow.Should().Be(new DateTime(1975, 11, 11));

            timeProvider.DateIs(2007, 01, 15);
            SystemDateTime.UtcNow.Should().Be(new DateTime(2007, 01, 15));
        }
    }
}
