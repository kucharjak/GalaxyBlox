using Android.Util;
using GalaxyBlox.Static;

namespace GalaxyBlox.Static
{
    public static class Settings
    {
        public static Size GameSize = new Size(480, 800); // new Size(720, 1200);
        public static Size GameArenaSize = new Size(18, 30);  //new Size(12, 20);
        
        public static SettingOptions.Indicator Indicator =  SettingOptions.Indicator.Shape;
    }
}