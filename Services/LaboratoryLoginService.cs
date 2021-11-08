using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Stores;
using LaboratoryAppMVVM.ViewModels;
using System;

namespace LaboratoryAppMVVM.Services
{
    public class LaboratoryLoginService : ILoginService<TypeOfUser, ViewModelNavigationStore>
    {
        public Func<ViewModelBase> LoginInAndGetLoginType(TypeOfUser userType,
                                                          ViewModelNavigationStore navigationStore)
        {
            switch (userType.Name)
            {
                case "Лаборант":
                    return new Func<ViewModelBase>(() => new LaboratoryAssistantViewModel(navigationStore));
                case "Лаборант-исследователь":
                    return new Func<ViewModelBase>(() => new LaboratoryResearcherViewModel(navigationStore));
                case "Бухгалтер":
                    return new Func<ViewModelBase>(() => new AccountantViewModel(navigationStore));
                case "Администратор":
                    return new Func<ViewModelBase>(() => new AdminViewModel(navigationStore));
                default:
                    return null;
            }
        }
    }
}
