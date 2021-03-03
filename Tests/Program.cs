using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // test variables
                Console.WriteLine("Setting test variables");

                Stopwatch sw = new Stopwatch();
                var testClass = new ContiniousTasksTests();
                var elapsedTimes = new Dictionary<string, double> { { "AsyncAwait", 0 }, { "ContinueWith", 0 }, { "ContinueWithArray", 0 }, { "ContinueWithFactory", 0 } };

                // number of continious tasks to start
                var startedContiniousTasksCount = 1;

                // number of times the continious tasks should be started
                var iterations = 1;

                // indicates whether information should be printed to console
                var showMessages = false;

                Console.WriteLine($"|------ Count of continious tasks: {startedContiniousTasksCount} ------|");
                Console.WriteLine($"|------ Count of iterations: {iterations} ------|");
                Console.WriteLine($"|------ Show all test messages: {showMessages} ------|");

                // in order to get comparable results, I start one calculation beforehead to start up the application
                var task = Task.Run(async () => await testClass.AsyncAwait(1));
                task.Wait();

                Console.WriteLine("------ Start tests ------");

                for (var i = 0; i < iterations; i++)
                {
                    // test start
                    Console.WriteLine($"------ Start iteration {i+1} ------");

                    // async tasks with await
                    if (showMessages) Console.WriteLine("Start AsyncAwait");
                    sw.Start();
                    try
                    {
                        task = Task.Run(async () => await testClass.AsyncAwait(startedContiniousTasksCount));
                        task.Wait();
                    }
                    catch (Exception ex)
                    {
                        if (showMessages) Console.WriteLine($"AsyncAwait Exception: {ex.Message}");
                    }

                    sw.Stop();
                    if (showMessages) Console.WriteLine("AsyncAwait done.");
                    if (showMessages) Console.WriteLine($"AsyncAwait Elapsed={sw.Elapsed.TotalMilliseconds}");
                    if (i != 0 || iterations == 1) elapsedTimes["AsyncAwait"] = +sw.Elapsed.TotalMilliseconds;

                    sw.Reset();

                    // continouation task with Task.Run
                    if (showMessages) Console.WriteLine("Start ContinueWith ");
                    try
                    {
                        sw.Start();
                        testClass.ContinueWith(startedContiniousTasksCount, true);
                    }
                    catch (Exception ex)
                    {
                        if (showMessages) Console.WriteLine($"ContinueWith Exception: {ex.Message}");
                    }

                    sw.Stop();
                    if (showMessages) Console.WriteLine("ContinueWith done.");
                    if (showMessages) Console.WriteLine($"ContinueWith Elapsed={sw.Elapsed.TotalMilliseconds}");
                    if (i != 0 || iterations == 1) elapsedTimes["ContinueWith"] = +sw.Elapsed.TotalMilliseconds;

                    sw.Reset();

                    // continuation task with Task.Run in task array
                    if (showMessages) Console.WriteLine("Start ContinueWithArray");
                    try
                    {
                        sw.Start();
                        testClass.ContinueWithArray(startedContiniousTasksCount);
                    }
                    catch (Exception ex)
                    {
                        if (showMessages) Console.WriteLine($"ContinueWithArray Exception: {ex.Message}");
                    }

                    sw.Stop();
                    if (showMessages) Console.WriteLine("ContinueWithArray done.");
                    if (showMessages) Console.WriteLine($"ContinueWithArray Elapsed={sw.Elapsed.TotalMilliseconds}");
                    if (i != 0 || iterations == 1) elapsedTimes["ContinueWithArray"] = +sw.Elapsed.TotalMilliseconds;

                    sw.Reset();

                    // continuation task with Task.Factory.StartNew
                    if (showMessages) Console.WriteLine("Start ContinueWithFactory");
                    try
                    {
                        sw.Start();
                        testClass.ContinueWithFactory(startedContiniousTasksCount);
                    }
                    catch (Exception ex)
                    {
                        if (showMessages) Console.WriteLine($"ContinueWithFactory Exception: {ex.Message}");
                    }

                    sw.Stop();
                    if (showMessages) Console.WriteLine("ContinueWithFactory done.");
                    if (showMessages) Console.WriteLine($"ContinueWithFactory Elapsed={sw.Elapsed.TotalMilliseconds}");
                    if (i != 0 || iterations == 1) elapsedTimes["ContinueWithFactory"] = +sw.Elapsed.TotalMilliseconds;

                    sw.Reset();

                    Console.WriteLine($"------ Iteration {i+1} done ------");
                }

                // tests are done
                Console.WriteLine("------ Tests done ------");

                // the first iteration will not be counted, since it usually is an aberration
                Console.WriteLine($"Mean time AsyncAwait: {elapsedTimes["AsyncAwait"] / (iterations > 1 ? iterations-1 : 1)}");
                Console.WriteLine($"Mean time ContinueWith: {elapsedTimes["ContinueWith"] / (iterations > 1 ? iterations - 1 : 1)}");
                Console.WriteLine($"Mean time ContinueWithArrayy: {elapsedTimes["ContinueWithArray"] / (iterations > 1 ? iterations - 1 : 1)}");
                Console.WriteLine($"Mean time ContinueWithFactory: {elapsedTimes["ContinueWithFactory"] / (iterations > 1 ? iterations - 1 : 1)}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Test class for continious tasks
        /// </summary>
        public class ContiniousTasksTests
        {
            /// <summary>
            /// A simple method that does a few calculations with the given number
            /// </summary>
            /// <param name="i">A number.</param>
            /// <param name="callType">The type of the test. May be displayed if <see cref="i"/> is 2. </param>
            /// <returns></returns>
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
                    await Task.Factory.StartNew(() => DoSomeCalculations(6));
                    await Task.Factory.StartNew(() => DoSomeCalculations(5));
                    await Task.Factory.StartNew(() => DoSomeCalculations(4));

                    if (withError)
                    {
                        await Task.Factory.StartNew(() => ThrowException());
                        await Task.Factory.StartNew(() => ThisShouldNotHappen());
                    } 
                    else
                    {
                        await Task.Factory.StartNew(() => DoSomeCalculations(3));
                        await Task.Factory.StartNew(() => DoSomeCalculations(2, "AsyncAwait"));
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }
    }
}