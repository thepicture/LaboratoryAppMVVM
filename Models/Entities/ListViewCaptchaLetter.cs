using System.Windows.Media;

namespace LaboratoryAppMVVM.Models.Entities
{
    public class ListViewCaptchaLetter : CaptchaLetter
    {
        public SolidColorBrush Color { get; set; }
        public int FontSize { get; set; }
    }
}
