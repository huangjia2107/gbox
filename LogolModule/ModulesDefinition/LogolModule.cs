
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition; 

namespace LogolModule
{
    [ModuleExport(typeof(LogolModule))]
    public class LogolModule : IModule
    {
        [Import]
        public IRegionManager regionManager { private get; set; } 

        public void Initialize()
        {
            RegisterViews();           
        }

        private void RegisterViews()
        {
            regionManager.RegisterViewWithRegion("LogolRegion", typeof(Views.LogolView));
        }
    }
}
