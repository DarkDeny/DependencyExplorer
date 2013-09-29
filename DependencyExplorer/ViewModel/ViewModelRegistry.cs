using DependencyExplorer.Infrastructure;
using DependencyExplorer.ViewModel.Licensing;
using StructureMap.Configuration.DSL;

namespace DependencyExplorer.ViewModel
{
    public class ViewModelRegistry : Registry
    {
        public ViewModelRegistry()
        {
            For<TrialLicenseInfoViewModel>()
                .Use<TrialLicenseInfoViewModel>()
                .Ctor<TrialLicenseInfoViewModel>("licenseManager");

            For<FullLicenseInfoViewModel>()
                .Use<FullLicenseInfoViewModel>()
                .Ctor<FullLicenseInfoViewModel>("licenseManager");

            For<LicenseManager>().Use(LicenseManager.Instance);
        }
    }
}