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
        /// <param name="s1">The first string.</param>
        /// <param name="s2">The second string.</param>
        /// <returns>The not concrete distance between two given strings.</returns>
        public int Calculate(string s1, string s2)
        {
            if (s2.Length == 0) return s1.Length;
            if (s1.Length == 0) return s2.Length;
            if (s1[0] == s2[0])
            {
                return Calculate(s1.Substring(1), s2.Substring(1));
            }
            else
            {
                return 1 + Math.Min
                    (
                        Calculate(s1.Substring(1), s2),
                        Math.Min(Calculate(s1, s2.Substring(1)), Calculate(s1.Substring(1), s2.Substring(1)))
                    );
            }
        }
    }
}
