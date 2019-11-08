using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibScrapeTP;
using LibScrapeTP.Entities;

namespace TwojProfesorMD
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var c = new TpParser(University.UEK);
            var task = c.GetTutorPages();
            int count = task.Count();
            Debug.WriteLine(count);
            Console.WriteLine(count);
        }
    }
}
