using LaboratoryAppMVVM.Stores;
using LaboratoryAppMVVM.ViewModels;
using System;

namespace LaboratoryAppMVVM.Services
{
    public interface ILoginService<TUserType, TNavigationStore>
        where TNavigationStore : ViewModelNavigationStore
    {
        /// <summary>
        /// The method must get the user type 
        /// and return the view model with the navigation
        /// depending on the given user type 
        /// for further actions.
        /// </summary>
        /// <param name="userType">The user type to identify a type to return.</param>
        /// <param name="navigationStore">The navigation store of the returning login type.</param>
        /// <returns>The function to create a view model with the navigation.</returns>
        Func<ViewModelBase> LoginInAndGetLoginType(TUserType userType,
                                                   TNavigationStore navigationStore);
    }
}
