using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GalaxyBlox.Utils
{
    /// <summary>
    /// Methods for handling colors.
    /// </summary>
    public static class Colors
    {
        /// <summary>
        /// Method that mix colors from list to one color by calculating average of all color channels. 
        /// </summary>
        /// <param name="colors">List of colors to mix</param>
        /// <returns></returns>
        public static Color MixMultipleColors(List<Color> colors)
        {
            int r = 0, g = 0, b = 0;

            foreach(var col in colors)
            {
                r += col.R / colors.Count;
                g += col.G / colors.Count;
                b += col.B / colors.Count;
            }

            return new Color(r, g, b);
        }
    }
}