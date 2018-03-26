using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;

namespace doudizhu.ModuleDefinition
{
    [ModuleExport(typeof(doudizhu))]
    public class doudizhu:IModule
    {
        [Import]
        public IRegionManager regionManager { private get; set; }

        public void Initialize()
        {
            RegisterViews();
        }

        private void RegisterViews()
        {
            regionManager.RegisterViewWithRegion("doudizhu", typeof(Views.doudizhu)); 
        }
    }
}
