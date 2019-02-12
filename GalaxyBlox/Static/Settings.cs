using System;
using System.IO.IsolatedStorage;
using Microsoft.Xna.Framework;
using static GalaxyBlox.Static.SettingClasses;

namespace GalaxyBlox.Static
{
    /// <summary>
    /// Vital settings of the game.
    /// </summary>
    public static class Settings
    {
        public static readonly bool ShowFPS = false;

        private const string settingsPath = "settings.xml";
        private const string highscoresPath = "highscores.xml";
        public const int MaxHighscoresPerGameMod = 5;
        public const bool UseLastHighscoreName = false;

        public static readonly Vector2 WindowSize = new Vector2(720, 1208);

        public static Vector2 ArenaSize = new Vector2(12, 20);

        public static UserSettings UserSettings;
        public static Highscores Highscores;

        public static void LoadAll()
        {
            LoadUserSettings();
            LoadHighscores();
        }

        public static void SaveAll()
        {
            SaveUserSettings();
            SaveHighscores();
        }

        public static void LoadUserSettings()
        {
            Utils.XmlIsoStore.TryDeserialize(out UserSettings, settingsPath);
        }

        public static void SaveUserSettings()
        {
            Utils.XmlIsoStore.Serialize(UserSettings, settingsPath);
        }

        public static void LoadHighscores()
        {
            Utils.XmlIsoStore.TryDeserialize(out Highscores, highscoresPath);
        }

        public static void SaveHighscores()
        {
            Utils.XmlIsoStore.Serialize(Highscores, highscoresPath);
        }
    }
}