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
    using System.Diagnostics;
    using System.Threading;

    [Obsolete("This will be removed in v2.0.  Use NodaTime.IClock instead because Jon Skeet is awesome!")]
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }

    [Obsolete("This will be removed in v2.0.  Use NodaTime.IClock instead because Jon Skeet is awesome!")]
    public interface ITestableTimeProvider : ITimeProvider, IDisposable
    {
        void DateIs(DateTime date);
        void DateIs(Int32 year, Int32 month, Int32 day);
        void Reset();
    }

    [DebuggerNonUserCode]
    [Obsolete("This will be removed in v2.0.  Use NodaTime.IClock instead because Jon Skeet is awesome!")]
    public static class SystemDateTime
    {
        private static readonly Semaphore Pool = new Semaphore(1, 1);

        private static Func<DateTime> GetTime = GetUtc;

        public static DateTime UtcNow => GetTime();

        private static DateTime GetUtc() => new DateTime(DateTime.UtcNow.Ticks, DateTimeKind.Unspecified);

        [Obsolete("This will be removed in v2.0.  Use NodaTime.IClock instead because Jon Skeet is awesome!")]
        public static void DateIs(DateTime dateTime) => GetTime = () => new DateTime(dateTime.Ticks, DateTimeKind.Unspecified);

        [Obsolete("This will be removed in v2.0.  Use NodaTime.IClock instead because Jon Skeet is awesome!")]
        public static void DateIs(Int32 year, Int32 month = 1, Int32 day = 1) => DateIs(new DateTime(year, month, day));

        [Obsolete("This will be removed in v2.0.  Use NodaTime.IClock instead because Jon Skeet is awesome!")]
        public static void Reset() => GetTime = GetUtc;

        public static ITestableTimeProvider GetTestTimeProvider() => new TestableTimeProvider();

        private class TestableTimeProvider : ITestableTimeProvider
        {
            private Boolean _locked;

            public DateTime UtcNow => SystemDateTime.UtcNow;

            public void DateIs(DateTime dateTime)
            {
                if (!_locked)
                {
                    _locked = Pool.WaitOne();
                }

                SystemDateTime.DateIs(dateTime);
            }

            public void DateIs(Int32 year, Int32 month = 1, Int32 day = 1) => DateIs(new DateTime(year, month, day));

            public void Dispose()
            {
                SystemDateTime.Reset();
                if (_locked)
                {
                    Pool.Release();
                }
            }

            public void Reset() => SystemDateTime.Reset();
        }
    }
}
