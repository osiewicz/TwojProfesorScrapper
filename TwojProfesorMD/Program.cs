using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibScrapeTP;
using LibScrapeTP.Entities;
using LibScrapeTP.ETLSteps.Extract;

namespace TwojProfesorMD
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            foreach (var val in Enum.GetValues(typeof(University)).Cast<University>().Take(1))
            {
                var c = new PageUrlExtracter(val, TimeSpan.FromSeconds(3));
                var before = DateTime.Now;
                var task = c.GetTutorPagesUrls().ToList();
                var fetcher = new TutorPageFetcher(Path.Combine(@"C:\Programowanie", val.ToString()));
                fetcher.GetPages(new HashSet<string>(task).ToList(), TimeSpan.FromSeconds(3));
                var count = task.Count;
                var after = DateTime.Now - before;

                Debug.WriteLine(count);
                Console.WriteLine(count);
            }

        }
    }
}
