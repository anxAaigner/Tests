using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tests.ContinueWith
{
    /// <summary>
    /// Starter for ContinueWith tests
    /// </summary>
    class ContinueWithTestStarter
    {
        private readonly Stopwatch _sw;
        private readonly ContinueWithTests _testClass;
        private readonly Dictionary<string, double> _elapsedTimes;
        private readonly int _startedContiniousTasksCount;
        private readonly int _iterations;
        private readonly bool _showMessages;

        /// <summary>
        /// Starter for ContinueWith tests
        /// </summary>
        /// <param name="startedContiniiousTaskCount">number of continious tasks to start</param>
        /// <param name="iterations">number of times the continious tasks should be started</param>
        /// <param name="showMessages">indicates whether information should be printed to console</param>
        public ContinueWithTestStarter(int startedContiniiousTaskCount, int iterations, bool showMessages = false) 
        {
            _sw = new Stopwatch();
            _testClass = new ContinueWithTests();
            _elapsedTimes = new Dictionary<string, double> { { "AsyncAwait", 0 }, { "ContinueWith", 0 }, { "ContinueWithArray", 0 }, { "ContinueWithFactory", 0 } };

            _startedContiniousTasksCount = startedContiniiousTaskCount;
            _iterations = iterations;
            _showMessages = showMessages;
        }

        /// <summary>
        /// Starts the tests
        /// </summary>
        public void Start()
        {
            Console.WriteLine($"|------ Count of continious tasks: {_startedContiniousTasksCount} ------|");
            Console.WriteLine($"|------ Count of iterations: {_iterations} ------|");
            Console.WriteLine($"|------ Show all test messages: {_showMessages} ------|");

            // in order to get comparable results, I start one calculation beforehead to start up the application
            var task = Task.Run(async () => await _testClass.AsyncAwait(1));
            task.Wait();

            Console.WriteLine("------ Start tests ------");

            for (var i = 0; i < _iterations; i++)
            {
                // test start
                Console.WriteLine($"------ Start iteration {i + 1}/{_iterations} ------");

                // async tasks with await
                if (_showMessages)
                    Console.WriteLine("Start AsyncAwait");
                _sw.Start();
                try
                {
                    task = Task.Run(async () => await _testClass.AsyncAwait(_startedContiniousTasksCount));
                    task.Wait();
                }
                catch (Exception ex)
                {
                    if (_showMessages)
                        Console.WriteLine($"AsyncAwait Exception: {ex.Message}");
                }

                _sw.Stop();
                if (_showMessages)
                    Console.WriteLine("AsyncAwait done.");
                if (_showMessages)
                    Console.WriteLine($"AsyncAwait Elapsed={_sw.Elapsed.TotalMilliseconds}");
                if (i != 0 || _iterations == 1)
                    _elapsedTimes["AsyncAwait"] = +_sw.Elapsed.TotalMilliseconds;

                _sw.Reset();

                // continouation task with Task.Run
                if (_showMessages)
                    Console.WriteLine("Start ContinueWith ");
                try
                {
                    _sw.Start();
                    _testClass.ContinueWith(_startedContiniousTasksCount);
                }
                catch (Exception ex)
                {
                    if (_showMessages)
                        Console.WriteLine($"ContinueWith Exception: {ex.Message}");
                }

                _sw.Stop();
                if (_showMessages)
                    Console.WriteLine("ContinueWith done.");
                if (_showMessages)
                    Console.WriteLine($"ContinueWith Elapsed={_sw.Elapsed.TotalMilliseconds}");
                if (i != 0 || _iterations == 1)
                    _elapsedTimes["ContinueWith"] = +_sw.Elapsed.TotalMilliseconds;

                _sw.Reset();

                // continuation task with Task.Run in task array
                if (_showMessages)
                    Console.WriteLine("Start ContinueWithArray");
                try
                {
                    _sw.Start();
                    _testClass.ContinueWithArray(_startedContiniousTasksCount);
                }
                catch (Exception ex)
                {
                    if (_showMessages)
                        Console.WriteLine($"ContinueWithArray Exception: {ex.Message}");
                }

                _sw.Stop();
                if (_showMessages)
                    Console.WriteLine("ContinueWithArray done.");
                if (_showMessages)
                    Console.WriteLine($"ContinueWithArray Elapsed={_sw.Elapsed.TotalMilliseconds}");
                if (i != 0 || _iterations == 1)
                    _elapsedTimes["ContinueWithArray"] = +_sw.Elapsed.TotalMilliseconds;

                _sw.Reset();

                // continuation task with Task.Factory.StartNew
                if (_showMessages)
                    Console.WriteLine("Start ContinueWithFactory");
                try
                {
                    _sw.Start();
                    _testClass.ContinueWithFactory(_startedContiniousTasksCount);
                }
                catch (Exception ex)
                {
                    if (_showMessages)
                        Console.WriteLine($"ContinueWithFactory Exception: {ex.Message}");
                }

                _sw.Stop();
                if (_showMessages)
                    Console.WriteLine("ContinueWithFactory done.");
                if (_showMessages)
                    Console.WriteLine($"ContinueWithFactory Elapsed={_sw.Elapsed.TotalMilliseconds}");
                if (i != 0 || _iterations == 1)
                    _elapsedTimes["ContinueWithFactory"] = +_sw.Elapsed.TotalMilliseconds;

                _sw.Reset();

                Console.WriteLine($"------ Iteration {i + 1} done ------");
            }

            // tests are done
            Console.WriteLine("------ Tests done ------");

            // the first iteration will not be counted, since it usually is an aberration
            Console.WriteLine($"Mean time AsyncAwait: {_elapsedTimes["AsyncAwait"] / (_iterations > 1 ? _iterations - 1 : 1)}");
            Console.WriteLine($"Mean time ContinueWith: {_elapsedTimes["ContinueWith"] / (_iterations > 1 ? _iterations - 1 : 1)}");
            Console.WriteLine($"Mean time ContinueWithArray: {_elapsedTimes["ContinueWithArray"] / (_iterations > 1 ? _iterations - 1 : 1)}");
            Console.WriteLine($"Mean time ContinueWithFactory: {_elapsedTimes["ContinueWithFactory"] / (_iterations > 1 ? _iterations - 1 : 1)}");
        }
    }
}
