using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using ScottPlot;

namespace task15
{
    public static class IntegralSolver
    {
        public static double SolveSingleThread(double a, double b, Func<double, double> function, double step)
        {
            double sum = 0.0;
            double current = a;
            while (current < b)
            {
                double next = current + step;
                if (next > b) next = b;
                sum += (function(current) + function(next)) * (next - current) / 2.0;
                current += step;
            }
            return sum;
        }

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

        public static double SolveMultiThread(double a, double b, Func<double, double> function, double step, int threadsNumber)
        {
            double totalResult = 0.0;
            var barrier = new Barrier(threadsNumber + 1);
            double totalWidth = b - a;
            double threadWidth = totalWidth / threadsNumber;

            for (int i = 0; i < threadsNumber; i++)
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

    class Program
    {
        static void Main(string[] args)
        {
            double a = -100.0;
            double b = 100.0;
            Func<double, double> targetFunction = Math.Sin;
            double targetPrecision = 1e-4;

            double[] steps = { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5, 1e-6 };
            double chosenStep = 1e-1;

            foreach (double step in steps)
            {
                double value = IntegralSolver.SolveSingleThread(a, b, targetFunction, step);
                if (Math.Abs(value) <= targetPrecision)
                {
                    chosenStep = step;
                    break;
                }
            }

            int runs = 5;
            List<double> singleThreadTimes = new List<double>();
            for (int i = 0; i < runs; i++)
            {
                var sw = Stopwatch.StartNew();
                IntegralSolver.SolveSingleThread(a, b, targetFunction, chosenStep);
                sw.Stop();
                singleThreadTimes.Add(sw.Elapsed.TotalMilliseconds);
            }
            double avgSingleThreadTime = singleThreadTimes.Average();

            int[] threadCounts = { 1, 2, 4, 6, 8, 12, 16 };
            Dictionary<int, double> threadPerformance = new Dictionary<int, double>();

            foreach (int threads in threadCounts)
            {
                List<double> multiThreadTimes = new List<double>();
                for (int i = 0; i < runs; i++)
                {
                    var sw = Stopwatch.StartNew();
                    IntegralSolver.SolveMultiThread(a, b, targetFunction, chosenStep, threads);
                    sw.Stop();
                    multiThreadTimes.Add(sw.Elapsed.TotalMilliseconds);
                }
                threadPerformance[threads] = multiThreadTimes.Average();
            }

            var optimalPair = threadPerformance.OrderBy(p => p.Value).First();
            int optimalThreads = optimalPair.Key;
            double avgOptimalMultiThreadTime = optimalPair.Value;

            double percentageDifference = ((avgSingleThreadTime - avgOptimalMultiThreadTime) / avgSingleThreadTime) * 100.0;

            string reportPath = "performance_report.txt";
            using (StreamWriter writer = new StreamWriter(reportPath))
            {
                writer.WriteLine($"Chosen Step Size: {chosenStep}");
                writer.WriteLine($"Average Single-Threaded Execution Time: {avgSingleThreadTime:F2} ms");
                writer.WriteLine($"Optimal Number of Threads: {optimalThreads}");
                writer.WriteLine($"Average Optimal Multi-Threaded Execution Time: {avgOptimalMultiThreadTime:F2} ms");
                writer.WriteLine($"Performance Difference: {percentageDifference:F2}%");
            }

            var plt = new Plot();
            double[] xs = threadPerformance.Values.ToArray();
            double[] ys = threadPerformance.Keys.Select(k => (double)k).ToArray();

            var scatter = plt.Add.Scatter(xs, ys);
            plt.XLabel("Execution Time (ms)");
            plt.YLabel("Thread Count");
            plt.Title("Thread Count vs Execution Time");
            
            plt.SavePng("threads_vs_time.png", 800, 600);
        }
    }
}
