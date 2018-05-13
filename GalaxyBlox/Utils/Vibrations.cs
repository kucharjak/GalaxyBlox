using Android.App;
using Android.Content;
using Android.OS;

namespace GalaxyBlox.Utils
{
    /// <summary>
    /// Class for controling vibrations on device.
    /// </summary>
    public static class Vibrations
    {
        private static Vibrator Vibrator;

        private static void InitializeVibrator()
        {
            Vibrator = (Vibrator)Game1.Activity.GetSystemService(Context.VibratorService);
        }
        
        /// <summary>
        /// Vibration method. 
        /// </summary>
        /// <param name="milliseconds">Duration of vibrations in milliseconds.</param>
        public static void Vibrate(long milliseconds)
        {
            if (Vibrator == null)
                InitializeVibrator();

            if (Vibrator.HasVibrator && Static.Settings.UserSettings.Vibration)
                Vibrator.Vibrate(milliseconds);
        }
    }
}