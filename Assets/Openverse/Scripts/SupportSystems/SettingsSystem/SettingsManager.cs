//-------------------------------
//SettingsManager
//This script is responsible for handeling user settings stored in structs
//
//Author: streep
//Creation Date: 12-08-2022
//--------------------------------
namespace Openverse.SupportSystems
{
    using Newtonsoft.Json;
    using System.IO;
    using UnityEngine;

    public class SettingsManager<T>
    {
        private T settings;
        private string filename;

        public SettingsManager(string filename)
        {
            this.filename = filename;
        }

        public void Load()
        {
            string fileLoc = Application.persistentDataPath + "/Openverse/UserData/" + filename + ".json";
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
            string fileLoc = Application.persistentDataPath + "/Openverse/UserData/" + filename + ".json";
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