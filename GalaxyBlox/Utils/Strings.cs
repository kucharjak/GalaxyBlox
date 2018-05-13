using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Globalization;

namespace GalaxyBlox.Utils
{
    /// <summary>
    /// Class with methods for string handling. 
    /// </summary>
    public static class Strings
    {
        /// <summary>
        /// Takes numeric score and returns it's value in string without number separator.
        /// </summary>
        /// <param name="score">Score value</param>
        /// <returns></returns>
        public static string ScoreToLongString(long score)
        {
            var f = new NumberFormatInfo { NumberGroupSeparator = " " };
            return score.ToString(f);
        }

        /// <summary>
        /// Takes numeric score and returns it's value in string with set lenght and appropriate suffixe.
        /// </summary>
        /// <param name="score">Score value</param>
        /// <param name="scoreLength">Wanted score length</param>
        /// <param name="separateWithSpace"></param>
        /// <returns></returns>
        public static string ScoreToShortString(long score, int scoreLength, bool separateWithSpace = false)
        {
            string scoreString = score.ToString();
            if (scoreString.Length <= scoreLength)
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
                    if (scoreString.Length < scoreLength)
                    {
                        var rest = tmpScoreString.Substring(scoreString.Length, scoreLength - scoreString.Length);
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