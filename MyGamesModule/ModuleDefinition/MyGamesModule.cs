using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition; 

namespace MyGamesModule.ModuleDefinition
{
    [ModuleExport(typeof(MyGamesModule))]
    public class MyGamesModule:IModule
    {
        [Import]
        public IRegionManager regionManager { private get; set; } 

        public void Initialize()
        {
            RegisterViews();
        }

        private void RegisterViews()
        {
            regionManager.RegisterViewWithRegion("MyGamesRegion", typeof(Views.MyGamesView));
        }
    }
}
