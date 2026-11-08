using System.Windows;
using Shop.ViewModels;

namespace Shop
{
    public partial class LoginWindow : Window
    {
        // Конструктор принимает ViewModel, которую создаст Ninject
        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}