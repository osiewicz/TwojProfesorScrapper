using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using LibScrapeTP;

namespace TransformDriver
{
    internal class Program
    {
        internal class ParseOptions
        {
            [Option('i', "input", Required = true)]
            public string InputPath { get; set; }
            [Option('o', "output", Required = true)]
            public string OutputPath { get; set; }
        }

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ParseOptions>(args).WithParsed(o =>
            {
                Parallel.ForEach(Directory.EnumerateDirectories(o.InputPath), universityPath =>
                {
                    var tutorPagesAsFilePaths = Directory.EnumerateFiles(universityPath);
                    var tutorPagesParsed = new List<LibScrapeTP.Entities.TutorSummaryPage>();
                    foreach(var tutorPage in tutorPagesAsFilePaths)
                    {
                        using var pageAsStream = File.OpenRead(tutorPage);
                        var parsed = LibScrapeTP.ETLSteps.Transform.TutorPageParser.Parse(pageAsStream);
                        tutorPagesParsed.Add(parsed);
                    }
                });
            });
        }
    }
}
