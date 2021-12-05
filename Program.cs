using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace GoodRom
{
    class Program
    {
        const string JAP = "(J)";
        const string EUR = "(E)";
        const string USA = "(U)";

        const string OutputDirectory = @"C:\Torrents\Test";

        static void Main(string[] args)
        {
            string path = @"C:\Torrents\GoodSNESn\GoodSNES";

            var files = Directory.EnumerateFiles(path);

            var filtered = files
                .Where(f => f.Contains("[!]"))
                .GroupBy(f => f.Contains("(") ? f.Substring(0, f.IndexOf("(") - 1) : f, f => f, (key, g) => new { PureName = key, FileNames = g.ToList() });

            var best = filtered.SelectMany(f => {
                if(f.FileNames.Where(f => f.Contains(USA)).Any()){
                    return f.FileNames.Where(f => f.Contains(USA));
                }
                else if (f.FileNames.Where(f => f.Contains(JAP)).Any())
                {
                    return f.FileNames.Where(f => f.Contains(JAP));
                }
                else if (f.FileNames.Where(f => f.Contains(EUR)).Any())
                {
                    return f.FileNames.Where(f => f.Contains(EUR));
                }
                return f.FileNames;
            });

            //best.ToList().ForEach(f => Console.WriteLine(f));

            var usa = best.Where(f => f.Contains(USA));
            var jap = best.Where(f => f.Contains(JAP));
            var euro = best.Where(f => f.Contains(EUR));

            Console.WriteLine("Total before filtering: " + files.Count());
            Console.WriteLine("Total Jap: " + jap.Count());
            Console.WriteLine("Total Euro: " + euro.Count());
            Console.WriteLine("Total US: " + usa.Count());
            Console.WriteLine("Total after filtering::" + best.Count());

            var dirNames = CreateDirs();
            CopyFiles(best, dirNames);

            Console.ReadLine();
        }

        static IEnumerable<string> CreateDirs()
        {
            var dirNames = Enumerable.Range('A', 26).Select(x => (char)x).ToArray().Select(d => d.ToString()).Append("0-9");

            foreach(var dir in dirNames)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(Path.Combine(OutputDirectory, dir));
                }
            }
            return dirNames;
        }

        static void CopyFiles(IEnumerable<string> files, IEnumerable<string> dirNames)
        {
            foreach(var dir in dirNames)
            {
                var filesForDir = files.Where(f => new FileInfo(f).Name.StartsWith(dir));

                if (dir == "0-9") 
                {
                    filesForDir = files.Where(f => Regex.IsMatch(new FileInfo(f).Name[0].ToString(), "[0-9]"));
                }

                foreach (var file in filesForDir)
                {
                    Console.WriteLine(file + " copy to " + Path.Combine(OutputDirectory, dir, new FileInfo(file).Name));
                    File.Copy(file, Path.Combine(OutputDirectory, dir, new FileInfo(file).Name));
                }
            }

        }
    }
}
