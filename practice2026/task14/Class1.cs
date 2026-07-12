using System;
using System.Threading;

namespace task14
{
    public static class DefiniteIntegral
    {
        private static void SafeAdd(ref double location, double value)
        {
            double newCurrentValue = location;
            while (true)
            {
                double currentValue = newCurrentValue;
                double newValue = currentValue + value;
                newCurrentValue = Interlocked.CompareExchange(ref location, newValue, currentValue);
                if (newCurrentValue == currentValue)
                    break;
            }
        }

        public static double Solve(double a, double b, Func<double, double> function, double step, int threadsnumber)
        {
            double totalResult = 0.0;
            var barrier = new Barrier(threadsnumber + 1);
            double totalWidth = b - a;
            double threadWidth = totalWidth / threadsnumber;

            for (int i = 0; i < threadsnumber; i++)
            {
                int index = i;
                var thread = new Thread(() =>
                {
                    double localA = a + index * threadWidth;
                    double localB = a + (index + 1) * threadWidth;
                    double localSum = 0.0;
                    double current = localA;

                    while (current < localB)
                    {
                        double next = current + step;
                        if (next > localB) next = localB;

                        localSum += (function(current) + function(next)) * (next - current) / 2.0;
                        current += step;
                    }

                    SafeAdd(ref totalResult, localSum);
                    barrier.SignalAndWait();
                });
                thread.Start();
            }

            barrier.SignalAndWait();
            return totalResult;
        }
    }
}
