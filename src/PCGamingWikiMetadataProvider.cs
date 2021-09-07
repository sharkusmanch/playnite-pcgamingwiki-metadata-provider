using Playnite.SDK.Plugins;
using Playnite.SDK;
using Playnite.SDK.Models;
using System.Collections.Generic;
using System;

namespace PCGamingWikiMetadata
{
    public class PCGamingWikiMetadataProvider : OnDemandMetadataProvider
    {
        private readonly MetadataRequestOptions options;
        private readonly PCGamingWikiMetadata plugin;

        private PCGWClient client;

        private PCGWGame pcgwData;
        private static readonly ILogger logger = LogManager.GetLogger();

        private List<MetadataField> availableFields;
        public override List<MetadataField> AvailableFields
        {
            get
            {
                if (availableFields == null)
                {
                    availableFields = GetAvailableFields();
                }

                return availableFields;
            }
        }

        private List<MetadataField> GetAvailableFields()
        {
            if (pcgwData == null)
            {
                GetPCGWMetadata();
            }

            var fields = new List<MetadataField>();
            fields.Add(MetadataField.Name);
            fields.Add(MetadataField.Links);
            fields.Add(MetadataField.ReleaseDate);
            fields.Add(MetadataField.Genres);
            fields.Add(MetadataField.Series);
            fields.Add(MetadataField.Features);
            fields.Add(MetadataField.Developers);
            fields.Add(MetadataField.Publishers);
            fields.Add(MetadataField.CriticScore);

            return fields;
        }

        private void GetPCGWMetadata()
        {
            if (pcgwData != null)
            {
                return;
            }

            if (!options.IsBackgroundDownload)
            {
                var item = plugin.PlayniteApi.Dialogs.ChooseItemWithSearch(null, (a) =>
                {
                    return client.SearchGames(a);
                }, options.GameData.Name);

                if (item != null)
                {
                    var searchItem = item as PCGWGame;
                    logger.Debug($"GetPCGWMetadata for {searchItem.Name}");
                    this.pcgwData = (PCGWGame)item;
                    this.client.FetchGamePageContent(this.pcgwData);
                }
                else
                {
                    this.pcgwData = new PCGWGame();
                    logger.Warn($"Cancelled search");
                }
            }
            else
            {
                try
                {
                    List<GenericItemOption> results = client.SearchGames(options.GameData.Name);

                    if (results.Count == 0)
                    {
                        this.pcgwData = new PCGWGame();
                        return;
                    }

                    if (results.Count > 1)
                    {
                        logger.Warn($"More than one result for {options.GameData.Name}. Using first result.");
                    }

                    this.pcgwData = (PCGWGame)results[0];
                    this.client.FetchGamePageContent(this.pcgwData);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Failed to get PCGW metadata.");
                }
            }
        }

        public PCGamingWikiMetadataProvider(MetadataRequestOptions options, PCGamingWikiMetadata plugin)
        {
            this.options = options;
            this.plugin = plugin;
            this.client = new PCGWClient();
        }

        public override string GetName(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.Name))
            {
                return this.pcgwData.Name;
            }

            return base.GetName(args);
        }


        public override IEnumerable<Link> GetLinks(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.Links))
            {
                return this.pcgwData.Links;
            }

            return base.GetLinks(args);
        }

        public override ReleaseDate? GetReleaseDate(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.ReleaseDate))
            {
                return this.pcgwData.WindowsReleaseDate();
            }

            return base.GetReleaseDate(args);
        }

        public override IEnumerable<MetadataProperty> GetGenres(GetMetadataFieldArgs args)
        {

            if (AvailableFields.Contains(MetadataField.Genres))
            {
                return this.pcgwData.Genres;
            }

            return base.GetGenres(args);
        }

        public override IEnumerable<MetadataProperty> GetFeatures(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.Features))
            {
                return this.pcgwData.Features;
            }

            return base.GetFeatures(args);
        }

        public override IEnumerable<MetadataProperty> GetSeries(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.Series))
            {
                return this.pcgwData.Series;
            }

            return base.GetSeries(args);
        }

        public override IEnumerable<MetadataProperty> GetDevelopers(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.Developers))
            {
                return this.pcgwData.Developers;
            }

            return base.GetDevelopers(args);
        }

        public override int? GetCriticScore(GetMetadataFieldArgs args)
        {
            int? score;

            if (AvailableFields.Contains(MetadataField.CriticScore) &&
                    (this.pcgwData.GetOpenCriticReception(out score) ||
                    this.pcgwData.GetIGDBReception(out score) ||
                    this.pcgwData.GetMetacriticReception(out score))
                )
            {
                logger.Debug($"Got critic score for {this.pcgwData.Name}");
                return score;
            }

            return base.GetCriticScore(args);
        }

        public override IEnumerable<MetadataProperty> GetPublishers(GetMetadataFieldArgs args)
        {
            if (AvailableFields.Contains(MetadataField.Publishers))
            {
                return this.pcgwData.Publishers;
            }

            return base.GetPublishers(args);
        }
    }
}
