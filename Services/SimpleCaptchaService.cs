using LaboratoryAppMVVM.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaboratoryAppMVVM.Services
{
    public class SimpleCaptchaService : ICaptchaService
    {
        private const int minValueOfASCIIEncoding = 48;
        private const int maxValueOfASCIIEncoding = 90 + 1;
        private const int fontSize = 20;
        private readonly Random _random;
        private List<ListViewCaptchaLetter> _captchaLetters;

        public SimpleCaptchaService()
        {
            _random = new Random();
        }

        public IEnumerable<CaptchaLetter> GetCaptchaList(int minLettersCount,
                                                             int maxLettersCount)
        {
            int lettersCount = _random.Next(minLettersCount, maxLettersCount + 1);
            List<char> characterList = new List<char>();
            _captchaLetters = new List<ListViewCaptchaLetter>();

            for (int i = minValueOfASCIIEncoding; i < maxValueOfASCIIEncoding; i++)
            {
                if (char.IsDigit((char)i) || char.IsLetter((char)i))
                {
                    characterList.Add((char)i);
                }
            }

            for (int i = 0; i < lettersCount; i++)
            {
                _captchaLetters.Add(new ListViewCaptchaLetter
                {
                    Letter = Convert.ToString
                    (
                        characterList.ElementAt(_random.Next(0, characterList.Count))
                    ),
                    FontSize = fontSize,
                });
            }

            return _captchaLetters;
        }
    }
}
