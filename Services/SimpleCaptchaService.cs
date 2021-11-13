using LaboratoryAppMVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LaboratoryAppMVVM.Services
{
    public class SimpleCaptchaService : ICaptchaService
    {
        private readonly Random _random;
        private List<ListViewCaptchaLetter> _captchasLettersList;

        public SimpleCaptchaService()
        {
            _random = new Random();
        }

        public IEnumerable<CaptchaLetterBase> GetCaptchaList(int minLetters, int maxLetters)
        {
            int lettersCount = _random.Next(minLetters, maxLetters + 1);
            List<char> characterList = new List<char>();
            _captchasLettersList = new List<ListViewCaptchaLetter>();

            for (int i = 0; i < 127; i++)
            {
                if (char.IsDigit((char)i) || char.IsLetter((char)i))
                {
                    characterList.Add((char)i);
                }
            }

            for (int i = 0; i < lettersCount; i++)
            {
                _captchasLettersList.Add(new ListViewCaptchaLetter
                {
                    Letter = Convert.ToString(characterList.ElementAt(_random.Next(0, characterList.Count))),
                    FontSize = 20,
                });
            }

            return _captchasLettersList;
        }
    }
}
