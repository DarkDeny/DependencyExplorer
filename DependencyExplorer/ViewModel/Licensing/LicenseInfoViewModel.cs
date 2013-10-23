using System;
using System.Windows;
using System.Windows.Input;
using DependencyExplorer.Commands;
using DependencyExplorer.Infrastructure;
using DependencyExplorer.View.Licensing;
using Licensing.Model;

namespace DependencyExplorer.ViewModel.Licensing {
    public class LicenseInfoViewModel : LicenseViewModelBase {
        public LicenseInfoViewModel(LicenseManager licenseManager, IUIWindowDialogService dialogService, Window window)
            : base(licenseManager, dialogService, window) {
        }

        public ICommand BuyCommand {
            get {
                return new DelegateCommand(canExecute => true,
                    () => {
                        // TODO: redirect user to buying site!
                    });
            }
        }

        public String Licensee {
            get {
                if (HaveValidLicense()) {
                    return LicenseManager.LicenseInfo.Username;
                }

                return "Product is not licensed yet.";
            }
        }

        private bool HaveFullLicense() {
            return LicenseManager.LicenseInfo.Status == LicenseStatus.Valid
                   && LicenseManager.LicenseInfo.LicenseType == LicenseType.Full;
        }

        private bool HaveValidLicense() {
            return LicenseManager.LicenseInfo.Status == LicenseStatus.Valid;
        }

        public ICommand EnterLicenseCommand {
            get {
                return new DelegateCommand(canExecute => true,
                    () => {
                        DialogService.Show<EnterLicenseView>("Enter license", Window);
                        OnAllPropertiesChanged();
                    });
            }
        }

        public Visibility BuyCommandVisibility {
            get {
                return HaveFullLicense() ? Visibility.Hidden : Visibility.Visible;
            }
        }

        public string ValidMessage {
            get {
                if (HaveFullLicense()) {
                    return "Free upgrade until:";
                }
                return "Trial mode until:";
            }
        }
    }
}
