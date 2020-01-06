using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using LibScrapeTP.Entities;
using LoadDriver.Sqlite;
using Microsoft.Data.Sqlite;
using ProtoBuf;

namespace LoadDriver
{
    internal class Program
    {
        internal class ParseOptions
        {
            [Option('i', "input_dir", Required = true)]
            public string InputPath { get; set; }

            [Option('s', "sqlite_file", Default = "twojprofesor.sqlite")]
            public string SqlitePath { get; set; }

            [Option('f', "flush", Default = false)]
            public bool Flush { get; set; }

            [Option('e', "export", Default = false)]
            public bool Export { get; set; }

            [Option('d', "delete_input", Default = true)]
            public bool DeleteInput { get; set; }

            [Option('c', "export_csv", Default = false)]
            public bool ExportCsv { get; set; }
        }

        private static void Main(string[] args)
        { 
            Parser.Default.ParseArguments<ParseOptions>(args).WithParsed(opts =>
            {
                var shouldCreateNewDb = !File.Exists(opts.SqlitePath);
                if ((opts.Export || opts.ExportCsv) && !shouldCreateNewDb)
                {

                }

                if (opts.Flush || shouldCreateNewDb)
                {
                    var tmp = Path.GetTempFileName();
                    CreateDBSchema.CreateDatabase(tmp);

                    File.Delete(opts.SqlitePath);
                    File.Move(tmp, opts.SqlitePath);
                }
                using var conn = new SqliteConnection($@"URI=file:{opts.SqlitePath}");
                Parallel.ForEach(
                    Directory.EnumerateDirectories(opts.InputPath).SelectMany(Directory.EnumerateDirectories)
                        .SelectMany(Directory.EnumerateDirectories).SelectMany(Directory.EnumerateFiles),
                    tutorFilePath =>
                    {
                        using var file = File.OpenRead(tutorFilePath);
                        var deserialized = Serializer.Deserialize<TutorSummaryPage>(file);
                        var db = new SQLiteWrapper(conn);
                        foreach (var opinion in deserialized.Opinions)
                        {
                            // Could potentially be optimized in terms of Tutor lookup - right now
                            // each new opinion requires looking up the same tutor, even though
                            // only one time would suffice.
                            db.Add(opinion, deserialized.Tutor);
                        }
                    });

                if (opts.DeleteInput)
                {
                    Directory.Delete(opts.InputPath);
                    Directory.CreateDirectory(opts.InputPath);
                }
            });
        }
    }
}
