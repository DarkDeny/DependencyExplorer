using DependencyExplorer.Infrastructure;

namespace DependencyExplorer.ViewModel
{
    public class LicenseViewModelBase : ViewModelBase
    {
        public LicenseViewModelBase(LicenseManager licenseManager)
        {
            LicenseManager = licenseManager;
        }

        private LicenseManager LicenseManager { get; set; }

        public string Licensee {
            get {
                return LicenseManager.LicenseInfo.Username;
            }
        }

        public string ValidThroughText {
            get {
                return LicenseManager.LicenseInfo.ValidThroughText;
            }
        }
    }
}