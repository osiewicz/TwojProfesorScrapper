using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using LibScrapeTP.Entities;
using LoadDriver.Sqlite;
using System.Data.SQLite;
using ProtoBuf;
using System.Threading;

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
                    using var connc = new SQLiteConnection($@"Data Source={opts.SqlitePath};Version=3;");
                    connc.Open();
                    var wrap = new SQLiteWrapper(connc);
                    var tutors = wrap.GetAll();
                    using var file = File.OpenWrite("tutors.csv");
                    using var sw = new StreamWriter(file);
                    foreach (var tut in tutors)
                    {
                        sw.WriteLine(string.Join(',', tut.Name, tut.University, tut.AcademicTitle, tut.MajorDepartment));
                    }
                }

                if (opts.Flush || shouldCreateNewDb)
                {
                    var tmp = Path.GetTempFileName();
                    CreateDBSchema.CreateDatabase(tmp);

                    File.Delete(opts.SqlitePath);
                    File.Move(tmp, opts.SqlitePath);
                }

                using var conn = new SQLiteConnection($@"Data Source={opts.SqlitePath};Version=3;");
                conn.Open();
                var d = Directory.EnumerateDirectories(opts.InputPath).SelectMany(Directory.EnumerateFiles).ToList();
                Console.WriteLine($"Processing {d.Count} input files.");

                var countOfOpinions = 0;
                Parallel.ForEach(
                    Directory.EnumerateFiles(opts.InputPath),
                    tutorFilePath =>
                    {
                        using var file = File.OpenRead(tutorFilePath);
                        var deserialized = Serializer.Deserialize<TutorSummaryPage>(file);
                        var db = new SQLiteWrapper(conn);
                        var count = 0;
                        if (deserialized.Opinions != null)
                        {
                            foreach (var opinion in deserialized.Opinions)
                            {
                                // Could potentially be optimized in terms of Tutor lookup - right now
                                // each new opinion requires looking up the same tutor, even though
                                // only one time would suffice.
                                db.Add(opinion, deserialized.Tutor);
                            }
                            count = deserialized.Opinions.Length;
                        } else
                        {
                            db.Add(deserialized.Tutor);
                        }
                        Interlocked.Add(ref countOfOpinions, count);
                    });
                conn.Close();

                Console.WriteLine($"Extracted {countOfOpinions} opinions");
            });
        }
    }
}
