using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace GalaxyBlox.Utils
{
    public static class Strings
    {
        public static string ScoreToString(long score, int scoreLenght)
        {
            string scoreString = score.ToString();
            if (scoreString.Length <= scoreLenght)
                return scoreString;

            var suffix = "";
            if (score >= 1000000000)
            {
                suffix = "B";
                scoreString = ((int)(score / 1000000000)).ToString();
                if (scoreString.Length < scoreLenght)
                    scoreString += "." + ((int)(score % 1000000000)).ToString().Substring(0, scoreLenght - scoreString.Length);
            }
            else if (score >= 1000000)
            {
                suffix = "M";
                scoreString = ((int)(score / 1000000)).ToString();
                if (scoreString.Length < scoreLenght)
                    scoreString += "." + ((int)(score % 1000000)).ToString().Substring(0, scoreLenght - scoreString.Length);
            }
            else if (score >= 1000)
            {
                suffix = "K";
                scoreString = ((int)(score / 1000)).ToString();
                if (scoreString.Length < scoreLenght)
                    scoreString += "." + ((int)(score % 1000)).ToString().Substring(0, scoreLenght - scoreString.Length);
            }
            scoreString += suffix;

            return scoreString;
        }
    }
}