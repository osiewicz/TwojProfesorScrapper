using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Io;
using LibScrapeTP;
using LibScrapeTP.Entities;
using TwojProfesor.Properties;
using Timer = System.Timers.Timer;


namespace TwojProfesorMD
{
    internal class Program
    {
        private const string extractTokenFile = "extracted";
        private static void Main(string[] args)
        {
            const string supportedOptions = "ETLDAT";
            var promptDelay = TimeSpan.FromSeconds(4);
            var t = new Stopwatch();
            using var c = new CancellationTokenSource();
            t.Start();
            Console.WriteLine(Resources.Header);
            Console.WriteLine(Resources.Welcome);
            Console.WriteLine(Resources.Options);
            Console.WriteLine();
            Console.WriteLine(Resources.AltOptions);
            var promptUser = Task.Run(() =>
            {
                Task.Delay(promptDelay, c.Token).Wait(c.Token); 
                Console.WriteLine(Resources.Prompt);
            }, c.Token);
            string response;

            do
            {
                response = Console.ReadLine();
            } while (!string.Join("", response.Select(char.ToUpper).Distinct()).Intersect(supportedOptions).Any());

            var shouldCancelPrompt = t.Elapsed < promptDelay;
            if (shouldCancelPrompt)
            {
                c.Cancel();
            }

        }
    }
}
