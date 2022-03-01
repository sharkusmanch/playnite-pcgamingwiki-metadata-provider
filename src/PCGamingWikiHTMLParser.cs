using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Playnite.SDK;
using System.Globalization;
using Playnite.SDK.Models;

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
            ParseInput();
            ParseCloudSync();
            ParseInfobox();
        }

        private void RemoveCitationsFromHTMLNode(HtmlNode node)
        {
            var removeChil = node.SelectNodes(".//sup");

            if (removeChil != null)
            {
                node.RemoveChildren(removeChil);
            }
        }

        private void ParseInput()
        {
            var table = this.doc.DocumentNode.SelectSingleNode("//table[@id='table-settings-input']");

            if (table == null)
            {
                return;
            }

            var rows = table.SelectNodes("//tr[@class='template-infotable-body table-settings-input-body-row']");

            string param = "";

            foreach (HtmlNode row in rows)
            {
                foreach (HtmlNode child in row.SelectNodes(".//th|td"))
                {
                    switch (child.Attributes["class"].Value)
                    {
                        case "table-settings-input-body-parameter":
                            param = child.FirstChild.InnerText;
                            break;
                        case "table-settings-input-body-rating":
                            switch (param)
                            {
                                case "Full controller support":
                                    this.game.AddControllerSupport(child.FirstChild.Attributes["title"].Value);
                                    break;
                                default:
                                    break;

                            }
                            param = "";
                            break;
                    }
                }
            }
        }

        private void ParseCloudSync()
        {
            var table = this.doc.DocumentNode.SelectSingleNode("//table[@id='table-cloudsync']");

            if (table == null)
            {
                return;
            }

            var rows = table.SelectNodes("//tr[@class='template-infotable-body table-cloudsync-body-row']");

            string launcher = "";

            foreach (HtmlNode row in rows)
            {
                foreach (HtmlNode child in row.SelectNodes(".//th|td"))
                {
                    switch (child.Attributes["class"].Value)
                    {
                        case "table-cloudsync-body-system":
                            launcher = child.FirstChild.InnerText;
                            break;
                        case "table-cloudsync-body-rating":
                            this.game.AddCloudSaves(launcher, child.FirstChild.Attributes["title"].Value);
                            launcher = "";
                            break;
                    }
                }
            }
        }

        private void ParseInfobox()
        {
            HtmlNode table = this.doc.DocumentNode.SelectSingleNode("//table[@id='infobox-game']");
            string currentHeader = "";

            foreach (HtmlNode row in table.SelectNodes(".//tr"))
            {
                string key = "";

                foreach (HtmlNode child in row.SelectNodes(".//th|td"))
                {
                    RemoveCitationsFromHTMLNode(child);
                    const string pattern = @"[\t\s]";
                    string text = Regex.Replace(child.InnerText.Trim(), pattern, " ");

                    switch (child.Name)
                    {
                        case "th":
                            currentHeader = text;
                            break;

                        case "td":
                            switch (child.Attributes["class"].Value)
                            {
                                case "template-infobox-type":
                                    if (text == "")
                                        break;
                                    key = text;
                                    break;
                                case "template-infobox-icons":
                                    AddLinks(child);
                                    break;
                                case "template-infobox-info":
                                    if (text == "")
                                        break;
                                    switch (currentHeader)
                                    {
                                        case "Taxonomy":
                                            this.game.AddTaxonomy(key, text);
                                            break;
                                        case "Reception":
                                            AddReception(key, child);
                                            break;
                                        case "Release dates":
                                            ApplyReleaseDate(key, text);
                                            break;
                                        case "Engines":
                                            this.game.AddTaxonomy("Engines", text);
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

        private void AddReception(string aggregator, HtmlNode node)
        {
            int score;

            if (Int32.TryParse(node.SelectNodes(".//a")[0].InnerText, out score))
            {
                this.game.AddReception(aggregator, score);
            }
            else
            {
                logger.Error($"Unable to add reception {aggregator} {score}");
            }
        }

        private void ApplyReleaseDate(string platform, string releaseDate)
        {
            DateTime? date = ParseWikiDate(releaseDate);

            if (date == null)
            {
                return;
            }

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
                logger.Error($"Unable to parse date: {dateString}");
                return null;
            }
        }

        private void AddLinks(HtmlNode icons)
        {
            string url;
            foreach (var c in icons.ChildNodes)
            {
                url = c.ChildNodes[0].Attributes["href"].Value;
                switch (c.Attributes["Title"].Value)
                {
                    case var title when new Regex(@"^Official site$").IsMatch(title):
                        this.game.Links.Add(new Playnite.SDK.Models.Link("Official site", url));
                        break;
                    case var title when new Regex(@"GOG Database$").IsMatch(title):
                        this.game.Links.Add(new Playnite.SDK.Models.Link("GOG Database", url));
                        break;
                    default:
                        string[] linkTitle = c.Attributes["Title"].Value.Split(' ');
                        string titleComp = linkTitle[linkTitle.Length - 1];
                        this.game.Links.Add(new Playnite.SDK.Models.Link(titleComp, url));
                        break;
                }
            }
        }

        private void AddCompany(HtmlNode node, IList<MetadataProperty> list)
        {
            string company = node.InnerText;
            list.Add(new MetadataNameProperty(company));
        }

        private string ParseText(HtmlNode node)
        {
            var nodes = node.SelectNodes("./a");
            var currNode = nodes[nodes.Count - 1];

            return currNode.InnerText.Trim();
        }
    }
}
