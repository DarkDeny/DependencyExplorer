using DependencyExplorer.ViewModel;
using StructureMap;

namespace DependencyExplorer.Infrastructure {
    public class ContainerBootstrapper {
        public static void BootstrapStructureMap() {
            ObjectFactory.Initialize(
                x => {
                    x.AddRegistry(new ViewModelRegistry());
                    x.For<IUIWindowDialogService>().Use<UIWindowDialogService>();
                });
        }
    }
}
