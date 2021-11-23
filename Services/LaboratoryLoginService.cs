using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Stores;
using LaboratoryAppMVVM.ViewModels;
using System;

namespace LaboratoryAppMVVM.Services
{
    public class LaboratoryLoginService : ILoginService<User, ViewModelNavigationStore>
    {
        public Func<ViewModelBase> LoginInAndGetLoginType(
            User user,
            ViewModelNavigationStore navigationStore)
        {
            switch (user.TypeOfUser.Name)
            {
                case "Лаборант":
                    return new Func<ViewModelBase>(() =>
                    {
                        return new LaboratoryAssistantViewModel(navigationStore, user);
                    });
                case "Лаборант-исследователь":
                    return new Func<ViewModelBase>(() =>
                    {
                        return new LaboratoryResearcherViewModel(navigationStore,
                            user,
                            new LaboratoryWindowService());
                    });
                case "Бухгалтер":
                    return new Func<ViewModelBase>(() =>
                    {
                        return new AccountantViewModel(user);
                    });
                case "Администратор":
                    return new Func<ViewModelBase>(() =>
                    {
                        return new AdminViewModel(navigationStore, user);
                    });
                default:
                    return null;
            }
        }
    }
}
