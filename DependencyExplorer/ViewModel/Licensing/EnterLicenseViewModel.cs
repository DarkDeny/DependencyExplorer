using System;
using System.Windows;
using System.Windows.Input;
using DependencyExplorer.Commands;
using DependencyExplorer.Infrastructure;
using Licensing.Model;

namespace DependencyExplorer.ViewModel.Licensing {
    public class EnterLicenseViewModel : LicenseViewModelBase {
        public EnterLicenseViewModel(LicenseManager licenseManager, IUIWindowDialogService dialogService, Window window)
            : base(licenseManager, dialogService, window) {
            _LicenseContent = "Paste your license here:";
            _ApplyLicenseCommand = new DelegateCommand(CanApplyLicense, ApplyLicense);
            
        }

        private readonly DelegateCommand _ApplyLicenseCommand;

        private string _LicenseContent;
        public string LicenseContent {
            get {
                return _LicenseContent;
            }
            set {
                _LicenseContent = value;
                NewLicense = LicenseManager.ParseLicense(_LicenseContent);
                OnAllPropertiesChanged();
                _ApplyLicenseCommand.FireCanExecuteChanged();
            }
        }

        private LicenseInfo NewLicense { get; set; }

        public LicenseStatus LicenseStatus {
            get {
                return NewLicense.Status;
            }
        }

        public String LicenseStatusText {
            get {
                return LicenseManager.LicenseInfo.ErrorMessage;
            }
        }

        private  bool CanApplyLicense(object obj) {
            return null != NewLicense && NewLicense.Status == LicenseStatus.Valid;
        }

        private void ApplyLicense() {
            LicenseManager.PersistLicense(NewLicense);
            Window.Close();
        }

        public ICommand ApplyLicenseCommand {
            get {
                return _ApplyLicenseCommand;
            }
        }
    }
}
