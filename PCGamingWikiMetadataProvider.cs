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

            return fields;
        }

        private void GetPCGWMetadata()
        {
            if (pcgwData != null)
            {
                return;
            }

            logger.Debug("GetPCGWMetadata");

            if (!options.IsBackgroundDownload)
            {
                logger.Debug("Starting selection...");

                var item = plugin.PlayniteApi.Dialogs.ChooseItemWithSearch(null, (a) =>
                {
                    return client.SearchGames(a);
                }, options.GameData.Name);

                if (item != null)
                {
                    var searchItem = item as PCGWGame;
                    logger.Debug($"GetPCGWMetadata for {searchItem.Name}");
                    this.pcgwData = (PCGWGame)item;
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

        public override string GetName()
        {
            if (AvailableFields.Contains(MetadataField.Name))
            {
                return this.pcgwData.Name;
            }

            return base.GetName();
        }


        public override List<Link> GetLinks()
        {
            if (AvailableFields.Contains(MetadataField.Links))
            {
                return this.pcgwData.Links;
            }

            return base.GetLinks();
        }
    }
}