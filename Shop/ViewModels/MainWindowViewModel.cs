using System.Windows;
using System.Windows.Input;
using Ninject;
using Shop.Core;
using Shop.Domain.Entities;
using Shop.Interfaces.Services;
using Shop.Views;

namespace Shop.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private User _currentUser;
        private object _currentView;

        public User CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
               
                OnPropertyChanged(nameof(CanManageProducts));
                OnPropertyChanged(nameof(CanManageSupplies));
                OnPropertyChanged(nameof(CanManageLoyalty));
                OnPropertyChanged(nameof(CanViewReports));
                OnPropertyChanged(nameof(UserInfo));
            }
        }

        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

       
        public string UserInfo => $"{CurrentUser?.FullName} ({CurrentUser?.Role?.Name})";

        
        public ICommand NavigateCommand { get; }

     
        public ICommand LogoutCommand { get; }

        
        public Visibility CanManageProducts => IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        public Visibility CanManageSupplies => IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        public Visibility CanManageLoyalty => IsAdmin ? Visibility.Visible : Visibility.Collapsed;
        public Visibility CanViewReports => IsAdmin ? Visibility.Visible : Visibility.Collapsed;

        private bool IsAdmin => CurrentUser?.Role?.Name?.ToLower() == "admin";

        public MainWindowViewModel()
        {
            NavigateCommand = new RelayCommand(Navigate);
            LogoutCommand = new RelayCommand(Logout);

           
            Navigate("Sale");
        }

        private void Navigate(object parameter)
        {
            if (parameter is string viewName)
            {
                switch (viewName)
                {
                    
                    case "Sale":
                       
                        int cashierId = CurrentUser?.Id ?? 0;

                        var saleViewModel = new SaleViewModel(
                            App.Kernel.Get<ISaleService>(),
                            App.Kernel.Get<IProductService>(),
                            cashierId
                        );

                        var saleView = new SaleView();
                        saleView.DataContext = saleViewModel;
                        CurrentView = saleView;
                        break;
                    case "Products":
                        var productsViewModel = App.Kernel.Get<ProductsViewModel>();
                        var productsView = new ProductsView();
                        productsView.DataContext = productsViewModel;
                        CurrentView = productsView;
                        break;
                    case "Supplies":
                        CurrentView = new SuppliesView();
                        break;
                    case "Loyalty":
                       
                        var loyaltyViewModel = App.Kernel.Get<LoyaltyViewModel>();
                        var loyaltyView = new LoyaltyView();
                        loyaltyView.DataContext = loyaltyViewModel;
                        CurrentView = loyaltyView;
                        break;
                    case "Reports":
                        CurrentView = new ReportsView();
                        break;
              
                }
            }
        }

        private void Logout(object parameter)
        {
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is MainWindow)?.Close();
            var loginWindow = App.Kernel.Get<LoginWindow>();
            loginWindow.Show();
        }
    }
}