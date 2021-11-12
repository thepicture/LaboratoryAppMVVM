using LaboratoryAppMVVM.Models;
using System.Collections.Generic;

namespace LaboratoryAppMVVM.Services
{
    public interface ICaptchaService
    {
        /// <summary>
        /// Generates the captcha and returns the string representation.
        /// </summary>
        /// <returns>The string representation of the captcha.</returns>
        IEnumerable<CaptchaLetterBase> GetCaptchaList(int minLetters, int maxLetters);

    }
}
