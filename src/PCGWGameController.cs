using Playnite.SDK;
using Playnite.SDK.Models;
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
                { PCGamingWikiType.Video.FPS120Plus, new Func<bool>( () => this.settings.ImportFeatureFramerate120) },
                { PCGamingWikiType.Video.FPS60, new Func<bool>( () => this.settings.ImportFeatureFramerate60) },
                { PCGamingWikiType.Video.Ultrawide, new Func<bool>( () => this.settings.ImportFeatureUltrawide) },
                { PCGamingWikiType.Video.VR, new Func<bool>( () => this.settings.ImportFeatureVR) },
                { PCGamingWikiType.VRHeadsets.HTCVive, new Func<bool>( () => this.settings.ImportFeatureVRHTCVive) },
                { PCGamingWikiType.VRHeadsets.OculusRift, new Func<bool>( () => this.settings.ImportFeatureVROculusRift) },
                { PCGamingWikiType.VRHeadsets.OSVR, new Func<bool>( () => this.settings.ImportFeatureVROSVR) },
                { PCGamingWikiType.VRHeadsets.WindowsMixedReality, new Func<bool>( () => this.settings.ImportFeatureVRWMR) },
            };

            this.taxonomyFunctions = new Dictionary<string, Action<string>>()
            {
                { PCGamingWikiType.Taxonomy.Engines, new Action<string>( value => this.Game.AddCSVTags(value, ResourceProvider.GetString("LOCPCGWSettingsImportTagEngine"), this.settings.AddTagPrefix)) },
                { PCGamingWikiType.Taxonomy.Themes, new Action<string>( value => this.Game.AddCSVTags(value, ResourceProvider.GetString("LOCPCGWSettingsImportTagThemes"), this.settings.AddTagPrefix)) },
                { PCGamingWikiType.Taxonomy.ArtStyles, new Action<string>( value => this.Game.AddCSVTags(value, ResourceProvider.GetString("LOCPCGWSettingsImportTagArtStyle"), this.settings.AddTagPrefix)) },
                { PCGamingWikiType.Taxonomy.Vehicles, new Action<string>( value => this.Game.AddCSVTags(value, ResourceProvider.GetString("LOCPCGWSettingsImportTagVehicles"), this.settings.AddTagPrefix)) },
                { PCGamingWikiType.Taxonomy.Controls, new Action<string>( value => this.Game.AddCSVTags(value, ResourceProvider.GetString("LOCPCGWSettingsImportTagControls"), this.settings.AddTagPrefix)) },
                { PCGamingWikiType.Taxonomy.Perspectives, new Action<string>( value => this.Game.AddCSVTags(value, ResourceProvider.GetString("LOCPCGWSettingsImportTagPerspectives"), this.settings.AddTagPrefix)) },
                { PCGamingWikiType.Taxonomy.Pacing, new Action<string>( value => this.Game.AddCSVTags(value, ResourceProvider.GetString("LOCPCGWSettingsImportTagPacing"), this.settings.AddTagPrefix)) },
                { PCGamingWikiType.Taxonomy.Modes, new Action<string>( value => this.Game.AddCSVFeatures(value)) },
                { PCGamingWikiType.Taxonomy.Genres, new Action<string>( value => this.Game.AddGenres(value)) },
                { PCGamingWikiType.Taxonomy.Series, new Action<string>( value => this.Game.AddCSVSeries(value)) },
            };
        }

        private bool SettingExistsAndEnabled(string key)
        {
            Func<bool> enabled;
            bool settingExists = this.settingsMap.TryGetValue(key, out enabled);
            return settingExists && enabled.Invoke();
        }

        private bool IsSettingDisabled(string key)
        {
            Func<bool> enabled;
            bool settingExists = this.settingsMap.TryGetValue(key, out enabled);
            return settingExists && !(enabled.Invoke());
        }

        public void AddTaxonomy(string key, string text)
        {
            if (IsSettingDisabled(key))
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

        public void AddVRFeature(string headset, string rating)
        {
            if (IsSettingDisabled(PCGamingWikiType.Video.VR))
            {
                return;
            }

            if (SettingExistsAndEnabled(headset) && NativeOrLimitedSupport(rating))
            {
                this.Game.AddVRFeature();
            }
        }

        public void AddVideoFeature(string key, string rating)
        {
            if (IsSettingDisabled(key) || !NativeOrLimitedSupport(rating))
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
                case PCGamingWikiType.Video.FPS60:
                    this.Game.SetFramerate60();
                    break;
                case PCGamingWikiType.Video.FPS120Plus:
                    this.Game.SetFramerate120Plus();
                    break;
                case PCGamingWikiType.Video.Ultrawide:
                    this.Game.AddFeature("Ultra-widescreen");
                    break;
                case PCGamingWikiType.Video.FPS60And120:
                    this.AddVideoFeature(PCGamingWikiType.Video.FPS60, rating);
                    this.AddVideoFeature(PCGamingWikiType.Video.FPS120Plus, rating);
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

        public void AddDeveloper(string name)
        {
            this.Game.Developers.Add(new MetadataNameProperty(name));
        }

        public void AddPublisher(string name)
        {
            this.Game.Publishers.Add(new MetadataNameProperty(name));
        }
    }
}
