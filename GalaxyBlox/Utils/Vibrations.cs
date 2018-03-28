using Android.App;
using Android.Content;
using Android.OS;

namespace GalaxyBlox.Utils
{
    public static class Vibrations
    {
        private static Vibrator Vibrator;

        private static void InitializeVibrator()
        {
            Vibrator = (Vibrator)Game1.Activity.GetSystemService(Context.VibratorService);
        }
        
        public static void Vibrate(long milliseconds)
        {
            if (Vibrator == null)
                InitializeVibrator();

            if (Vibrator.HasVibrator && Static.Settings.Game.UserSettings.Vibration)
                Vibrator.Vibrate(milliseconds);
        }
    }
}