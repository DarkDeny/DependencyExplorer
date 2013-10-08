using System.Windows;

using DependencyExplorer.Infrastructure;

namespace DependencyExplorer.ViewModel.Licensing
{
    public class LicenseViewModelBase : WindowViewModelBase {
        public LicenseViewModelBase(LicenseManager licenseManager, IUIWindowDialogService dialogService, Window window)
            : base (dialogService, window) {
            LicenseManager = licenseManager;
        }

        protected LicenseManager LicenseManager { get; set; }

        public string ValidThroughText {
            get {
                return LicenseManager.LicenseInfo.ValidThroughText;
            }
        }
    }
}