using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition; 

namespace SeachModule.ModuleDefinition
{
    [ModuleExport(typeof(SeachModule))]
    public class SeachModule:IModule
    {
        [Import]
        public IRegionManager regionManager { private get; set; } 

        public void Initialize()
        {
            RegisterViews();
        }

        private void RegisterViews()
        {
            regionManager.RegisterViewWithRegion("SeachRegion", typeof(Views.SeachView));
        }
    }
}
