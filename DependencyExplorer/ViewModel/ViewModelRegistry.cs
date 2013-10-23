using DependencyExplorer.Infrastructure;
using DependencyExplorer.ViewModel.Licensing;
using StructureMap.Configuration.DSL;

namespace DependencyExplorer.ViewModel
{
    public class ViewModelRegistry : Registry
    {
        public ViewModelRegistry()
        {
            For<LicenseInfoViewModel>()
                .Use<LicenseInfoViewModel>()
                .Ctor<LicenseInfoViewModel>("licenseManager");

            For<LicenseManager>().Use(LicenseManager.GetInstance());
        }
    }
}