using System;

namespace LaboratoryAppMVVM.Models
{
    /// <summary>
    /// Class for calculating a not concrete distance between two strings.
    /// </summary>
    public class LevenshteinDistanceCalculator : ICalculator
    {
        /// <summary>
        /// Calculates a not concrete distance between two given strings.
        /// </summary>
        /// <param name="firstStringAndSecondStringArray">
        /// A first and a second strings.
        /// </param>
        /// <returns>The not concrete distance 
        /// between two given strings as the integer.</returns>
        public object Calculate(params object[] firstStringAndSecondStringArray)
        {
            if (firstStringAndSecondStringArray == null
                || firstStringAndSecondStringArray.Length != 2)
            {
                throw new ArgumentOutOfRangeException("To calculate " +
                    "a levenshtein distance, " +
                    "method should get " +
                    "an array with exactly two given strings.");
            }
            string firstSentence = (string)firstStringAndSecondStringArray[0];
            string secondSentence = (string)firstStringAndSecondStringArray[1];
            if (secondSentence.Length == 0)
            {
                return firstSentence.Length;
            }

            if (firstSentence.Length == 0)
            {
                return secondSentence.Length;
            }

            if (firstSentence[0] == secondSentence[0])
            {
                return Calculate(new string[]{ firstSentence.Substring(1),
                                 secondSentence.Substring(1) });
            }
            else
            {
                return 1 + Math.Min
                    (
                        (int)Calculate
                        (
                            new string[] { firstSentence.Substring(1), secondSentence }
                        ),
                        Math.Min
                        (
                            (int)Calculate
                            (
                                new string[]
                                {
                                    firstSentence,
                                    secondSentence.Substring(1)
                                }
                            ), (int)Calculate
                            (
                                new string[]
                                {
                                    firstSentence.Substring(1),
                                    secondSentence.Substring(1)
                                }
                            )
                        )
                    );
            }
        }
    }
}
