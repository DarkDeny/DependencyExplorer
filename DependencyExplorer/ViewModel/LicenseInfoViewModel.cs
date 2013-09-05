using System.Windows;

using DependencyExplorer.Infrastructure;

using Licensing.Model;

namespace DependencyExplorer.ViewModel
{
    public class LicenseInfoViewModel : ViewModelBase
    {
        public LicenseInfoViewModel(): this(new LicenseManager())
        {
            
        }

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
                return this.LicenseManager.LicenseInfo.ToString();
            }
        }

        public Visibility FullLicenseVisibility
        {
            get
            {
                return LicenseManager.Status == LicenseStatus.ValidFull ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility NoLicenseVisibility
        {
            get
            {
                return LicenseManager.LicenseInfo.LicenseType == LicenseStatus.NoLicense ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility TrialLicenseVisibility
        {
            get
            {
                return LicenseManager.LicenseInfo.LicenseType == LicenseStatus.ValidTrial ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
