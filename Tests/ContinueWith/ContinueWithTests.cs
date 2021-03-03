using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.ContinueWith
{
    /// <summary>
    /// Test class for continious tasks
    /// </summary>
    public class ContinueWithTests
    {
        /// <summary>
        /// A simple method that does a few calculations with the given number
        /// </summary>
        /// <param name="i">A number.</param>
        /// <param name="callType">The type of the test. May be displayed if <see cref="i"/> is 2. </param>
        /// <returns>The calculation result.</returns>
        public double DoSomeCalculations(int i, string callType = "")
        {
            var b = i + i * 3 - 1.5 * i;
            var c = b + b * 2 - 1.5 * b;
            var d = c + c * 3 - 1.5 * c;
            var e = d + d * 2 - 1.5 * d;

            // Uncomment if you want to check, if all calls are finished before new call type is started
            //if (i == 2) Console.WriteLine(e + callType);

            return e;
        }

        /// <summary>
        /// A simple async method that does a few calculations with the given number
        /// </summary>
        /// <param name="i">A number.</param>
        /// <param name="callType">The type of the test. May be displayed if <see cref="i"/> is 2. </param>
        /// <returns>An awaitable task with the calculation result.</returns>
        public async Task<double> DoSomeCalculationsAsync(int i, string callType = "")
        {
            return await Task.Factory.StartNew(() =>
            {
                var b = i + i * 3 - 1.5 * i;
                var c = b + b * 2 - 1.5 * b;
                var d = c + c * 3 - 1.5 * c;
                var e = d + d * 2 - 1.5 * d;

                // Uncomment if you want to check, if all calls are finished before new call type is started
                //if (i == 2) Console.WriteLine(e + callType);

                return e;
            });
        }

        /// <summary>
        /// Throws an exception.
        /// </summary>
        private void ThrowException()
        {
            throw new Exception("Something faulty happened");
        }

        /// <summary>
        /// Writes a line. Should not be displayed when called after exception.
        /// </summary>
        private void ThisShouldNotHappen()
        {
            Console.WriteLine("This should not happen");
        }

        /// <summary>
        /// Writes a line. Should be displayed, even though when called after exception.
        /// </summary>
        private void ActuallyShouldHappen()
        {
            Console.WriteLine("This should actually happen. OK!");
        }

        /// <summary>
        /// Starts a continious task made with ContinueWith and Run <see cref="times"/> times 
        /// and waits for all results.
        /// </summary>
        /// <param name="times">The number of instances of continious tasks that should be started.</param>
        /// <param name="withError">If <code>true</code>, the continious task throws an exception.</param>
        public void ContinueWith(int times, bool withError = false)
        {
            Task[] tasks = new Task[times];
            for (var i = 0; i < times; i++)
            {
                tasks[i] = ContinueWith(withError, TaskScheduler.Current);
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Starts a continious task made with ContinueWith and Factory.StartNew <see cref="times"/> times 
        /// and waits for all results.
        /// </summary>
        /// <param name="times">The number of instances of continious tasks that should be started.</param>
        /// <param name="withError">If <code>true</code>, the continious task throws an exception.</param>
        public void ContinueWithFactory(int times, bool withError = false)
        {
            Task[] tasks = new Task[times];
            for (var i = 0; i < times; i++)
            {
                tasks[i] = ContinueWithFactory(withError, TaskScheduler.Current);
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Starts a continious task made with ContinueWith inside an Tak Array and Start <see cref="times"/> times 
        /// and waits for all results.
        /// </summary>
        /// <param name="times">The number of instances of continious tasks that should be started.</param>
        /// <param name="withError">If <code>true</code>, the continious task throws an exception.</param>
        public void ContinueWithArray(int times, bool withError = false)
        {
            Task[] tasks = new Task[times];
            for (var i = 0; i < times; i++)
            {
                tasks[i] = ContinueWithArray(withError, TaskScheduler.Current);
            }

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// Starts a continious task made with async/await calls <see cref="times"/> times 
        /// and waits for all results.
        /// </summary>
        /// <param name="times">The number of instances of continious tasks that should be started.</param>
        /// <param name="withError">If <code>true</code>, the continious task throws an exception.</param>
        public async Task AsyncAwait(int times, bool withError = false)
        {
            Task[] tasks = new Task[times];
            for (var i = 0; i < times; i++)
            {
                tasks[i] = AsyncAwait(withError);
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Starts a continious task containing 5 calculations with ContinueWith
        /// </summary>
        /// <param name="withError">Indiciates whether an exception should be thrown in order to test exception handling.</param>
        /// <returns>An async task.</returns>
        private Task ContinueWith(bool withError, TaskScheduler taskScheduler)
        {
            Task task;
            var cancelToken = new CancellationTokenSource();

            if (withError)
            {
                task = Task.Run(() => DoSomeCalculations(6), cancelToken.Token)
                .ContinueWith(_ => DoSomeCalculations(5), cancelToken.Token, TaskContinuationOptions.None, taskScheduler)
                .ContinueWith(_ => DoSomeCalculations(4), cancelToken.Token, TaskContinuationOptions.None, taskScheduler)
                .ContinueWith(_ => ThrowException(), cancelToken.Token, TaskContinuationOptions.None, taskScheduler)
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                            // cancelToken.Cancel();
                            ThrowException();
                    }
                    else
                    {
                        ThisShouldNotHappen();
                    }
                }, cancelToken.Token, TaskContinuationOptions.None, taskScheduler);

                task.ContinueWith(t => ThisShouldNotHappen(), cancelToken.Token, TaskContinuationOptions.OnlyOnRanToCompletion, taskScheduler);
                task.ContinueWith(t => ActuallyShouldHappen(), cancelToken.Token, TaskContinuationOptions.OnlyOnFaulted, taskScheduler);
            }
            else
            {
                task = Task.Run(() => DoSomeCalculations(6), cancelToken.Token)
                    .ContinueWith(_ => DoSomeCalculations(5), cancelToken.Token, TaskContinuationOptions.None, taskScheduler)
                    .ContinueWith(_ => DoSomeCalculations(4), cancelToken.Token, TaskContinuationOptions.None, taskScheduler)
                    .ContinueWith(_ => DoSomeCalculations(3), cancelToken.Token, TaskContinuationOptions.None, taskScheduler)
                    .ContinueWith(_ => DoSomeCalculations(2), cancelToken.Token, TaskContinuationOptions.None, taskScheduler);
            }

            return task;
        }

        /// <summary>
        /// Starts a task array containing 5 calculations with ContinueWith
        /// </summary>
        /// <param name="withError">Indiciates whether an exception should be thrown in order to test exception handling.</param>
        /// <returns>An async task.</returns>
        private Task ContinueWithArray(bool withError, TaskScheduler taskScheduler)
        {
            var cancelationToken = new CancellationToken();

            var task = Task.Factory.StartNew(() => DoSomeCalculations(6), cancelationToken, TaskCreationOptions.None, taskScheduler);
            var task2 = task.ContinueWith(_ => DoSomeCalculations(5), cancelationToken, TaskContinuationOptions.None, taskScheduler);
            var task3 = task2.ContinueWith(_ => DoSomeCalculations(4), cancelationToken, TaskContinuationOptions.None, taskScheduler);
            var task4 = task3.ContinueWith(_ => DoSomeCalculations(3), cancelationToken, TaskContinuationOptions.None, taskScheduler);
            var task5 = task4.ContinueWith(_ => DoSomeCalculations(2, "ContinueWithArray"), cancelationToken, TaskContinuationOptions.None, taskScheduler);

            return Task.Factory.StartNew(() => { Task.WaitAll(new[] { task, task2, task3, task4, task5 }); });
        }

        /// <summary>
        /// Starts a continious task making 5 calculations with a task factory and ContinueWith
        /// </summary>
        /// <param name="withError">Indiciates whether an exception should be thrown in order to test exception handling.</param>
        /// <returns>An async task.</returns>
        private Task ContinueWithFactory(bool withError, TaskScheduler taskScheduler)
        {
            using var cancelToken = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => DoSomeCalculations(6), cancelToken.Token, TaskCreationOptions.None, taskScheduler)
                .ContinueWith(_ => DoSomeCalculations(5), cancelToken.Token, TaskContinuationOptions.None, taskScheduler)
                .ContinueWith(_ => DoSomeCalculations(4), cancelToken.Token, TaskContinuationOptions.None, taskScheduler)
                .ContinueWith(_ => DoSomeCalculations(3), cancelToken.Token, TaskContinuationOptions.None, taskScheduler)
                .ContinueWith(_ => DoSomeCalculations(2, "ContinueWithFactory"), taskScheduler);

            return task;
        }

        /// <summary>
        /// Starts an async task making 5 calculations asynchronously with async/await
        /// </summary>
        /// <param name="withError">Indiciates whether an exception should be thrown in order to test exception handling.</param>
        /// <returns>An async task.</returns>
        private async Task AsyncAwait(bool withError)
        {
            try
            {
                await DoSomeCalculationsAsync(6);
                await DoSomeCalculationsAsync(5);
                await DoSomeCalculationsAsync(4);

                if (withError)
                {
                    await Task.Factory.StartNew(() => ThrowException());
                    await Task.Factory.StartNew(() => ThisShouldNotHappen());
                }
                else
                {
                    await DoSomeCalculationsAsync(3);
                    await DoSomeCalculationsAsync(2, "AsyncAwait");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
