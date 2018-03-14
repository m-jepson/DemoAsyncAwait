using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwait
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO DEMO 1
            //var task = FibonacciAsync(100);
            //Console.WriteLine("Task started");
            //task.Wait();
            //Console.WriteLine("Task completed.");
            //var result = task.Result;
            //Console.WriteLine($"final result: {result}");
            //Console.ReadKey();

            // TODO DEMO 2
            //var result1 = FibonacciAsync(100).Result;
            //var result2 = FibonacciAsync(250).Result;
            //var result3 = FibonacciAsync(500).Result;
            //Console.WriteLine($"FibonacciAsync(100): {result1}");
            //Console.WriteLine($"FibonacciAsync(250): {result2}");
            //Console.WriteLine($"FibonacciAsync(500): {result3}");
            //Console.ReadKey();

            // TODO DEMO 3
            //var task1 = FibonacciAsync(100);
            //var task2 = FibonacciAsync(250);
            //var task3 = FibonacciAsync(500);
            //Task.WaitAll(task1, task2, task3);
            //Console.WriteLine($"FibonacciAsync(100): {task1.Result}");
            //Console.WriteLine($"FibonacciAsync(250): {task2.Result}");
            //Console.WriteLine($"FibonacciAsync(500): {task3.Result}");
            //Console.ReadKey();

            // TODO DEMO 4
            //var targets = new[] { 100, 250, 500 };
            //var results = MultipleFibonacciAsync1(targets).Result.ToList();
            //Console.WriteLine($"FibonacciAsync(100): {results[0]}");
            //Console.WriteLine($"FibonacciAsync(250): {results[1]}");
            //Console.WriteLine($"FibonacciAsync(500): {results[2]}");
            //Console.ReadKey();

            // TODO DEMO 5
            //var targets = new[] { 100, 250, 500 };
            //var results = MultipleFibonacciAsync2(targets).Result.ToList();
            //Console.WriteLine($"FibonacciAsync(100): {results[0]}");
            //Console.WriteLine($"FibonacciAsync(250): {results[1]}");
            //Console.WriteLine($"FibonacciAsync(500): {results[2]}");
            //Console.ReadKey();
            
            // TODO DEMO 6
            var targets = new[] { 100, 250, 500 };
            var cts = new CancellationTokenSource(); // <-- this is used to cancel a task.
            var task = MultipleFibonacciAsync1(targets, cts.Token); // <-- Pass the token to enable cancellation.
            Thread.Sleep(2500);
            cts.Cancel(); // <-- This cancels a running task.
            var results = task.Result.ToList();
            Console.WriteLine($"FibonacciAsync(100): {results[0]}");
            if (results.Count == 3)
            {
                Console.WriteLine($"FibonacciAsync(250): {results[1]}");
                Console.WriteLine($"FibonacciAsync(500): {results[2]}");
            }

            Console.ReadKey();

            File.WriteAllTextAsync("C:\\Temp\\test.txt", "Some data to go into the file");
        }

        static async Task<int> FibonacciAsync(int target, CancellationToken cancellationToken = default(CancellationToken))
        {
            var i = 1;
            var j = 1;
            while (i < target && !cancellationToken.IsCancellationRequested)
            //while(i < target)
            {
                //cancellationToken.ThrowIfCancellationRequested();
                var tmp = i;
                i += j;
                j = tmp;
                Console.WriteLine(i);
                await Task.Delay(1000).ConfigureAwait(false);
            }

            return i;
        }

        static async Task<IEnumerable<int>> MultipleFibonacciAsync1(IEnumerable<int> targets, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<int>();

            foreach (var target in targets)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                result.Add(await FibonacciAsync(target, cancellationToken).ConfigureAwait(false));
            }

            return result;
        }


        static async Task<IEnumerable<int>> MultipleFibonacciAsync2(IEnumerable<int> targets, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = new List<Task<int>>();

            foreach (var target in targets)
                result.Add(FibonacciAsync(target, cancellationToken));

            await Task.WhenAll(result.ToArray()).ConfigureAwait(false);

            return result.Select(r => r.Result);
        }
    }
}
