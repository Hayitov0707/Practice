using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11
{
    public interface ICalculator
    {
        int Add(int a, int b);
        int Minus(int a, int b);
        int Mul(int a, int b);
        int Div(int a, int b);
    }

    public static class CalculatorFactory
    {
        public static ICalculator CreateCalculator(string sourceCode)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var coreDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(coreDir, "System.Runtime.dll"))
            };

            var compilation = CSharpCompilation.Create(
                "DynamicCalculator_" + Guid.NewGuid().ToString("N"),
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                throw new InvalidOperationException("Compilation failed");
            }

            ms.Seek(0, SeekOrigin.Begin);
            var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
            var type = assembly.GetType("Calculator")!;
            return (ICalculator)Activator.CreateInstance(type)!;
        }
    }
}
