using System.Windows.Forms;

namespace LaboratoryAppMVVM.Models.LaboratoryIO
{
    public class SimpleFolderDialog : IBrowserDialog
    {
        private FolderBrowserDialog _dialog;
        private bool _result;

        public object GetSelectedItem()
        {
            return !_result ? null : (object)_dialog.SelectedPath;
        }

        public bool ShowDialog()
        {
            _dialog = new FolderBrowserDialog();
            _result = _dialog.ShowDialog() == DialogResult.OK;
            return _result;
        }
    }
}