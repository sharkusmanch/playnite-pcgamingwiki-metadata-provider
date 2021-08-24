using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Playnite.SDK;
using System.Globalization;

namespace PCGamingWikiMetadata
{

    public class PCGamingWikiHTMLParser
    {
        private readonly ILogger logger = LogManager.GetLogger();
        private HtmlDocument doc;
        private PCGWGame game;

        public PCGamingWikiHTMLParser(string html, PCGWGame game)
        {
            this.doc = new HtmlDocument();
            this.doc.LoadHtml(html);
            this.game = game;
        }

        public void ApplyGameMetadata()
        {
            var table = this.doc.DocumentNode.SelectSingleNode("//table[@id='infobox-game']");
            string currentHeader = "";

            foreach (var row in table.SelectNodes(".//tr"))
            {
                string key = "";

                foreach (var child in row.SelectNodes(".//th|td"))
                {
                    const string pattern = @"[\t\s]";
                    string text = Regex.Replace(child.InnerText.Trim(), pattern, " ");

                    switch(child.Name) {
                    case "th":
                        currentHeader = text;
                        break;

                    case "td":
                        switch(child.Attributes["class"].Value)
                        {
                            case "template-infobox-type":
                                if (text == "")
                                    break;
                                key = text;
                                break;
                            case "template-infobox-icons":
                                foreach (var c in  child.ChildNodes)
                                {
                                    string[] linkTitle = c.Attributes["Title"].Value.Split(' ');
                                    string title = linkTitle[linkTitle.Length - 1];
                                    string url = c.ChildNodes[0].Attributes["href"].Value;
                                    this.game.Links.Add(new Playnite.SDK.Models.Link(title, url));
                                }
                                break;
                            case "template-infobox-info":
                                if (text == "")
                                    break;
                                switch(currentHeader) {
                                    case "Taxonomy":
                                        this.game.AddTaxonomy(key, text);
                                        break;
                                    case "Release dates":
                                        ApplyReleaseDate(key, text);
                                        break;
                                    case "Developers":
                                        AddCompany(child, this.game.Developers);
                                        break;
                                    case "Publishers":
                                        AddCompany(child, this.game.Publishers);
                                        break;
                                    default:
                                        logger.Debug($"ApplyGameMetadata unknown header {currentHeader}");
                                        break;
                                }
                                break;
                        }
                        break;
                    }
                }
            }
        }

        private void ApplyReleaseDate(string platform, string releaseDate)
        {
            this.game.AddReleaseDate(platform, ParseWikiDate(releaseDate));
        }

        private DateTime? ParseWikiDate(string dateString)
        {
            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) == true)
            {
                return date;
            }
            else
            {
                return null;
            }
        }

        private void AddCompany(HtmlNode node, IList<string> list)
        {
            string company = ParseCompany(node);
            if (company == null)
            {
                logger.Debug("Unable to parse company");
                return;
            }
            list.Add(company);
        }

        private string ParseCompany(HtmlNode node)
        {
            // foreach (var c in node.ChildNodes)
            // {
            //     var attr = c.Attributes["target"];
            //     if (attr != null)
            //     {
            //         return c.InnerText;
            //     }
            // }

            return node.ChildNodes[1].InnerText;
        }
    }
}
