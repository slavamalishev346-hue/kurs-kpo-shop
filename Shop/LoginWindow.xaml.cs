using System.Windows;
using Shop.ViewModels;

namespace Shop
{
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

           
            viewModel.Login = "admin";
            pwdBox.Password = "admin123";
        }
    }
}