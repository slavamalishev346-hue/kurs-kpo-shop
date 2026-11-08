using System.Windows;
using Ninject;
using Shop.DI;

namespace Shop
{
    public partial class App : Application
    {
      
        public static IKernel Kernel { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //  ядро Ninject
            Kernel = new StandardKernel();

            
            Kernel.Load(new DalModule());
            Kernel.Load(new BllModule());

            
            var loginWindow = Kernel.Get<LoginWindow>();
            loginWindow.Show();
        }
    }
}