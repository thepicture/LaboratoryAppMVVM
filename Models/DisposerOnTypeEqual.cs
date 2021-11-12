using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;

namespace LaboratoryAppMVVM.Models
{
    public class DisposerOnTypeEqual<T>
    {
        /// <summary>
        /// Disposes of the current timer if the type equals to navigation store type.
        /// Mostly used in a view model logic, when
        /// changing of view model requires stopping
        /// of services.
        /// </summary>
        /// <param name="haveTimeService">A time service.</param>
        /// <param name="type">The type.</param>
        /// <param name="navigationStore">The navigation store.</param>
        public static void Dispose(IHaveTimeService haveTimeService, ViewModelNavigationStore navigationStore)
        {
            if (navigationStore.CurrentViewModel.GetType() == typeof(T))
            {
                haveTimeService.Stop();
            }
        }
    }
}
