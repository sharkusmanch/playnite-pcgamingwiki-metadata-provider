using System;
using Playnite.SDK;
using Playnite.SDK.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace PCGamingWikiMetadata
{
    public class PCGWGame : GenericItemOption
    {
        private readonly ILogger logger = LogManager.GetLogger();
        public int PageID { get; set; }

        private List<string> genres;
        public List<string> Genres { get { return genres; } }
        private List<string> developers;
        public List<string> Developers { get { return developers; } }
        private List<string> publishers;
        public List<string> Publishers { get { return publishers; } }
        private List<string> features;
        public List<string> Features { get { return features; } }
        private List<Link> links;
        public List<Link> Links { get { return links; } }
        public IDictionary<string, int?> reception;

        private IDictionary<string, DateTime?> ReleaseDates;

        public string Series;

        public PCGWGame()
        {
            this.links = new List<Link>();
            this.genres = new List<string>();
            this.features = new List<string>();
            this.developers = new List<string>();
            this.publishers = new List<string>();
            this.ReleaseDates = new Dictionary<string, DateTime?>();
            this.reception = new Dictionary<string, int?>();
        }

        public PCGWGame(string name, int pageid) : this()
        {
            this.Name = name;
            this.PageID = pageid;
            AddPCGamingWikiLink();
        }

        protected Link PCGamingWikiLink()
        {
            string escapedName = Uri.EscapeUriString(this.Name);
            return new Link("PCGamingWiki", $"https://www.pcgamingwiki.com/wiki/{escapedName}");
        }

        public void AddPCGamingWikiLink()
        {
            this.links.Add(PCGamingWikiLink());
        }

        public void AddReception(string aggregator, int score)
        {
            this.reception[aggregator] = score;
        }

        public bool GetOpenCriticReception(out int? score)
        {
            return GetReception("OpenCritic", out score);
        }

        public bool GetIGDBReception(out int? score)
        {
            return GetReception("IGDB", out score);
        }
        public bool GetMetacriticReception(out int? score)
        {
            return GetReception("Metacritic", out score);
        }

        protected bool GetReception(string aggregator, out int? score)
        {
            return this.reception.TryGetValue(aggregator, out score);
        }

        public void AddTaxonomy(string type, string value)
        {
            switch (type)
            {
                case "Microtransactions":
                    break;
                case "Modes":
                    this.features.Add(value);
                    break;
                case "Pacing":
                    break;
                case "Perspectives":
                    break;
                case "Controls":
                    break;
                case "Genres":
                    this.genres.AddRange(SplitCSVString(value));
                    break;
                case "Vehicles":
                    break;
                case "Art styles":
                    break;
                case "Themes":
                    break;
                case "Series":
                    this.Series = value;
                    break;
                default:
                    logger.Debug($"Unknown taxonomy {type}");
                    break;
            }
        }

        public DateTime? WindowsReleaseDate()
        {
            DateTime? date;

            if (this.ReleaseDates.TryGetValue("Windows", out date))
            {
                return date;
            }
            else
            {
                return null;
            }
        }

        public void AddReleaseDate(string platform, DateTime? date)
        {
            this.ReleaseDates[platform] = date;
        }

        public string[] SplitCSVString(string csv)
        {
            TextFieldParser parser = new TextFieldParser(new StringReader(csv));
            parser.SetDelimiters(",");
            return parser.ReadFields();
        }
    }
}
