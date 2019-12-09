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
        private static readonly TimeSpan DefaultFetchTimeout = TimeSpan.FromSeconds(5);
        private static readonly string DefaultRelativeStoragePath = $"./TpExtract{DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss")}";
        //private static readonly


        public class ParseOptions
        {
            [Option('t', "timeout", Required = true)]
            public uint Timeout { get; set; }
            [Option('p', "path", Required = true)]
            public string StoragePath { get; set; }
        }
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ParseOptions>(args).WithParsed<ParseOptions>( o =>
            {
                var path = o.StoragePath;
                var timeout = TimeSpan.FromSeconds(o.Timeout);
                
                Parallel.ForEach(Enum.GetValues(typeof(University)).Cast<University>(), university =>
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
