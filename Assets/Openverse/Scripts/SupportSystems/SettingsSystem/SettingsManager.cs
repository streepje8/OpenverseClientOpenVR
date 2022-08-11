using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Openverse.SupportSystems
{

    public class SettingsManager<T>
    {
        private T settings;

        public void Load()
        {
            string fileLoc = Application.persistentDataPath + "/Openverse/UserData/UserSettings.json";
            if (File.Exists(fileLoc))
            {
                string fileContents = File.ReadAllText(fileLoc);
                settings = JsonConvert.DeserializeObject<T>(fileContents);
            }
            else
            {
                Save();
            }
        }

        public void Save()
        {
            string fileLoc = Application.persistentDataPath + "/Openverse/UserData/UserSettings.json";
            string jsonString = JsonConvert.SerializeObject(settings, Formatting.Indented);
            if (!File.Exists(fileLoc))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Openverse/UserData");
                File.Create(fileLoc).Dispose();
            }
            File.WriteAllText(fileLoc, jsonString);
        }

        public T GetSettings()
        {
            return settings;
        }

        public void ModifySettings(T newSettings)
        {
            settings = newSettings;
        }
    }
}