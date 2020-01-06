using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Text;
using LibScrapeTP;
using LibScrapeTP.Entities;

namespace LoadDriver.Sqlite
{
    class SQLiteWrapper
    {
        private readonly SQLiteConnection _conn;

        public SQLiteWrapper(SQLiteConnection conn)
        {
            this._conn = conn;
        }

        public int? Get(Tutor tutor)
        {
            using var comm = _conn.CreateCommand();
            comm.CommandText =
                "SELECT id from Tutor where name=@name and title=@title and department=@department and university=@university";

            comm.Parameters.AddWithValue("@name", tutor.Name);
            comm.Parameters.AddWithValue("@title", tutor.AcademicTitle.ToString());
            comm.Parameters.AddWithValue("@department", tutor.MajorDepartment);
            comm.Parameters.AddWithValue("@university", tutor.University.ToString());

            var res = comm.ExecuteReader();
            if (!res.HasRows)
            {
                // Tutor exists already - nothing to do.
                return null;
            }
            res.Read();

            return res.GetInt32(0);
        }
        public int Add(Tutor tutor)
        {
            var id = Get(tutor);
            if (!id.HasValue)
            {
                using var command = _conn.CreateCommand();
                command.CommandText =
                    "INSERT INTO Tutor(name, title, department, university) VALUES(@name, @title, @department, @university)";
                command.Parameters.AddWithValue("@name", tutor.Name);
                command.Parameters.AddWithValue("@title", tutor.AcademicTitle.ToString());
                command.Parameters.AddWithValue("@department", tutor.MajorDepartment);
                command.Parameters.AddWithValue("@university", tutor.University.ToString());
                command.ExecuteNonQuery();

                id = Get(tutor);
                Debug.Assert(id.HasValue);
            }

            return id.Value;
        }

        public int? Get(GradeSet gs)
        {
            using var comm = _conn.CreateCommand();
            comm.CommandText =
                "SELECT id from GradeSet where attractiveness=@attr and competency=@competency and eop=@eop and friendliness=@friendliness and scoring=@scoring and absence=@absence";
            comm.Parameters.AddWithValue("@attr", gs.AttractivenessOfClasses);
            comm.Parameters.AddWithValue("@competency", gs.Competency);
            comm.Parameters.AddWithValue("@eop", gs.EaseOfPassing);
            comm.Parameters.AddWithValue("@friendliness", gs.Friendliness);
            comm.Parameters.AddWithValue("@scoring", gs.ScoringSystem);
            comm.Parameters.AddWithValue("@absence", gs.AbsenceSystem);

            var res = comm.ExecuteReader();
            if (!res.HasRows)
            {
                return null;
            }

            res.Read();
            return res.GetInt32(0);
        }
        public int Add(GradeSet gs)
        {
            var id = Get(gs);
            if (!id.HasValue)
            {
                using var command = _conn.CreateCommand();
                command.CommandText =
                    "INSERT INTO GradeSet(attractiveness, competency, eop, friendliness, scoring, absence) VALUES(@attractiveness, @competency, @eop, @friendliness, @scoring, @absence)";
                command.Parameters.AddWithValue("@attractiveness", gs.AttractivenessOfClasses);
                command.Parameters.AddWithValue("@competency", gs.Competency);
                command.Parameters.AddWithValue("@eop", gs.EaseOfPassing);
                command.Parameters.AddWithValue("@friendliness", gs.Friendliness);
                command.Parameters.AddWithValue("@scoring", gs.ScoringSystem);
                command.Parameters.AddWithValue("@absence", gs.AbsenceSystem);

                command.ExecuteNonQuery();
                // Retry after insert.
                id = Get(gs);

                Debug.Assert(id.HasValue);
            }

            return id.Value;
        }

        public int? Get(Opinion opinion)
        {
            using var comm = _conn.CreateCommand();
            comm.CommandText =
                "SELECT id from Opinion where username=@username and date=@date and subject=@subject and comment=@comment";
            comm.Parameters.AddWithValue("@username", opinion.Name);
            comm.Parameters.AddWithValue("@date", opinion.AddedOn);
            comm.Parameters.AddWithValue("@subject", opinion.Subject);
            comm.Parameters.AddWithValue("@comment", opinion.Comment);

            var res = comm.ExecuteReader();
            if (!res.HasRows)
            {
                return null;
            }

            res.Read();
            return res.GetInt32(0);
        }
        public int Add(Opinion opinion, Tutor tutor)
        {
            var id = Get(opinion);
            if (!id.HasValue)
            {
                var gsid = Add(opinion.Grades);
                var tid = Add(tutor);
                using var command = _conn.CreateCommand();
                command.CommandText =
                    "INSERT INTO Opinion(tutor_id, username, gsid, date, subject, comment) VALUES(@tid, @uname, @gsid, @date, @subject, @comment)";
                command.Parameters.AddWithValue("@tid", tid);
                command.Parameters.AddWithValue("@uname", opinion.Name);
                command.Parameters.AddWithValue("@gsid", gsid);
                command.Parameters.AddWithValue("@date", opinion.AddedOn);
                command.Parameters.AddWithValue("@subject", opinion.Subject);
                command.Parameters.AddWithValue("@comment", opinion.Comment);

                command.ExecuteNonQuery();
                // Retry after insert.
                id = Get(opinion);

                Debug.Assert(id.HasValue);
            }

            return id.Value;
        }

        public List<Tutor> GetAll()
        {
            using var command = _conn.CreateCommand();
            command.CommandText = "SELECT name, title, department, university from Tutor";
            var r = command.ExecuteReader();
            var ret = new List<Tutor>();
            while (r.Read())
            {
                ret.Add(new Tutor()
                {
                    Name = r.GetString(0),
                    AcademicTitle = Enum.Parse<AcademicTitle>(r.GetString(1)),
                    MajorDepartment = r.GetString(2),
                    University = Enum.Parse<University>(r.GetString(3))

                });
            };

            return ret;
        }
    }
}
