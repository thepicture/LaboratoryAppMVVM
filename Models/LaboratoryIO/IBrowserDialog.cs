namespace LaboratoryAppMVVM.Models.LaboratoryIO
{
    /// <summary>
    /// Defines a method to show a dialog 
    /// which allows an user to interact with it to
    /// select an item, which can be used later.
    /// </summary>
    public interface IBrowserDialog
    {
        /// <summary>
        /// Shows a dialog to select an item.
        /// </summary>
        /// <returns>Returns true if actions done in the dialog 
        /// led to the successful selection of an item. 
        /// Otherwise returns false.</returns>
        bool ShowDialog();
        /// <summary>
        /// Gets the selected item in the dialog. 
        /// It can be a path, file or another item.
        /// </summary>
        /// <returns>The selected item in the dialog, otherwise 
        /// null.</returns>
        object GetSelectedItem();
    }
}
