using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition; 

namespace AllGamesModule.ModuleDefinition
{
    [ModuleExport(typeof(AllGamesModule))] 
    public class AllGamesModule:IModule
    {
        [Import]
        public IRegionManager regionManager { private get; set; } 

        public void Initialize()
        {
            RegisterViews();
        }

        private void RegisterViews() 
        {
            regionManager.RegisterViewWithRegion("AllGamesRegion", typeof(Views.AllGamesView));
        }
    }
}
