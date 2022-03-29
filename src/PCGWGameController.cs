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
        
        private Dictionary<string, Func<bool>> taxonomySettings;
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
            this.taxonomySettings = new Dictionary<string, Func<bool>>()
            {
                { PCGamingWikiType.Taxonomy.Engines, new Func<bool>( () => this.settings.ImportTagEngine) },
                { PCGamingWikiType.Taxonomy.Monetization, new Func<bool>( () => this.settings.ImportTagMonetization) },
                { PCGamingWikiType.Taxonomy.Microtransactions, new Func<bool>( () => this.settings.ImportTagMicrotransactions) },
                { PCGamingWikiType.Taxonomy.Modes, new Func<bool>( () => this.settings.ImportTagModes) },
                { PCGamingWikiType.Taxonomy.Pacing, new Func<bool>( () => this.settings.ImportTagPacing) },
                { PCGamingWikiType.Taxonomy.Perspectives, new Func<bool>( () => this.settings.ImportTagPerspectives) },
                { PCGamingWikiType.Taxonomy.Controls, new Func<bool>( () => this.settings.ImportTagControls) },
                { PCGamingWikiType.Taxonomy.Vehicles, new Func<bool>( () => this.settings.ImportTagVehicles) },
                { PCGamingWikiType.Taxonomy.Themes, new Func<bool>( () => this.settings.ImportTagThemes) },
                { PCGamingWikiType.Taxonomy.ArtStyles, new Func<bool>( () => this.settings.ImportTagArtStyle) },
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
                { PCGamingWikiType.Taxonomy.Series, new Action<string>( value => this.Game.AddCSVSeries(value)) },

            };
        }

        public void AddTaxonomy(string key, string text)
        {
            Func<bool> enabled;
            bool settingExists = this.taxonomySettings.TryGetValue(key, out enabled);

            if (settingExists && !(enabled.Invoke()))
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
    }
}