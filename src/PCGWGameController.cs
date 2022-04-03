using Playnite.SDK;
using System.Collections.Generic;
using System;

namespace PCGamingWikiMetadata
{
    public class PCGWGameController
    {
        private readonly ILogger logger = LogManager.GetLogger();
        public PCGWGame Game;
        private PCGamingWikiMetadataSettings settings;
        public PCGamingWikiMetadataSettings Settings { get { return settings; } }

        private Dictionary<string, Func<bool>> settingsMap;
        private Dictionary<string, Action<string>> taxonomyFunctions;

        public PCGWGameController(PCGamingWikiMetadataSettings settings)
        {
            this.settings = settings;
            InitalizeSettingsMappings();
        }

        public PCGWGameController(PCGWGame game, PCGamingWikiMetadataSettings settings)
        {
            this.Game = game;
            this.settings = settings;
            InitalizeSettingsMappings();
        }

        private void InitalizeSettingsMappings()
        {
            this.settingsMap = new Dictionary<string, Func<bool>>()
            {
                { PCGamingWikiType.Taxonomy.Engines, new Func<bool>( () => this.settings.ImportTagEngine) },
                { PCGamingWikiType.Taxonomy.Monetization, new Func<bool>( () => this.settings.ImportTagMonetization) },
                { PCGamingWikiType.Taxonomy.Microtransactions, new Func<bool>( () => this.settings.ImportTagMicrotransactions) },
                { PCGamingWikiType.Taxonomy.Pacing, new Func<bool>( () => this.settings.ImportTagPacing) },
                { PCGamingWikiType.Taxonomy.Perspectives, new Func<bool>( () => this.settings.ImportTagPerspectives) },
                { PCGamingWikiType.Taxonomy.Controls, new Func<bool>( () => this.settings.ImportTagControls) },
                { PCGamingWikiType.Taxonomy.Vehicles, new Func<bool>( () => this.settings.ImportTagVehicles) },
                { PCGamingWikiType.Taxonomy.Themes, new Func<bool>( () => this.settings.ImportTagThemes) },
                { PCGamingWikiType.Taxonomy.ArtStyles, new Func<bool>( () => this.settings.ImportTagArtStyle) },
                { PCGamingWikiType.Video.HDR, new Func<bool>( () => this.settings.ImportFeatureHDR) },
                { PCGamingWikiType.Video.RayTracing, new Func<bool>( () => this.settings.ImportFeatureRayTracing) },
            };

            this.taxonomyFunctions = new Dictionary<string, Action<string>>()
            {
                { PCGamingWikiType.Taxonomy.Engines, new Action<string>( value => this.Game.AddCSVTags(value)) },
                { PCGamingWikiType.Taxonomy.Themes, new Action<string>( value => this.Game.AddCSVTags(value)) },
                { PCGamingWikiType.Taxonomy.ArtStyles, new Action<string>( value => this.Game.AddCSVTags(value)) },
                { PCGamingWikiType.Taxonomy.Vehicles, new Action<string>( value => this.Game.AddCSVTags(value)) },
                { PCGamingWikiType.Taxonomy.Controls, new Action<string>( value => this.Game.AddCSVTags(value)) },
                { PCGamingWikiType.Taxonomy.Perspectives, new Action<string>( value => this.Game.AddCSVTags(value)) },
                { PCGamingWikiType.Taxonomy.Pacing, new Action<string>( value => this.Game.AddCSVTags(value)) },
                { PCGamingWikiType.Taxonomy.Modes, new Action<string>( value => this.Game.AddCSVFeatures(value)) },
                { PCGamingWikiType.Taxonomy.Genres, new Action<string>( value => this.Game.AddGenres(value)) },
                { PCGamingWikiType.Taxonomy.Series, new Action<string>( value => this.Game.AddCSVSeries(value)) },
            };
        }

        private bool IsSettingEnabled(string key)
        {
            Func<bool> enabled;
            bool settingExists = this.settingsMap.TryGetValue(key, out enabled);
            return settingExists && !(enabled.Invoke());
        }

        public void AddTaxonomy(string key, string text)
        {
            if (IsSettingEnabled(key))
            {
                return;
            }

            Action<string> action;
            bool functionExists = this.taxonomyFunctions.TryGetValue(key, out action);

            if (functionExists)
            {
                this.taxonomyFunctions[key](text);
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

        public void AddCloudSaves(string launcher, string description)
        {
            BuiltinExtension? extension = LauncherNameToPluginID(launcher);

            if (BuiltinExtensions.GetExtensionFromId(this.Game.LibraryGame.PluginId) == extension)
            {
                switch (description)
                {
                    case PCGamingWikiType.Rating.NativeSupport:
                        this.Game.AddFeature("Cloud Saves");
                        break;
                    case PCGamingWikiType.Rating.NotSupported:
                        if (this.settings.ImportTagNoCloudSaves)
                        {
                            this.Game.AddTag("No Cloud Saves");
                        }
                        break;
                    case PCGamingWikiType.Rating.Unknown:
                        break;
                }
            }
        }

        public void AddVideoFeature(string key, string rating)
        {
            if (IsSettingEnabled(key) && NativeOrLimitedSupport(rating))
            {
                return;
            }

            switch (key)
            {
                case PCGamingWikiType.Video.HDR:
                    this.Game.AddFeature("HDR");
                    break;
                case PCGamingWikiType.Video.RayTracing:
                    this.Game.AddFeature("Ray Tracing");
                    break;
                default:
                    break;
            }
        }

        public void SetXboxPlayAnywhere()
        {
            if (this.settings.ImportXboxPlayAnywhere)
            {
                this.Game.SetXboxPlayAnywhere();
            }
        }

        private bool NativeOrLimitedSupport(string rating)
        {
            return rating == PCGamingWikiType.Rating.NativeSupport ||
                rating == PCGamingWikiType.Rating.Limited;
        }

        public void AddMultiplayer(string networkType, string rating, short playerCount, IList<string> notes)
        {
            if (!this.settings.ImportMultiplayerTypes)
            {
                return;
            }

            switch (networkType)
            {
                case PCGamingWikiType.Multiplayer.Local:
                    this.Game.AddMultiplayerLocal(rating, playerCount, notes);
                    break;
                case PCGamingWikiType.Multiplayer.LAN:
                    this.Game.AddMultiplayerLAN(rating, playerCount, notes);
                    break;
                case PCGamingWikiType.Multiplayer.Online:
                    this.Game.AddMultiplayerOnline(rating, playerCount, notes);
                    break;
                case PCGamingWikiType.Multiplayer.Asynchronous:
                    this.Game.AddMultiplayerAsynchronous(rating, playerCount, notes);
                    break;
                default:
                    break;

            }
        }
    }
}