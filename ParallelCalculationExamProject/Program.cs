using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelCalculationExamProject
{
    class Program 
    {
        private static readonly long MaxNumber = 1_000_000_000L;
        private static readonly long Divisor = 3;
        static void Main(string[] args)
        {
            Calculator calculator = new Calculator();
            Stopwatch sw = new Stopwatch();

            Console.WriteLine("Sequential execution starting...");
            sw.Start();
            long result = calculator.calculateNumberOfDivisible(0, MaxNumber, Divisor);
            sw.Stop();
            Console.WriteLine("Result          : " + result + " calculated under " + sw.ElapsedMilliseconds + " milliseconds");

        }
    }
}
