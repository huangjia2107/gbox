using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition; 

namespace CardModule.ModuleDefinition
{
    [ModuleExport(typeof(CardModule))]
    public class CardModule:IModule
    {
        [Import]
        public IRegionManager regionManager { private get; set; } 

        public void Initialize()
        {
            RegisterViews();
        }

        private void RegisterViews()
        {
            regionManager.RegisterViewWithRegion("CardRegion", typeof(Views.CardView));
        }
    }
}
