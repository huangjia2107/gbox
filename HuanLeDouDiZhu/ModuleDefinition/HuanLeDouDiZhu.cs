using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;

namespace huanledoudizhu.ModuleDefinition
{
    [ModuleExport(typeof(huanledoudizhu))]
    public class huanledoudizhu : IModule
    {
        [Import]
        public IRegionManager regionManager { private get; set; }

        public void Initialize()
        {
            RegisterViews();
        }

        private void RegisterViews()
        {
            regionManager.RegisterViewWithRegion("huanledoudizhu", typeof(Views.huanledoudizhu));
        }
    }
}
