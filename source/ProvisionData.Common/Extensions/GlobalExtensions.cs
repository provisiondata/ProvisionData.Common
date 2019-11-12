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

namespace ProvisionData.Extensions
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    [DebuggerNonUserCode]
    public static class GlobalExtensions
    {
        public static Boolean As<TInterface>(this Object implementation, Action<TInterface> doThis, Action otherwiseThis = null)
        {
            if (doThis == null)
            {
                throw new ArgumentNullException(nameof(doThis));
            }

            if (implementation is TInterface impl)
            {
                doThis(impl);
                return true;
            }
            else
            {
                otherwiseThis?.Invoke();
                return false;
            }
        }

        [SuppressMessage("Reliability", "CA2007:Consider calling ConfigureAwait on the awaited task", Justification = "We don't call ConfigureAwait(false) because the caller can do it if they need/want to.")]
        [SuppressMessage("Design", "RCS1090:Call 'ConfigureAwait(false)'.", Justification = "We don't call ConfigureAwait(false) because the caller can do it if they need/want to.")]
        public static async Task<Boolean> AsAsync<TInterface>(this Object implementation, Func<TInterface, Task> action, Func<Task> falseAction = null)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (implementation is TInterface impl)
            {
                await action(impl);
                return true;
            }
            else
            {
                if (!(falseAction is null))
                {
                    await falseAction();
                }

                return false;
            }
        }
    }
}
