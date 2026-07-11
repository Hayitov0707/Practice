using System;
using System.IO;
using System.Reflection;
using CommandLib;

namespace CommandRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");
            var assembly = Assembly.LoadFrom(dllPath);
            
            var sizeType = assembly.GetType("FileSystemCommands.DirectorySizeCommand")!;
            var sizeCmd = (ICommand)Activator.CreateInstance(sizeType, AppDomain.CurrentDomain.BaseDirectory)!;
            sizeCmd.Execute();

            var findType = assembly.GetType("FileSystemCommands.FindFilesCommand")!;
            var findCmd = (ICommand)Activator.CreateInstance(findType, AppDomain.CurrentDomain.BaseDirectory, "*.dll")!;
            findCmd.Execute();
        }
    }
}
