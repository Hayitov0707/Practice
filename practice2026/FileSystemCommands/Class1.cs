using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CommandLib;

namespace FileSystemCommands
{
    public class DirectorySizeCommand : ICommand
    {
        private readonly string _dirPath;
        public long CalculatedSize { get; private set; }

        public DirectorySizeCommand(string dirPath)
        {
            _dirPath = dirPath;
        }

        public void Execute()
        {
            if (!Directory.Exists(_dirPath)) return;
            CalculatedSize = Directory.GetFiles(_dirPath, "*", SearchOption.AllDirectories)
                                     .Sum(t => new FileInfo(t).Length);
            Console.WriteLine($"Directory size: {CalculatedSize} bytes");
        }
    }

    public class FindFilesCommand : ICommand
    {
        private readonly string _dirPath;
        private readonly string _searchPattern;
        public List<string> FoundFiles { get; private set; } = new List<string>();

        public FindFilesCommand(string dirPath, string searchPattern)
        {
            _dirPath = dirPath;
            _searchPattern = searchPattern;
        }

        public void Execute()
        {
            if (!Directory.Exists(_dirPath)) return;
            var files = Directory.GetFiles(_dirPath, _searchPattern);
            FoundFiles.Clear();
            foreach (var file in files)
            {
                FoundFiles.Add(file);
                Console.WriteLine($"Found: {Path.GetFileName(file)}");
            }
        }
    }
}
