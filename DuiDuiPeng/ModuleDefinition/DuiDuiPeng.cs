using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition; 

namespace duiduipeng.ModuleDefinition
{
    [ModuleExport(typeof(duiduipeng))]
    public class duiduipeng:IModule
    {
        [Import]
        public IRegionManager regionManager { private get; set; }

        public void Initialize()
        {
            RegisterViews();
        }

        private void RegisterViews()
        {
            regionManager.RegisterViewWithRegion("duiduipeng", typeof(Views.duiduipeng));
        }
    }
}
