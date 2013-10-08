using System;
using System.Windows;
using System.Windows.Input;

using DependencyExplorer.Infrastructure;

using DependencyVisualizer.Commands;

using Licensing.Model;

namespace DependencyExplorer.ViewModel.Licensing {
    public class EnterLicenseViewModel : LicenseViewModelBase {
        public EnterLicenseViewModel(LicenseManager licenseManager, IUIWindowDialogService dialogService, Window window)
            : base(licenseManager, dialogService, window) {
            _LicenseContent = "Paste your license here:";
        }

        private string _LicenseContent;
        public string LicenseContent {
            get {
                return _LicenseContent;
            }
            set {
                _LicenseContent = value;
                LicenseManager.Instance.ParseLicense(_LicenseContent);
                OnAllPropertiesChanged();
            }
        }

        public LicenseStatus LicenseStatus {
            get {
                return LicenseManager.Instance.Status;
            }
        }

        public String LicenseStatusText {
            get {
                return LicenseManager.Instance.LicenseInfo.ErrorMessage;
            }
        }

        private  bool CanApplyLicense(object obj) {
            return LicenseManager.Instance.Status == LicenseStatus.Valid;
        }

        private void ApplyLicense() {
            LicenseManager.Instance.PersistLicense();
            Window.Close();
        }

        public ICommand ApplyLicenseCommand {
            get {
                return new DelegateCommand(CanApplyLicense, ApplyLicense);
            }
        }
    }
}
