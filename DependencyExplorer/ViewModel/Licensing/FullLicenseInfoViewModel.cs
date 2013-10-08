using System.Windows;

using DependencyExplorer.Infrastructure;

namespace DependencyExplorer.ViewModel.Licensing
{
    public class FullLicenseInfoViewModel : LicenseViewModelBase {
        public FullLicenseInfoViewModel(LicenseManager licenseManager, IUIWindowDialogService dialogService, Window window)
            : base(licenseManager, dialogService, window) { }
    }
}