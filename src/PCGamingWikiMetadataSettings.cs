using Playnite.SDK;
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

        private bool importTagEngine = true;
        public bool ImportTagEngine { get { return importTagEngine; } set { importTagEngine = value; ; NotifyPropertyChanged("ImportTagEngine"); } }
        private bool importTagMonetization = true;
        public bool ImportTagMonetization { get { return importTagMonetization; } set { importTagMonetization = value; ; NotifyPropertyChanged("ImportTagMonetization"); } }
        private bool importTagMicrotransactions = true;
        public bool ImportTagMicrotransactions { get { return importTagMicrotransactions; } set { importTagMicrotransactions = value; ; NotifyPropertyChanged("ImportTagMicrotransactions"); } }        private bool importTagPacing = true;
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
