﻿using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LibScrapeTP.Entities;

namespace LibScrapeTP.ETLSteps.Transform
{
    public static class TutorPageParser
    {
        private static readonly Regex SummaryGradePattern = new Regex("<input[^>]*value=\"(\\d\\d?)\"", RegexOptions.Compiled);
        private static readonly Regex OpinionGradePattern = new Regex(";\">(\\d\\d?)<", RegexOptions.Compiled);
        private static readonly Regex TitlePattern = new Regex(@"^([\p{Ll}\. ]*) (\p{Lu}\p{Ll}+[ -]?){2,} (\p{Lu})* (\p{Lu}\p{Ll}*[ -]?){2,}$", RegexOptions.Compiled);
        public static TutorSummaryPage Parse(Stream inputStream)
        {
            var htmlParser = new HtmlParser();
            var dom = htmlParser.ParseDocument(inputStream);
            var ret = new TutorSummaryPage();

            var opinionDivs = dom.All.Where(elem => elem.TagName.Equals("DIV") && elem.HasChildNodes && new[] { "post", "clearfix" }.All(elem.ClassList
                                                        .Contains));
            var opinions = new List<Opinion>();

            // Parse all opinions for a given tutor.
            foreach(var opinionAsDiv in opinionDivs)
            {
                var metadataDiv = opinionAsDiv.FirstElementChild.Children;
                var usernameSpan = metadataDiv.First(child => child.TagName.Equals("SPAN") && child.ClassList.Contains("username"));
                var username = usernameSpan.FirstElementChild.InnerHtml;
                var subjectAndDate = usernameSpan.Children.Skip(1).First().InnerHtml.Replace("b>", "").Replace("</", "")
                    .Replace("<", "").Split("|");
                Debug.Assert(subjectAndDate.Length > 1);

                var subject = subjectAndDate[0];
                var date = DateTime.Parse(subjectAndDate[1].Trim('|', ' '));

                // Parse grades for the opinion.
                // God forgive me for I have failed in my quest
                var gradeSpanAsText = metadataDiv.First(child => child.TagName.Equals("SPAN") && child.ClassList.Contains("description")).InnerHtml;
                // Extract grades with regex because why the f**k not?
                // https://stackoverflow.com/questions/1732348/regex-match-open-tags-except-xhtml-self-contained-tags
                var gradesAsMatches = OpinionGradePattern.Matches(gradeSpanAsText);

                Debug.Assert(gradesAsMatches.Count == 6);

                var grades = new GradeSet()
                {
                    AttractivenessOfClasses = int.Parse(gradesAsMatches[0].Groups[1].Value),
                    Competency = int.Parse(gradesAsMatches[1].Groups[1].Value),
                    EaseOfPassing = int.Parse(gradesAsMatches[2].Groups[1].Value),
                    Friendliness = int.Parse(gradesAsMatches[3].Groups[1].Value),
                    ScoringSystem = int.Parse(gradesAsMatches[4].Groups[1].Value),
                    AbsenceSystem = int.Parse(gradesAsMatches[5].Groups[1].Value),
                };

                var commentText = opinionAsDiv.Children.First(tag => tag.TagName.Equals("P")).InnerHtml;

                var res = new Opinion
                {
                    Name = username,
                    Subject = subject,
                    AddedOn = date,
                    Grades = grades,
                    Comment = commentText
                };
                opinions.Add(res);
            }

            ret.Opinions = opinions.ToArray();

            var _ = dom.All.First(x => x.ClassList != null && x.ClassList.Contains("post") && x.TagName.Equals("DIV")).FirstElementChild.InnerHtml.Replace("\n", "");//Children.Where(tag =>
                //tag.TagName.Equals("INPUT") && tag.Attributes.Length > 3 && tag.ClassList.Contains("knob")).ToList();
            //var gradesOnTop = string.Join(' ', _.Select(x=>x.ToString()));
            var gradesOnTopOfPage =
                SummaryGradePattern.Matches(_);
            Debug.Assert(gradesOnTopOfPage.Count == 6);
            
            ret.DisplayedGradeSet = new GradeSet()
            {
                AttractivenessOfClasses = int.Parse(gradesOnTopOfPage[0].Groups[1].Value),
                Competency = int.Parse(gradesOnTopOfPage[1].Groups[1].Value),
                EaseOfPassing = int.Parse(gradesOnTopOfPage[2].Groups[1].Value),
                Friendliness = int.Parse(gradesOnTopOfPage[3].Groups[1].Value),
                ScoringSystem = int.Parse(gradesOnTopOfPage[4].Groups[1].Value),
                AbsenceSystem = int.Parse(gradesOnTopOfPage[5].Groups[1].Value),
            };

            // A bit of a hack: instead of parsing the page we can reuse page title to get
            // prof's Name/surname, their title and major/minor department.

            const string titleDelimiter = " -";
            var posOfDelimiter = dom.Title.IndexOf(titleDelimiter, StringComparison.Ordinal);
            Debug.Assert(posOfDelimiter != -1);
            if (posOfDelimiter == -1)
            {
                throw new ArgumentException("Page title does not conform to predefined format.");
            }

            var title = dom.Title.Substring(0, posOfDelimiter).Trim();
            // Data should match regex: [a-z\. ]* ([A-Z][a-z][ -]?){2,} [A-Z]* ([A-Z][a-z][ -]?){2,}
            // (barring Polish letters).
            var titleAsGroups = TitlePattern.Match(title);

            Debug.Assert(titleAsGroups.Success);
            if (!titleAsGroups.Success)
            {
                throw new ArgumentException("Page title does not conform to predefined format.");
            }

            var academicTitles = string.Join(' ', titleAsGroups.Groups[1].Captures);
            var name = string.Join(' ', titleAsGroups.Groups[2].Captures);

            var shorthandUniversityName = string.Join(' ', titleAsGroups.Groups[3].Captures);
            Enum.TryParse<University>(shorthandUniversityName.ToLower(), out var university);
            var department = string.Join(' ', titleAsGroups.Groups[4].Captures);
            ret.Tutor = new Tutor()
            {
                MajorDepartment = department,
                Name = name,
                University =  university,
                AcademicTitle = TitleParser.Parse(academicTitles)
            };

            return ret;
        }
    }
}
