using System;
using System.Threading;

namespace ParallelCalculationExamProject
{
    public class Calculator
    {
        public long calculateNumberOfDivisible(long first, long last, long divisor)
        {
            long amountOfNumber = 0;
            for (long i = first; i <= last; i++)
            {
                if (i % divisor == 0)
                {
                    amountOfNumber++;
                }
            }
            return amountOfNumber;
        }
    }
}
