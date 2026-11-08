using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ninject;
using Shop.Core;
using Shop.Interfaces.Services;

namespace Shop.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        private string _login;
        private string _password;
        private string _errorMessage;

       
        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); }
        }

    
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand CancelCommand { get; }

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        private bool CanExecuteLogin(object parameter) => !string.IsNullOrWhiteSpace(Login);

        private void ExecuteLogin(object parameter)
        {
            ErrorMessage = string.Empty;

            if (parameter is PasswordBox passwordBox)
            {
                string pwd = passwordBox.Password;
                var user = _userService.Authenticate(Login, pwd);

                if (user != null)
                {
                   
                    var mainWindow = App.Kernel.Get<MainWindow>();

                    
                    var mainViewModel = (MainWindowViewModel)mainWindow.DataContext;
                    mainViewModel.CurrentUser = user;

                    mainWindow.Show();
                    Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is LoginWindow)?.Close();
                }
                else
                {
                    ErrorMessage = "Неверный логин или пароль";
                }
            }
        }

        private void ExecuteCancel(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}