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

            //  модули
            Kernel.Load(new DalModule());
            Kernel.Load(new BllModule());

            // главное окно через Ninject
            var mainWindow = Kernel.Get<MainWindow>();
            mainWindow.Show();
        }
    }
}