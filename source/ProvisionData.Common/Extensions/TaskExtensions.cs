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
	using System.Threading.Tasks;

	// https://www.meziantou.net/fire-and-forget-a-task-in-dotnet.htm
	public static class TaskExtensions
	{
		/// <summary>
		/// Fire and Forget a <see cref="Task"/> not caring about whether it succeeds, fails, throws, etc.
		/// </summary>
		/// <param name="task"></param>
		/// <remarks>
		/// Sometimes you just want to start a task and not wait for it to complete, nor want to know if it succeeds or not.
		/// If you just use <see cref="Task.Run"/> and the task fails, you still need to handle the exception to avoid having
		/// <see cref="UnobservedTaskException"/> raised later.
		/// </remarks>
		/// <example>
		/// Task.Run(() => { ... }).Forget();
		/// </example>
		public static void Forget(this Task task)
		{
			// note: this code is inspired by a tweet from Ben Adams. If someone find the link to the tweet I'll be pleased to add it here.
			// Only care about tasks that may fault (not completed) or are faulted,
			// so fast-path for SuccessfullyCompleted and Canceled tasks.
			if (!task.IsCompleted || task.IsFaulted)
			{
				// use "_" (Discard operation) to remove the warning IDE0058: Because this call is not awaited, execution of the current method continues before the call is completed
				// https://docs.microsoft.com/en-us/dotnet/csharp/discards#a-standalone-discard
				_ = ForgetAwaited(task);
			}

			// Allocate the async/await state machine only when needed for performance reason.
			// More info about the state machine: https://blogs.msdn.microsoft.com/seteplia/2017/11/30/dissecting-the-async-methods-in-c/
			async static Task ForgetAwaited(Task task)
			{
				try
				{
					// No need to resume on the original SynchronizationContext, so use ConfigureAwait(false)
					await task.ConfigureAwait(false);
				}
				catch
				{
					// Nothing to do here
				}
			}
		}
	}
}
