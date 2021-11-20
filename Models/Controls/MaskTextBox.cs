using System.Linq;
using System.Windows.Controls;

namespace LaboratoryAppMVVM.Models.Controls
{
    public class MaskTextBox : TextBox
    {
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            Text = string.Concat(Enumerable.Repeat("*", Text.Length));
            base.OnTextChanged(e);
        }
    }
}
