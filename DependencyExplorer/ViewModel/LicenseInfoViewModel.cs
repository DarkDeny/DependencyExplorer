using System.Windows;

using DependencyExplorer.Infrastructure;

using Licensing.Model;

namespace DependencyExplorer.ViewModel
{
    public class LicenseInfoViewModel : ViewModelBase
    {
        public LicenseInfoViewModel(LicenseManager licenseManager)
        {
            LicenseManager = licenseManager;
        }

        private LicenseManager LicenseManager { get; set; }

        public string Licensee
        {
            get
            {
                return LicenseManager.LicenseInfo.Username;
            }
        }

        public string LicenseText
        {
            get
            {
                return LicenseManager.LicenseInfo.ToString();
            }
        }

        public Visibility FullLicenseVisibility
        {
            get
            {
                bool validFullLicense = LicenseManager.Status == LicenseStatus.Valid
                                        && LicenseManager.LicenseInfo.LicenseType == LicenseType.Full;
                return validFullLicense ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility NoLicenseVisibility
        {
            get
            {
                return LicenseManager.Status == LicenseStatus.NoLicense ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility TrialLicenseVisibility
        {
            get
            {
                bool validTrialLicense = LicenseManager.Status == LicenseStatus.Valid
                                        && LicenseManager.LicenseInfo.LicenseType == LicenseType.Trial;
                return validTrialLicense ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
