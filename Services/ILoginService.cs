using LaboratoryAppMVVM.Stores;
using LaboratoryAppMVVM.ViewModels;
using System;

namespace LaboratoryAppMVVM.Services
{
    public interface ILoginService<TUser, TNavigationStore>
        where TNavigationStore : ViewModelNavigationStore
    {
        /// <summary>
        /// Gets the user type 
        /// and returns a view model with the navigation
        /// depending on the given user type.
        /// </summary>
        /// <param name="user">The user to identify 
        /// the view model to return.</param>
        /// <param name="navigationStore">The navigation store 
        /// for a new view model 
        /// depending on the user's role.</param>
        /// <returns>The function 
        /// to create a view model 
        /// with the navigation.</returns>
        Func<ViewModelBase> LoginInAndGetLoginType(TUser user,
                                                   TNavigationStore navigationStore);
    }
}
