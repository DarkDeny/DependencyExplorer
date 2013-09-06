using DependencyExplorer.ViewModel;

using StructureMap;

namespace DependencyExplorer.Infrastructure
{
    public class ContainerBootstrapper
    {
        public static void BootstrapStructureMap()
        {
            ObjectFactory.Initialize(
                x =>
                {
                    x.For<IUIWindowDialogService>().Use<UIWindowDialogService>();
                    x.For<LicenseInfoViewModel>()
                        .Use<LicenseInfoViewModel>()
                        .Ctor<LicenseInfoViewModel>("licenseManager");
                    x.For<LicenseManager>().Use(LicenseManager.Instance);
                });
        }
    }
}
