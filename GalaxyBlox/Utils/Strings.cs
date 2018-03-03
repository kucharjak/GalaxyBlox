using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Globalization;

namespace GalaxyBlox.Utils
{
    public static class Strings
    {
        public static string ScoreToLongString(long score)
        {
            var f = new NumberFormatInfo { NumberGroupSeparator = " " };

            return score.ToString(f);
        }

        public static string ScoreToShortString(long score, int scoreLenght, bool separateWithSpace = false)
        {
            string scoreString = score.ToString();
            if (scoreString.Length <= scoreLenght)
                return scoreString;

            var suffixesList = new List<Tuple<int, string>>()
            {
                 new Tuple<int, string>(9, "B"),
                 new Tuple<int, string>(6, "M"),
                 new Tuple<int, string>(3, "K")
            };

            foreach(var suffixItem in suffixesList)
            {
                if (scoreString.Length > suffixItem.Item1)
                {
                    var tmpScoreString = scoreString;
                    scoreString = tmpScoreString.Substring(0, scoreString.Length - suffixItem.Item1);
                    if (scoreString.Length < scoreLenght)
                    {
                        var rest = tmpScoreString.Substring(scoreString.Length, scoreLenght - scoreString.Length);
                        if (rest.Replace("0", "").Count() > 0)
                            scoreString += "." + rest;
                    }

                    if (separateWithSpace)
                        scoreString += " ";

                    scoreString += suffixItem.Item2;
                }
            }

            return scoreString;
        }
    }
}