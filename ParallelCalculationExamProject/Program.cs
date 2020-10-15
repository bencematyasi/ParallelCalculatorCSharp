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
            SequentialRun(calculator, sw);
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Parallel execution is starting...");
            ParallelRun(calculator, sw);


        }

        public static void SequentialRun(Calculator calculator, Stopwatch sw)
        {
            sw.Reset();
            sw.Start();
            long result = calculator.calculateNumberOfDivisible(1, MaxNumber, Divisor);
            sw.Stop();
            Console.WriteLine("Result: " + result + " calculated under {0:F5} sec.", (sw.ElapsedMilliseconds / 1000f));
        }

        public static  void ParallelRun(Calculator calculator, Stopwatch sw)
        {
            LimitedConcurrencyLevelTaskScheduler taskScheduler = new LimitedConcurrencyLevelTaskScheduler(Environment.ProcessorCount);
            sw.Reset();
            sw.Start();
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(5000);
            long taskResult = 0;

            List<Task<long>> taskList = new List<Task<long>>();
            for (int i = 1; i < Environment.ProcessorCount + 1; i++)
            {
                var fraction = MaxNumber / (Environment.ProcessorCount);
                var first = ((fraction * i) - fraction) + 1;
                var last = fraction * i;

                /*Creating as many Tasks as thread is counted in the machine,
                and setting each task a CancellationToken, that will cancel the task after 5 seconds of runtime*/
                Task<long> task = new Task<long>(() => 
                    calculator.calculateNumberOfDivisible(first, last, Divisor), cts.Token);
                //Add the created task to taskList
                taskList.Add(task);
            }

            /*Start each task.
            The order of execution does not matter*/
            Parallel.ForEach(taskList.ToArray(), task =>
            {
                task.Start(taskScheduler);
            });

            //Wait all the tasks to be completed
            Task.WaitAll(taskList.ToArray());

            /*Add the tasks' return value/result to the taskResult variable.
            The order of addition does not matter, but to receive all the results the Interlocked is required to have*/
            Parallel.ForEach(taskList.ToArray(), task =>
            {
                Interlocked.Add(ref taskResult, task.Result);
            });
            cts.Dispose();
            sw.Stop();

            Console.WriteLine("Result: " + taskResult + " calculated under {0:F5} sec.", (sw.ElapsedMilliseconds / 1000f));

        }
    }
}
