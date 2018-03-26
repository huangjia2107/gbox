using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions; 
using System.ComponentModel.Composition;

namespace IntroductionModule.ModuleDefinition
{
    [ModuleExport(typeof(IntroductionModule))]
    public class IntroductionModule :IModule
    {
        [Import]
        public IRegionManager regionManager { private get; set; } 


        public void Initialize()
        {
            RegisterViews();
        }

        private void RegisterViews()
        {
            regionManager.RegisterViewWithRegion("IntroductionRegion", typeof(Views.IntroductionView));
        }
    }
}
