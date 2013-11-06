using DependencyExplorer.ViewModel;
using StructureMap;

namespace DependencyExplorer.Infrastructure {
    public class ContainerBootstrapper {
        public static void BootstrapStructureMap(App app) {
            ObjectFactory.Initialize(
                x => {
                    x.AddRegistry(new ViewModelRegistry());
                    x.For<IUIWindowDialogService>().Use<UIWindowDialogService>();
                    x.For<App>().Use(app);
                });
        }
    }
}
