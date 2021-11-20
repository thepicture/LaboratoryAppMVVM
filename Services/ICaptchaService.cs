using LaboratoryAppMVVM.Models.Entities;
using System.Collections.Generic;

namespace LaboratoryAppMVVM.Services
{
    public interface ICaptchaService
    {
        /// <summary>
        /// Generates the captcha and returns the list of captcha letters.
        /// </summary>
        /// <returns>The list of captcha letters.</returns>
        IEnumerable<CaptchaLetterBase> GetCaptchaList(int minLetters, int maxLetters);

    }
}
