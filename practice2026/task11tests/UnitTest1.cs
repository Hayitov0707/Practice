using Xunit;
using task11;

namespace task11tests
{
    public class CalculatorTests
    {
        [Fact]
        public void Calculator_ShouldExecuteMethodsWithoutReflection()
        {
            string sourceCode = @"
                using task11;
                public class Calculator : ICalculator
                {
                    public int Add(int a, int b) => a + b;
                    public int Minus(int a, int b) => a - b;
                    public int Mul(int a, int b) => a * b;
                    public int Div(int a, int b) => a / b;
                }";

            ICalculator calc = CalculatorFactory.CreateCalculator(sourceCode);

            Assert.Equal(15, calc.Add(10, 5));
            Assert.Equal(5, calc.Minus(10, 5));
            Assert.Equal(50, calc.Mul(10, 5));
            Assert.Equal(2, calc.Div(10, 5));
        }
    }
}
