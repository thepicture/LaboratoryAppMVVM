using LaboratoryAppMVVM.Stores;
using LaboratoryAppMVVM.ViewModels;
using System;

namespace LaboratoryAppMVVM.Services
{
    public interface ILoginService<TUser, TNavigationStore>
        where TNavigationStore : ViewModelNavigationStore
    {
        /// <summary>
        /// The method must get the user type 
        /// and return the view model with the navigation
        /// depending on the given user type 
        /// for further actions.
        /// </summary>
        /// <param name="user">The user to identify the view model to return.</param>
        /// <param name="navigationStore">The navigation store of the returning login type.</param>
        /// <returns>The function to create a view model with the navigation.</returns>
        Func<ViewModelBase> LoginInAndGetLoginType(TUser user,
                                                   TNavigationStore navigationStore);
    }
}
