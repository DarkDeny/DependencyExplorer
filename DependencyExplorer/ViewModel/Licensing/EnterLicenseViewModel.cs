using DependencyExplorer.Infrastructure;
using Licensing.Model;

namespace DependencyExplorer.ViewModel.Licensing {
    public class EnterLicenseViewModel : LicenseViewModelBase {
        public EnterLicenseViewModel(LicenseManager licenseManager)
            : base(licenseManager) {
        }

        private string _LicenseContent;
        public string LicenseContent {
            get {
                return _LicenseContent;
            }
            set {
                _LicenseContent = value;
                LicenseManager.Instance.ParseLicense(_LicenseContent);
                OnPropertyChanged();
            }
        }

        public LicenseStatus LicenseStatus {
            get {
                return LicenseManager.Instance.Status;
            }
        }
    }
}
