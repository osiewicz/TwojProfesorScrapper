using LibScrapeTP.Entities;
using System;
using System.Threading.Tasks;
using System.Linq;
using LibScrapeTP.ETLSteps.Extract;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using System.Threading;

namespace ExtractDriver
{
    class Program
    {
        private const uint DefaultFetchTimeout = 5; // in seconds
        private const string DefaultRelativeStoragePath = "./Extract";
        internal class ParseOptions
        {
            [Option('t', "timeout", Default = DefaultFetchTimeout)]
            public uint Timeout { get; set; }
            [Option('p', "path", Default = DefaultRelativeStoragePath)]
            public string StoragePath { get; set; }
        }

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ParseOptions>(args).WithParsed(o =>
            {
                var path = o.StoragePath;
                var timeout = TimeSpan.FromSeconds(o.Timeout);
                var totalCountOfFiles = 0;
                Parallel.ForEach(Enum.GetValues(typeof(University)).Cast<University>(), university =>
                {
                    var p = Path.Combine(path, university.ToString());
                    if (!Directory.Exists(p))
                    {
                        Directory.CreateDirectory(p);
                    }

                    var pages = new HashSet<string>(new PageUrlExtracter(university, timeout).GetTutorPagesUrls());
                    new TutorPageFetcher(p).GetPages(new HashSet<string>(pages).ToList(), timeout);
                    Interlocked.Add(ref totalCountOfFiles, pages.Count);
                });

                Console.WriteLine($"Fetched {totalCountOfFiles} .html files to {o.StoragePath}");
            });
        }
    }
}
