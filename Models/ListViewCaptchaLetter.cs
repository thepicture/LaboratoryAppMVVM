using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LaboratoryAppMVVM.Models
{
    public class ListViewCaptchaLetter : CaptchaLetterBase
    {
        public SolidColorBrush Color { get; set; }
        public int FontSize { get; set; }
    }
}
