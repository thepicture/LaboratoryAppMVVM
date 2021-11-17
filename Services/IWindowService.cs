namespace LaboratoryAppMVVM.Services
{
    /// <summary>
    /// A service for providing new windows.
    /// </summary>
    public interface IWindowService
    {
        /// <summary>
        /// Shows the window with respect to the given viewmodel.
        /// </summary>
        /// <param name="viewModel">The viewmodel to use in a shown window.</param>
        void ShowWindow(object viewModel);
    }
}
