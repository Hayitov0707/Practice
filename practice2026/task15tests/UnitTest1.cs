using Xunit;
using System;
using task15;

namespace task15tests
{
    public class IntegralPerformanceTests
    {
        [Fact]
        public void BothSolvers_ShouldReturnAccurateResults()
        {
            double a = -100.0;
            double b = 100.0;
            Func<double, double> function = Math.Sin;
            double step = 1e-3;

            double singleResult = IntegralSolver.SolveSingleThread(a, b, function, step);
            double multiResult = IntegralSolver.SolveMultiThread(a, b, function, step, 4);

            Assert.True(Math.Abs(singleResult) <= 1e-4);
            Assert.True(Math.Abs(multiResult) <= 1e-4);
        }
    }
}
