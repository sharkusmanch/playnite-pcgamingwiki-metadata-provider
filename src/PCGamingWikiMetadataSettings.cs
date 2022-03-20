using Newtonsoft.Json;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private bool importEngineTags = true;
        public bool ImportEngineTags { get { return importEngineTags; } set { importEngineTags = value; ; NotifyPropertyChanged("ImportEngineTags"); } }
        private bool importXboxPlayAnywhere = true;
        public bool ImportXboxPlayAnywhere { get { return importXboxPlayAnywhere; } set { importXboxPlayAnywhere = value; ; NotifyPropertyChanged("ImportXboxPlayAnywhere"); } }

        private bool importMultiplayerTypes = false;
        public bool ImportMultiplayerTypes { get { return importMultiplayerTypes; } set { importMultiplayerTypes = value; ; NotifyPropertyChanged("ImportMultiplayerTypes"); } }


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
                ImportEngineTags = savedSettings.ImportEngineTags;
                ImportXboxPlayAnywhere = savedSettings.ImportXboxPlayAnywhere;
                ImportMultiplayerTypes = savedSettings.ImportMultiplayerTypes;
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
