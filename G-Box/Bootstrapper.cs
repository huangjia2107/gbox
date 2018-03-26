
using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism.MefExtensions;
using System.ComponentModel.Composition.Hosting;

namespace G_Box
{
    class Bootstrapper : MefBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return this.Container.GetExportedValue<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();

            App.Current.MainWindow = (Window)this.Shell;
            PreWindow win = ServiceLocator.Current.GetInstance(typeof(PreWindow)) as PreWindow;
            win.Show();
        }


        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));

            DirectoryCatalog catalog = new DirectoryCatalog("Modules");
            this.AggregateCatalog.Catalogs.Add(catalog);
        }

        protected override IModuleCatalog CreateModuleCatalog()
        { 
            return new ConfigurationModuleCatalog();
        }
    }
}
