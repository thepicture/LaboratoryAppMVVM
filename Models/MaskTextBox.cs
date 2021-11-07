using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace LaboratoryAppMVVM.Models
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
