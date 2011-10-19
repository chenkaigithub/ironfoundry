﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using CloudFoundry.Net.Types;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using GalaSoft.MvvmLight.Messaging;
using CloudFoundry.Net.VsExtension.Ui.Controls.Utilities;

namespace CloudFoundry.Net.VsExtension.Ui.Controls.Model
{
    public class PreferencesProvider : IPreferencesProvider
    {
        private readonly string preferencesPath;
        private const string PreferencesFileName = "preferences.bin";

        public PreferencesProvider(string preferencesPath)
        {
            this.preferencesPath = preferencesPath;            
        }
        
        public Preferences Load()
        {
            var preferences = new Preferences()
            {
                Clouds = new ObservableCollection<Cloud>(),
                CloudUrls = CloudUrl.DefaultCloudUrls,
            };

            try
            {
                var fullPath = this.preferencesPath + "/" + PreferencesFileName;
                IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);                
                if (isoStore.DirectoryExists(this.preferencesPath) && isoStore.FileExists(fullPath))
                {
                    using (IsolatedStorageFileStream configStream = isoStore.OpenFile(fullPath, FileMode.Open))
                    {
                        var formatter = new BinaryFormatter();
                        preferences = formatter.Deserialize(configStream) as Preferences;
                    }
                }
            }
            catch (Exception)
            {
                // If preferences fail to load, swallow the exception.
            }
            return preferences;
        }        

        public void Save(Preferences preferences)
        {           
            var isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null);
            IsolatedStorageFileStream configStream;
            
            if (!isoStore.DirectoryExists(this.preferencesPath))
                isoStore.CreateDirectory(this.preferencesPath);

            var fullPath = this.preferencesPath + "/" + PreferencesFileName;
            if (!isoStore.FileExists(fullPath))
                configStream = isoStore.CreateFile(fullPath);
            else
                configStream = isoStore.OpenFile(fullPath, FileMode.Open);
            var formatter = new BinaryFormatter();            
            formatter.Serialize(configStream, preferences);            
            configStream.Flush();
            configStream.Close();
        }
    }
}
