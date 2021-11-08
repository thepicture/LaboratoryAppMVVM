using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Stores;
using LaboratoryAppMVVM.ViewModels;
using System;

namespace LaboratoryAppMVVM.Services
{
    public class LaboratoryLoginService : ILoginService<User, ViewModelNavigationStore>
    {
        public Func<ViewModelBase> LoginInAndGetLoginType(User user,
                                                          ViewModelNavigationStore navigationStore)
        {
            switch (user.TypeOfUser.Name)
            {
                case "Лаборант":
                    return new Func<ViewModelBase>(() => new LaboratoryAssistantViewModel(navigationStore, user));
                case "Лаборант-исследователь":
                    return new Func<ViewModelBase>(() => new LaboratoryResearcherViewModel(navigationStore, user));
                case "Бухгалтер":
                    return new Func<ViewModelBase>(() => new AccountantViewModel(navigationStore, user));
                case "Администратор":
                    return new Func<ViewModelBase>(() => new AdminViewModel(navigationStore, user));
                default:
                    return null;
            }
        }
    }
}
