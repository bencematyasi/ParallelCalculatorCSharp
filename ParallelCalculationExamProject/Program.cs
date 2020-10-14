using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;

namespace ParallelCalculationExamProject
{
    class Program
    {
        private static readonly long MaxNumber = 5_000_000_000L;
        private static readonly long Divisor = 3;
        static void Main(string[] args)
        {
            Calculator calculator = new Calculator();
            Stopwatch sw = new Stopwatch();
            Console.WriteLine("Sequential execution is starting...");
            sw.Start();
            long result = calculator.calculateNumberOfDivisible(0, MaxNumber, Divisor);
            sw.Stop();
            Console.WriteLine("Result          : " + result + " calculated under " + sw.ElapsedMilliseconds + " milliseconds");

            List<Task<long>> tasklist = new List<Task<long>>();

            CustomTaskScheduler taskScheduler = new CustomTaskScheduler(Environment.ProcessorCount);
            long taskResult = 0;
            Console.WriteLine("Parallel execution is starting...");

            CancellationTokenSource cts = new CancellationTokenSource();

            TaskFactory tf = new TaskFactory(taskScheduler);
            sw.Restart();
            for (int i = 1; i < Environment.ProcessorCount + 1; i++)
            {
                var last = MaxNumber / (Environment.ProcessorCount);
                var first = ((last * i) - last) + 1;
                if (i == 1)
                {
                    first = 0;
                }
                last = last * i;
                Task<long> task = new Task<long>(() => calculator.calculateNumberOfDivisible(first, last, Divisor), cts.Token);
                tasklist.Add(task);
            }
            //TODO get task result and add them together.
            foreach (var task in tasklist)
            {
                task.Start(taskScheduler);
            }
            Task.WaitAll(tasklist.ToArray());
            foreach (var t in tasklist)
            {
                taskResult += t.Result;
            }
            cts.Dispose();
            sw.Stop();

            Console.WriteLine("Result          : " + taskResult + " calculated under " + sw.ElapsedMilliseconds + " milliseconds");
        }
    }
}
