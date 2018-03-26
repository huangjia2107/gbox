using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition; 

namespace RankModule.ModuleDefinition
{
    [ModuleExport(typeof(RankModule))]
    public class RankModule : IModule
    {
        [Import]
        public IRegionManager regionManager { private get; set; } 

        public void Initialize()
        {
            RegisterViews();
        }

        private void RegisterViews()
        {
            regionManager.RegisterViewWithRegion("RankRegion", typeof(Views.RankView));
        }
    }
}
