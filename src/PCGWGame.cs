using System;
using Playnite.SDK;
using Playnite.SDK.Models;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace PCGamingWikiMetadata
{
    public class PCGWGame : GenericItemOption
    {
        private readonly ILogger logger = LogManager.GetLogger();
        public int PageID { get; set; }

        private List<MetadataProperty> genres;
        public List<MetadataProperty> Genres { get { return genres; } }
        private List<MetadataProperty> developers;
        public List<MetadataProperty> Developers { get { return developers; } }
        private List<MetadataProperty> publishers;
        public List<MetadataProperty> Publishers { get { return publishers; } }
        private List<MetadataProperty> features;
        public List<MetadataProperty> Features { get { return features; } }
        private List<MetadataProperty> series;
        public List<MetadataProperty> Series { get { return series; } }
        private List<Link> links;
        public List<Link> Links { get { return links; } }
        private List<MetadataProperty> tags;
        public List<MetadataProperty> Tags { get { return tags; } }

        private IDictionary<string, int?> reception;
        private IDictionary<string, bool?> cloudSaves;

        private IDictionary<string, ReleaseDate?> ReleaseDates;

        public Game LibraryGame;

        public PCGWGame()
        {
            this.links = new List<Link>();
            this.genres = new List<MetadataProperty>();
            this.features = new List<MetadataProperty>();
            this.series = new List<MetadataProperty>();
            this.developers = new List<MetadataProperty>();
            this.publishers = new List<MetadataProperty>();
            this.tags = new List<MetadataProperty>();
            this.ReleaseDates = new Dictionary<string, ReleaseDate?>();
            this.reception = new Dictionary<string, int?>();
            this.cloudSaves = new Dictionary<string, bool?>();
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

        public void AddControllerSupport(string description)
        {
            if (description == PCGamingWikiType.Rating.NativeSupport)
            {
                this.AddFeature("Full Controller Support");
            }
        }

        public void AddCloudSaves(string launcher, string description)
        {
            BuiltinExtension? extension = LauncherNameToPluginID(launcher);

            if (BuiltinExtensions.GetExtensionFromId(this.LibraryGame.PluginId) == extension)
            {
                switch (description)
                {
                    case PCGamingWikiType.Rating.NativeSupport:
                        this.AddFeature("Cloud Saves");
                        break;
                    case PCGamingWikiType.Rating.NotSupported:
                        this.AddTag("No Cloud Saves");
                        break;
                    case PCGamingWikiType.Rating.Unknown:
                        break;
                }
            }
        }

        private BuiltinExtension? LauncherNameToPluginID(string launcher)
        {
            switch (launcher)
            {
                case PCGamingWikiType.Cloud.Steam:
                    return BuiltinExtension.SteamLibrary;
                case PCGamingWikiType.Cloud.Xbox:
                    return BuiltinExtension.XboxLibrary;
                case PCGamingWikiType.Cloud.GOG:
                    return BuiltinExtension.GogLibrary;
                case PCGamingWikiType.Cloud.Epic:
                    return BuiltinExtension.EpicLibrary;
                case PCGamingWikiType.Cloud.Ubisoft:
                    return BuiltinExtension.UplayLibrary;
                case PCGamingWikiType.Cloud.Origin:
                    return BuiltinExtension.OriginLibrary;
                default:
                    return null;
            }
        }

        public void AddTaxonomy(string type, string value)
        {
            switch (type)
            {
                case "Monetization":
                    break;
                case "Microtransactions":
                    break;
                case "Modes":
                    AddCSVFeatures(value);
                    break;
                case "Pacing":
                    AddCSVTags(value);
                    break;
                case "Perspectives":
                    AddCSVTags(value);
                    break;
                case "Controls":
                    AddCSVTags(value);
                    break;
                case "Genres":
                    AddGenres(value);
                    break;
                case "Vehicles":
                    AddCSVTags(value);
                    break;
                case "Art styles":
                    AddCSVTags(value);
                    break;
                case "Themes":
                    AddCSVTags(value);
                    break;
                case "Engines":
                    AddCSVTags(value);
                    break;
                case "Series":
                    AddCSVSeries(value);
                    break;
                default:
                    logger.Debug($"Unknown taxonomy {type}");
                    break;
            }
        }

        public void AddTag(string t)
        {
            this.tags.Add(new MetadataNameProperty(t));
        }

        public void AddFeature(string t)
        {
            this.features.Add(new MetadataNameProperty(t));
        }

        public void AddSeries(string t)
        {
            this.series.Add(new MetadataNameProperty(t));
        }

        private void AddCSVSeries(string csv)
        {
            string[] tags = SplitCSVString(csv);

            foreach (string tag in tags)
            {
                AddSeries(tag);
            }
        }

        private void AddCSVFeatures(string csv)
        {
            string[] tags = SplitCSVString(csv);

            foreach (string tag in tags)
            {
                AddFeature(tag);
            }
        }

        private void AddCSVTags(string csv)
        {
            string[] tags = SplitCSVString(csv);

            foreach (string tag in tags)
            {
                AddTag(tag);
            }
        }

        public ReleaseDate? WindowsReleaseDate()
        {
            ReleaseDate? date;

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
            this.ReleaseDates[platform] = new ReleaseDate((DateTime)date);
        }

        public string[] SplitCSVString(string csv)
        {
            TextFieldParser parser = new TextFieldParser(new StringReader(csv));
            parser.SetDelimiters(",");
            return parser.ReadFields();
        }

        public void AddGenres(string genreCsv)
        {
            string[] genres = SplitCSVString(genreCsv);

            foreach (string genre in genres)
            {
                this.genres.Add(new MetadataNameProperty(genre));
            }
        }

        public void SetXboxPlayAnywhere()
        {
            if (BuiltinExtensions.GetExtensionFromId(this.LibraryGame.PluginId) == BuiltinExtension.XboxLibrary)
            {
                this.AddTag("Xbox Play Anywhere");
            }
        }
    }
}
