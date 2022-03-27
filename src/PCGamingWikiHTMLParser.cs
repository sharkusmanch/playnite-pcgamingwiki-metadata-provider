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
        private PCGWGameController gameController;

        public const short UndefinedPlayerCount = -1;

        public PCGamingWikiHTMLParser(string html, PCGWGameController gameController)
        {
            this.doc = new HtmlDocument();
            this.doc.LoadHtml(html);
            this.gameController = gameController;
        }

        public void ApplyGameMetadata()
        {
            ParseInput();
            ParseCloudSync();
            ParseInfobox();
            ParseMultiplayer();
        }

        private void RemoveCitationsFromHTMLNode(HtmlNode node)
        {
            var removeChil = node.SelectNodes(".//sup");

            if (removeChil != null)
            {
                node.RemoveChildren(removeChil);
            }
        }

        private IList<HtmlNode> SelectTableRowsByClass(string tableId, string rowClass)
        {
            var table = this.doc.DocumentNode.SelectSingleNode($"//table[@id='{tableId}']");

            if (table != null)
            {
                return table.SelectNodes($"//tr[@class='{rowClass}']");
            }

            return new List<HtmlNode>();
        }

        private void ParseMultiplayer()
        {
            var rows = SelectTableRowsByClass("table-network-multiplayer", "template-infotable-body table-network-multiplayer-body-row");
            string networkType = "";
            string rating = "";
            short playerCount = UndefinedPlayerCount;

            foreach (HtmlNode row in rows)
            {
                foreach (HtmlNode child in row.SelectNodes(".//th|td"))
                {
                    switch (child.Attributes["class"].Value)
                    {
                        case "table-network-multiplayer-body-parameter":
                            networkType = child.FirstChild.InnerText;
                            break;
                        case "table-network-multiplayer-body-rating":
                            rating = child.FirstChild.Attributes["title"].Value;
                            break;
                        case "table-network-multiplayer-body-players":
                            Int16.TryParse(child.FirstChild.InnerText, out playerCount);
                            break;
                        case "table-network-multiplayer-body-notes":
                            IList<string> notes = ParseMultiplayerNotes(child);

                            switch (networkType)
                            {
                                case "Local play":
                                    this.gameController.Game.AddMultiplayerLocal(rating, playerCount, notes);
                                    break;
                                case "LAN play":
                                    this.gameController.Game.AddMultiplayerLAN(rating, playerCount, notes);
                                    break;
                                case "Online play":
                                    this.gameController.Game.AddMultiplayerOnline(rating, playerCount, notes);
                                    break;
                                default:
                                    break;

                            }
                            rating = "";
                            networkType = "";
                            playerCount = UndefinedPlayerCount;
                            break;
                    }
                }
            }
        }

        private IList<string> ParseMultiplayerNotes(HtmlNode notes)
        {
            List<string> multiplayerTypes = new List<string>();

            Regex pattern = new Regex(@"class=""table-network-multiplayer-body-notes"">(?<mode1>(Co-op|Versus))?(,)?(&#32;)?(?<mode2>(Co-op|Versus))?<br>");
            Match match = pattern.Match(notes.OuterHtml);

            if (match.Groups["mode1"].Success)
            {
                multiplayerTypes.Add(match.Groups["mode1"].Value);
            }

            if (match.Groups["mode2"].Success)
            {
                multiplayerTypes.Add(match.Groups["mode2"].Value);
            }

            return multiplayerTypes;
        }

        private void ParseInput()
        {
            var rows = SelectTableRowsByClass("table-settings-input", "template-infotable-body table-settings-input-body-row");
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
                                    this.gameController.Game.AddFullControllerSupport(child.FirstChild.Attributes["title"].Value);
                                    break;
                                case "Controller support":
                                    this.gameController.Game.AddControllerSupport(child.FirstChild.Attributes["title"].Value);
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
            var rows = SelectTableRowsByClass("table-cloudsync", "template-infotable-body table-cloudsync-body-row");
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
                            this.gameController.Game.AddCloudSaves(launcher, child.FirstChild.Attributes["title"].Value);
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
                                            this.gameController.AddTaxonomy(key, text);
                                            break;
                                        case "Reception":
                                            AddReception(key, child);
                                            break;
                                        case "Release dates":
                                            ApplyReleaseDate(key, text);
                                            break;
                                        case PCGamingWikiType.Taxonomy.Engines:
                                            this.gameController.AddTaxonomy(PCGamingWikiType.Taxonomy.Engines, text);
                                            break;
                                        case "Developers":
                                            AddCompany(child, this.gameController.Game.Developers);
                                            break;
                                        case "Publishers":
                                            AddCompany(child, this.gameController.Game.Publishers);
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
                this.gameController.Game.AddReception(aggregator, score);
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

            this.gameController.Game.AddReleaseDate(platform, ParseWikiDate(releaseDate));
        }

        private DateTime? ParseWikiDate(string dateString)
        {
            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date) == true)
            {
                return date;
            }
            else
            {
                logger.Error($"Unable to parse date for {this.gameController.Game.Name}: {dateString}");
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
                        this.gameController.Game.Links.Add(new Playnite.SDK.Models.Link("Official site", url));
                        break;
                    case var title when new Regex(@"GOG Database$").IsMatch(title):
                        this.gameController.Game.Links.Add(new Playnite.SDK.Models.Link("GOG Database", url));
                        break;
                    default:
                        string[] linkTitle = c.Attributes["Title"].Value.Split(' ');
                        string titleComp = linkTitle[linkTitle.Length - 1];
                        this.gameController.Game.Links.Add(new Playnite.SDK.Models.Link(titleComp, url));
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
