using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace LoadDriver.Sqlite
{
    internal static class CreateDBSchema
    {
        public static void CreateDatabase(string path)
        {
            SQLiteConnection.CreateFile(path);
            using var conn = new SQLiteConnection($"Data Source={path};Version=3;");
            conn.Open();
            foreach (var commText in new[] { "DROP TABLE IF EXISTS Tutor",
                "CREATE TABLE Tutor(id INTEGER PRIMARY KEY, name TEXT, title TEXT, department TEXT, university TEXT)",
                "DROP TABLE IF EXISTS GradeSet",
                "CREATE TABLE GradeSet(id INTEGER PRIMARY KEY, attractiveness INTEGER, competency INTEGER, eop INTEGER, friendliness INTEGER, scoring INTEGER, absence INTEGER)",
                "DROP TABLE IF EXISTS Opinion",
                "CREATE TABLE Opinion(id INTEGER PRIMARY KEY, tutor_id  INTEGER, username TEXT, gsid INTEGER, date DATE, subject TEXT, comment TEXT, " + 
                "FOREIGN KEY (tutor_id) REFERENCES Tutor(id), FOREIGN KEY (gsid) REFERENCES GradeSet(id))",
            })
            {
                using var comm = conn.CreateCommand();
                comm.CommandText = commText;
                comm.ExecuteNonQuery();
            }

        }
    }
}
