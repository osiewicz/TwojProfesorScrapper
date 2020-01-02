using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace LibScrapeTP.ETLSteps.Transform
{
    public static class TutorPageParser
    {
        public static Entities.TutorSummaryPage Parse(Stream inputStream)
        {
            var htmlParser = new HtmlParser();
            var dom = htmlParser.ParseDocument(inputStream);
            var ret = new Entities.TutorSummaryPage();

            var opinionDivs = dom.All.Where(elem => elem.TagName.Equals("DIV") && elem.HasChildNodes && new[] { "post", "clearfix" }.All(elem.ClassList
                                                        .Contains));
            var opinions = new List<Entities.Opinion>();
            foreach(var opinionAsDiv in opinionDivs)
            {
                var usernameSpan = opinionAsDiv.Children.First(child => child.TagName.Equals("SPAN") && child.ClassList.Contains("username"));
                var username = usernameSpan.FirstChild.TextContent;
                var subjectAndDate = usernameSpan.Children.Skip(1).First().InnerHtml.Split('\n');
                Debug.Assert(subjectAndDate.Length > 1);

                var subject = subjectAndDate[0];
                var date = DateTime.Parse(subjectAndDate[1].Trim('|', ' '));

                var res = new Entities.Opinion
                {
                    Name = username,
                    Subject = subject,
                    AddedOn = date,
                };
                opinions.Add(res);
            }

            return ret;
        }
    }
}
