using Android.Util;
using GalaxyBlox.Static;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace GalaxyBlox.Static
{
    public static class Settings
    {
        private static IsolatedStorageFile dataFile = IsolatedStorageFile.GetUserStoreForDomain();
        private const string dataFilePath = "settings.xml";

        public static Size GameSize = new Size(480, 800); // new Size(720, 1200);
        public static Size GameArenaSize = new Size(12, 20);  //new Size(18, 30); //new Size(36, 60); 

        public static SettingOptions.Indicator Indicator =  SettingOptions.Indicator.Shape;

        public static SettingsXml SettingsClass;

        public static void SaveHighScore(long highScore)
        {
            SettingsClass.HighScore = highScore;
            SaveSettings();
        }

        public static void LoadSettings()
        {
            try
            {
                using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(dataFilePath, FileMode.Open, iso))
                    {
                        // Read the data from the file
                        XmlSerializer serializer = new XmlSerializer(typeof(SettingsXml));
                        SettingsClass = (SettingsXml)serializer.Deserialize(stream);
                    }
                }
            }
            catch
            {
                SettingsClass = new SettingsXml();
            }
        }

        public static void SaveSettings()
        {
            using (IsolatedStorageFile iso = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(dataFilePath, FileMode.Create, iso))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(SettingsXml));
                    serializer.Serialize(stream, SettingsClass);
                }
            }
        }
    }
    
    public class SettingsXml
    {
        public long HighScore;
    }
}