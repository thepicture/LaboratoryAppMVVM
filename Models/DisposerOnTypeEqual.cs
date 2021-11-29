using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;

namespace LaboratoryAppMVVM.Models
{
    /// <summary>
    /// Implements a method for disposing the current timer 
    /// if a type equals 
    /// to the given navigation store type.
    /// </summary>
    /// <typeparam name="T">A type of the navigation store.</typeparam>
    public class DisposerOnTypeEqual<T>
    {
        /// <summary>
        /// Disposes the current timer 
        /// if a type equals to the given navigation store type.
        /// </summary>
        /// <param name="haveTimeService">A time service.</param>
        /// <param name="type">A type of the navigation store.</param>
        /// <param name="navigationStore">A navigation store.</param>
        public static void Dispose(HaveTimeServiceBase haveTimeService, ViewModelNavigationStore navigationStore)
        {
            if (navigationStore.CurrentViewModel.GetType() == typeof(T))
            {
                haveTimeService.Stop();
            }
        }
    }
}
