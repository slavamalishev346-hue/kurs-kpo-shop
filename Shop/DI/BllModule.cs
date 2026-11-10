using Ninject.Modules;
using Shop.Interfaces.Services;
using Shop.BLL.Services;
using Shop.ViewModels;

namespace Shop.DI
{
    public class BllModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISaleService>().To<SaleService>().InTransientScope();
            Bind<IProductService>().To<ProductService>().InTransientScope();
            Bind<ILoyaltyService>().To<LoyaltyService>().InTransientScope();
            Bind<IReportService>().To<ReportService>().InTransientScope();
            Bind<ISupplyService>().To<SupplyService>().InTransientScope();
            Bind<IUserService>().To<UserService>();
            Bind<MainWindowViewModel>().ToSelf().InTransientScope();
         
            Bind<LoyaltyViewModel>().ToSelf().InTransientScope();
        }
    }
}