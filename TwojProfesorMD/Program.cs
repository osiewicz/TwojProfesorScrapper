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
using TwojProfesor;
using TwojProfesor.Properties;
using Timer = System.Timers.Timer;


namespace TwojProfesorMD
{
    internal class Program
    {
        private const string extractTokenFile = "extracted";
        private const string transformTokenFile = "transformed";
        private const string loadTokenFile = "loaded";
        private const string pathToExtractDriver = @"C:\Programowanie\xD\ExtractDriver\bin\Debug\netcoreapp3.1\ExtractDriver.exe";
        private const string pathToTransformDriver = @"C:\Programowanie\xD\TransformDriver\bin\Debug\netcoreapp3.1\TransformDriver.exe";
        private const string pathToLoadDriver = @"C:\Programowanie\xD\LoadDriver\bin\Debug\netcoreapp3.1\LoadDriver.exe";
        private static string _transformInputPath = string.Empty;
        private static string _transformOutputPath = string.Empty;
        private static void ClearTokens()
        {
            foreach (var token in new[] { extractTokenFile, transformTokenFile, loadTokenFile })
            {
                File.Delete(token);
            }
        }
        private static void ExtractHandler()
        {
            File.Create(extractTokenFile);
            Console.WriteLine(ExtractResources.Prompt);
            while (!Directory.Exists(_transformInputPath))
            {
                _transformInputPath = Console.ReadLine();
            }

            var handle = Process.Start(pathToExtractDriver, $"-p {_transformInputPath}");
            handle.WaitForExit();
        }


        private static void TransformHandler()
        {
            if (!File.Exists(extractTokenFile))
            {
                Console.WriteLine(TransformResources.WarnExtractNotDone);
                return;
            }

            File.Create(transformTokenFile);
            if (string.IsNullOrEmpty(_transformInputPath))
            {
                Console.WriteLine(TransformResources.Prompt1);
                while (!Directory.Exists(_transformInputPath))
                {
                    _transformInputPath = Console.ReadLine();
                }
            }

            Console.WriteLine(TransformResources.Prompt2);
            while (!Directory.Exists(_transformOutputPath))
            {
                _transformOutputPath = Console.ReadLine();
            }

            var handle = Process.Start(pathToTransformDriver, $"-i {_transformInputPath} -o {_transformOutputPath}");
            handle.WaitForExit();
        }

        private static void LoadHandler()
        {
            if (!File.Exists(transformTokenFile))
            {
                Console.WriteLine(TransformResources.WarnExtractNotDone);
                return;
            }
            File.Create(loadTokenFile);
            Console.WriteLine(Resources.AltOptions);
            var additionalChoices = Console.ReadLine().Select(char.ToUpper).ToList();
            if (additionalChoices.Contains('A'))
            {
                Console.WriteLine(LoadResources.Analyze);
                return;
            }

            var shouldDump = additionalChoices.Contains('D');
            var shouldDelete = additionalChoices.Contains('T');
            var dumpCommand = shouldDump ? "-e" : "";
            var truncateCommand = shouldDelete ? "-f" : "";
            while (!Directory.Exists(_transformOutputPath))
            {
                _transformOutputPath = Console.ReadLine();
            }

            var handle = Process.Start(pathToLoadDriver, $"-i {_transformOutputPath} {dumpCommand} {truncateCommand}");
            handle.WaitForExit();

            ClearTokens();
            if (shouldDelete && Directory.Exists(_transformInputPath))
            {
                foreach(var file in Directory.EnumerateDirectories(_transformInputPath).SelectMany(Directory.EnumerateFiles))
                {
                    File.Delete(file);
                }
            }

        }

        private static void Main(string[] args)
        {
            var handlers = new Dictionary<char, Action>()
            {
                {'E', ExtractHandler},
                {'T', TransformHandler},
                {'L', LoadHandler}
            };
            var promptDelay = TimeSpan.FromSeconds(4);
            var t = new Stopwatch();
            using var c = new CancellationTokenSource();
            t.Start();
            Console.WriteLine(Resources.Header);
            Console.WriteLine(Resources.Welcome);
            Console.WriteLine(Resources.Options);
            Console.WriteLine();

            var promptUser = Task.Run(() =>
            {
                Task.Delay(promptDelay, c.Token).Wait(c.Token);
                Console.WriteLine(Resources.Prompt);
            }, c.Token);
            List<char> selectedOptions;
            do
            {
                var response = Console.ReadLine();
                selectedOptions = response.Select(char.ToUpper).Distinct().Intersect(handlers.Keys).ToList();
            } while (!selectedOptions.Any());

            var shouldCancelPrompt = t.Elapsed < promptDelay;
            selectedOptions.Select(char.ToUpper).Select(x => handlers[x]).ToList().ForEach(act => act());
            if (shouldCancelPrompt)
            {
                c.Cancel();
            }

            promptUser.Wait();
            
        }
    }
}
