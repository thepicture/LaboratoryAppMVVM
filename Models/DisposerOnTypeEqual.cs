using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;

namespace LaboratoryAppMVVM.Models
{
    /// <summary>
    /// Implements method for disposing the current timer 
    /// if the type equals 
    /// to the given navigation store type.
    /// </summary>
    /// <typeparam name="T">The type of a navigation store.</typeparam>
    public class DisposerOnTypeEqual<T>
    {
        /// <summary>
        /// Disposes the current timer if the type equals to the given navigation store type.
        /// Used, when a view model must not have a timer.
        /// </summary>
        /// <param name="haveTimeService">A time service.</param>
        /// <param name="type">The type of a navigation store.</param>
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
