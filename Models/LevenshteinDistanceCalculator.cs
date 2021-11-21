using System;

namespace LaboratoryAppMVVM.Models
{
    /// <summary>
    /// Class for calculating a not concrete distance between two strings.
    /// </summary>
    public class LevenshteinDistanceCalculator
    {
        /// <summary>
        /// Calculates a not concrete distance between two given strings.
        /// </summary>
        /// <param name="firstString">The first string.</param>
        /// <param name="secondString">The second string.</param>
        /// <returns>The not concrete distance between two given strings.</returns>
        public int Calculate(string firstString, string secondString)
        {
            if (secondString.Length == 0) return firstString.Length;
            if (firstString.Length == 0) return secondString.Length;
            if (firstString[0] == secondString[0])
            {
                return Calculate(firstString.Substring(1), secondString.Substring(1));
            }
            else
            {
                return 1 + Math.Min
                    (
                        Calculate(firstString.Substring(1), secondString),
                        Math.Min(Calculate(firstString,
                                           secondString.Substring(1)), Calculate(
                                               firstString.Substring(1),
                                               secondString.Substring(1)))
                    );
            }
        }
    }
}
