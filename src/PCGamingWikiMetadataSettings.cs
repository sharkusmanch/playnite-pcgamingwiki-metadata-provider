﻿using Playnite.SDK;
using System.Collections.Generic;
using System.ComponentModel;

namespace PCGamingWikiMetadata
{
    public class PCGamingWikiMetadataSettings : ISettings, INotifyPropertyChanged
    {
        private readonly PCGamingWikiMetadata plugin;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool addTagPrefix = false;
        public bool AddTagPrefix { get { return addTagPrefix; } set { addTagPrefix = value; ; NotifyPropertyChanged("AddTagPrefix"); } }
        private bool importTagEngine = true;
        public bool ImportTagEngine { get { return importTagEngine; } set { importTagEngine = value; ; NotifyPropertyChanged("ImportTagEngine"); } }
        private bool importTagMonetization = false;
        public bool ImportTagMonetization { get { return importTagMonetization; } set { importTagMonetization = value; ; NotifyPropertyChanged("ImportTagMonetization"); } }
        private bool importTagMicrotransactions = false;
        public bool ImportTagMicrotransactions { get { return importTagMicrotransactions; } set { importTagMicrotransactions = value; ; NotifyPropertyChanged("ImportTagMicrotransactions"); } }
        private bool importTagPacing = true;
        public bool ImportTagPacing { get { return importTagPacing; } set { importTagPacing = value; ; NotifyPropertyChanged("ImportTagPacing"); } }
        private bool importTagPerspectives = true;
        public bool ImportTagPerspectives { get { return importTagPerspectives; } set { importTagPerspectives = value; ; NotifyPropertyChanged("ImportTagPerspectives"); } }
        private bool importTagControls = true;
        public bool ImportTagControls { get { return importTagControls; } set { importTagControls = value; ; NotifyPropertyChanged("ImportTagControls"); } }
        private bool importTagVehicles = true;
        public bool ImportTagVehicles { get { return importTagVehicles; } set { importTagVehicles = value; ; NotifyPropertyChanged("ImportTagVehicles"); } }
        private bool importTagThemes = true;
        public bool ImportTagThemes { get { return importTagThemes; } set { importTagThemes = value; ; NotifyPropertyChanged("ImportTagThemes"); } }
        private bool importTagArtStyle = true;
        public bool ImportTagArtStyle { get { return importTagArtStyle; } set { importTagArtStyle = value; ; NotifyPropertyChanged("ImportTagArtStyle"); } }
        private bool importTagNoCloudSaves = true;
        public bool ImportTagNoCloudSaves { get { return importTagNoCloudSaves; } set { importTagNoCloudSaves = value; ; NotifyPropertyChanged("ImportTagNoCloudSaves"); } }

        private bool importXboxPlayAnywhere = true;
        public bool ImportXboxPlayAnywhere { get { return importXboxPlayAnywhere; } set { importXboxPlayAnywhere = value; ; NotifyPropertyChanged("ImportXboxPlayAnywhere"); } }

        private bool importMultiplayerTypes = false;
        public bool ImportMultiplayerTypes { get { return importMultiplayerTypes; } set { importMultiplayerTypes = value; ; NotifyPropertyChanged("ImportMultiplayerTypes"); } }

        private bool importFeatureHDR = true;
        public bool ImportFeatureHDR { get { return importFeatureHDR; } set { importFeatureHDR = value; ; NotifyPropertyChanged("ImportFeatureHDR"); } }
        private bool importFeatureRayTracing = true;
        public bool ImportFeatureRayTracing { get { return importFeatureRayTracing; } set { importFeatureRayTracing = value; ; NotifyPropertyChanged("ImportFeatureRayTracing"); } }
        private bool importFeatureFramerate60 = false;
        public bool ImportFeatureFramerate60 { get { return importFeatureFramerate60; } set { importFeatureFramerate60 = value; ; NotifyPropertyChanged("ImportFeatureFramerate60"); } }
        private bool importFeatureFramerate120 = false;
        public bool ImportFeatureFramerate120 { get { return importFeatureFramerate120; } set { importFeatureFramerate120 = value; ; NotifyPropertyChanged("ImportFeatureFramerate120"); } }
        private bool importFeatureUltrawide = false;
        public bool ImportFeatureUltrawide { get { return importFeatureUltrawide; } set { importFeatureUltrawide = value; ; NotifyPropertyChanged("ImportFeatureUltrawide"); } }
        private bool importFeatureVR = false;
        public bool ImportFeatureVR { get { return importFeatureVR; } set { importFeatureVR = value; ; NotifyPropertyChanged("ImportFeatureVR"); } }

        private bool importFeatureVRHTCVive = true;
        public bool ImportFeatureVRHTCVive { get { return importFeatureVRHTCVive; } set { importFeatureVRHTCVive = value; ; NotifyPropertyChanged("ImportFeatureVRHTCVive"); } }

        private bool importFeatureVROculusRift = true;
        public bool ImportFeatureVROculusRift { get { return importFeatureVROculusRift; } set { importFeatureVROculusRift = value; ; NotifyPropertyChanged("ImportFeatureVROculusRift"); } }

        private bool importFeatureVROSVR = true;
        public bool ImportFeatureVROSVR { get { return importFeatureVROSVR; } set { importFeatureVROSVR = value; ; NotifyPropertyChanged("ImportFeatureVROSVR"); } }

        private bool importFeatureVRWMR = true;
        public bool ImportFeatureVRWMR { get { return importFeatureVRWMR; } set { importFeatureVRWMR = value; ; NotifyPropertyChanged("ImportFeatureVRWMR"); } }
        private bool importFeatureVRvorpX = false;
        public bool ImportFeatureVRvorpX { get { return importFeatureVRvorpX; } set { importFeatureVRvorpX = value; ; NotifyPropertyChanged("ImportFeatureVRvorpX"); } }

        private string tagPrefixMonetization = $"[{ResourceProvider.GetString("LOCPCGWSettingsTagPrefixMonetization")}]";
        public string TagPrefixMonetization { get { return tagPrefixMonetization; } set { tagPrefixMonetization = value; ; NotifyPropertyChanged("TagPrefixMonetization"); } }
        private string tagPrefixMicrotransactions = $"[{ResourceProvider.GetString("LOCPCGWSettingsTagPrefixMicrotransactions")}]";
        public string TagPrefixMicrotransactions { get { return tagPrefixMicrotransactions; } set { tagPrefixMicrotransactions = value; ; NotifyPropertyChanged("TagPrefixMicrotransactions"); } }
        private string tagPrefixModes = $"[{ResourceProvider.GetString("LOCPCGWSettingsTagPrefixModes")}]";
        public string TagPrefixModes { get { return tagPrefixModes; } set { tagPrefixModes = value; ; NotifyPropertyChanged("TagPrefixModes"); } }
        private string tagPrefixPacing = $"[{ResourceProvider.GetString("LOCPCGWSettingsTagPrefixPacing")}]";
        public string TagPrefixPacing { get { return tagPrefixPacing; } set { tagPrefixPacing = value; ; NotifyPropertyChanged("TagPrefixPacing"); } }
        private string tagPrefixPerspectives = $"[{ResourceProvider.GetString("LOCPCGWSettingsTagPrefixPerspectives")}]";
        public string TagPrefixPerspectives { get { return tagPrefixPerspectives; } set { tagPrefixPerspectives = value; ; NotifyPropertyChanged("TagPrefixPerspectives"); } }
        private string tagPrefixControls = $"[{ResourceProvider.GetString("LOCPCGWSettingsTagPrefixControls")}]";
        public string TagPrefixControls { get { return tagPrefixControls; } set { tagPrefixControls = value; ; NotifyPropertyChanged("TagPrefixControls"); } }
        private string tagPrefixGenres = $"[{ResourceProvider.GetString("LOCPCGWSettingsTagPrefixGenres")}]";
        public string TagPrefixGenres { get { return tagPrefixGenres; } set { tagPrefixGenres = value; ; NotifyPropertyChanged("TagPrefixGenres"); } }
        private string tagPrefixVehicles = $"[{ResourceProvider.GetString("LOCPCGWSettingsTagPrefixVehicles")}]";
        public string TagPrefixVehicles { get { return tagPrefixVehicles; } set { tagPrefixVehicles = value; ; NotifyPropertyChanged("TagPrefixVehicles"); } }
        private string tagPrefixThemes = $"[{ResourceProvider.GetString("LOCPCGWSettingsTagPrefixThemes")}]";
        public string TagPrefixThemes { get { return tagPrefixThemes; } set { tagPrefixThemes = value; ; NotifyPropertyChanged("TagPrefixThemes"); } }
        private string tagPrefixEngines = $"[{ResourceProvider.GetString("LOCPCGWSettingsTagPrefixEngines")}]";
        public string TagPrefixEngines { get { return tagPrefixEngines; } set { tagPrefixEngines = value; ; NotifyPropertyChanged("TagPrefixEngines"); } }
        private string tagPrefixSeries = $"[{ResourceProvider.GetString("LOCPCGWSettingsTagPrefixSeries")}]";
        public string TagPrefixSeries { get { return tagPrefixSeries; } set { tagPrefixSeries = value; ; NotifyPropertyChanged("TagPrefixSeries"); } }
        private string tagPrefixArtStyles = $"[{ResourceProvider.GetString("LOCPCGWSettingsTagPrefixArtStyles")}]";
        public string TagPrefixArtStyles { get { return tagPrefixArtStyles; } set { tagPrefixArtStyles = value; ; NotifyPropertyChanged("TagPrefixArtStyles"); } }

        // Parameterless constructor must exist if you want to use LoadPluginSettings method.
        public PCGamingWikiMetadataSettings()
        {
        }

        public PCGamingWikiMetadataSettings(PCGamingWikiMetadata plugin)
        {
            // Injecting your plugin instance is required for Save/Load method because Playnite saves data to a location based on what plugin requested the operation.
            this.plugin = plugin;

            // Load saved settings.
            var savedSettings = plugin.LoadPluginSettings<PCGamingWikiMetadataSettings>();

            // LoadPluginSettings returns null if not saved data is available.
            if (savedSettings != null)
            {
                ImportXboxPlayAnywhere = savedSettings.ImportXboxPlayAnywhere;
                ImportMultiplayerTypes = savedSettings.ImportMultiplayerTypes;

                AddTagPrefix = savedSettings.AddTagPrefix;
                ImportTagEngine = savedSettings.ImportTagEngine;
                ImportTagMonetization = savedSettings.ImportTagMonetization;
                ImportTagMicrotransactions = savedSettings.ImportTagMicrotransactions;
                ImportTagPacing = savedSettings.ImportTagPacing;
                ImportTagPerspectives = savedSettings.ImportTagPerspectives;
                ImportTagControls = savedSettings.ImportTagControls;
                ImportTagVehicles = savedSettings.ImportTagVehicles;
                ImportTagThemes = savedSettings.ImportTagThemes;
                ImportTagArtStyle = savedSettings.ImportTagArtStyle;
                ImportTagNoCloudSaves = savedSettings.ImportTagNoCloudSaves;

                ImportFeatureHDR = savedSettings.ImportFeatureHDR;
                ImportFeatureRayTracing = savedSettings.ImportFeatureRayTracing;
                ImportFeatureFramerate120 = savedSettings.ImportFeatureFramerate120;
                ImportFeatureFramerate60 = savedSettings.ImportFeatureFramerate60;
                ImportFeatureUltrawide = savedSettings.ImportFeatureUltrawide;

                ImportFeatureVR = savedSettings.importFeatureVR;
                ImportFeatureVRHTCVive = savedSettings.importFeatureVRHTCVive;
                ImportFeatureVROculusRift = savedSettings.importFeatureVROculusRift;
                ImportFeatureVROSVR = savedSettings.importFeatureVROSVR;
                ImportFeatureVRWMR = savedSettings.importFeatureVRWMR;

                TagPrefixMonetization = savedSettings.tagPrefixMonetization;
                TagPrefixMicrotransactions = savedSettings.tagPrefixMicrotransactions;
                TagPrefixPacing = savedSettings.tagPrefixPacing;
                TagPrefixPerspectives = savedSettings.tagPrefixPerspectives;
                TagPrefixControls = savedSettings.tagPrefixControls;
                TagPrefixVehicles = savedSettings.tagPrefixVehicles;
                TagPrefixThemes = savedSettings.tagPrefixThemes;
                TagPrefixEngines = savedSettings.tagPrefixEngines;
                TagPrefixArtStyles = savedSettings.tagPrefixArtStyles;
            }
        }

        public void BeginEdit()
        {
            // Code executed when settings view is opened and user starts editing values.
        }

        public void CancelEdit()
        {
            // Code executed when user decides to cancel any changes made since BeginEdit was called.
            // This method should revert any changes made to Option1 and Option2.
        }

        public void EndEdit()
        {
            // Code executed when user decides to confirm changes made since BeginEdit was called.
            // This method should save settings made to Option1 and Option2.
            plugin.SavePluginSettings(this);
        }

        public bool VerifySettings(out List<string> errors)
        {
            // Code execute when user decides to confirm changes made since BeginEdit was called.
            // Executed before EndEdit is called and EndEdit is not called if false is returned.
            // List of errors is presented to user if verification fails.
            errors = new List<string>();
            return true;
        }
    }
}
