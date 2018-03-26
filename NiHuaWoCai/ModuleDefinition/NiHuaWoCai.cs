using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;

namespace nihuawocai.ModuleDefinition
{
    [ModuleExport(typeof(nihuawocai))]
    public class nihuawocai:IModule
    {
        [Import]
        public IRegionManager regionManager { private get; set; }

        public void Initialize()
        {
            RegisterViews();
        }

        private void RegisterViews()
        {
            regionManager.RegisterViewWithRegion("nihuawocai", typeof(Views.nihuawocai));
        }
    }
}
