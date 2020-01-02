using LibScrapeTP.Entities;
using System;
using System.Threading.Tasks;
using System.Linq;
using LibScrapeTP.ETLSteps.Extract;
using System.Collections.Generic;
using System.IO;
using CommandLine;

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
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ParseOptions>(args).WithParsed(o =>
            {
                var path = o.StoragePath;
                var timeout = TimeSpan.FromSeconds(o.Timeout);
                
                Parallel.ForEach(Enum.GetValues(typeof(University)).Cast<University>().Take(1), university =>
                {
                    var p = Path.Combine(path, university.ToString());
                    if (!Directory.Exists(p))
                    {
                        Directory.CreateDirectory(p);
                    }

                    var pages = new HashSet<string>(new PageUrlExtracter(university, timeout).GetTutorPagesUrls());
                    new TutorPageFetcher(p).GetPages(new HashSet<string>(pages).ToList(), timeout);
                });
            });
        }
    }
}
