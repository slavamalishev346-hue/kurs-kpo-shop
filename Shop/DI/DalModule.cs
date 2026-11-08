using Ninject.Modules;
using Microsoft.EntityFrameworkCore;
using Shop.DAL;
using Shop.DAL.UnitOfWork;
using Shop.Interfaces.UnitOfWork;

namespace Shop.DI
{
    public class DalModule : NinjectModule
    {
        public override void Load()
        {
        
            Bind<MagazContext>().ToMethod(ctx =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<MagazContext>();
               
                optionsBuilder.UseNpgsql("Host=localhost;Database=Magaz;Username=postgres;Password=3662");
                return new MagazContext(optionsBuilder.Options);
            }).InTransientScope();

            Bind<IUnitOfWork>().To<UnitOfWork>().InTransientScope();
        }
    }
}