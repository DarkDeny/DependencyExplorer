using System;
using System.Windows;
using System.Windows.Input;
using DependencyExplorer.Infrastructure;
using DependencyExplorer.View.Licensing;
using DependencyVisualizer.Commands;

using Licensing.Model;

namespace DependencyExplorer.ViewModel.Licensing {
    public class TrialLicenseInfoViewModel : LicenseViewModelBase {
        public TrialLicenseInfoViewModel(LicenseManager licenseManager, IUIWindowDialogService dialogService, Window window)
            : base(licenseManager, dialogService, window) {
            DialogService = dialogService;
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
                if (HaveFullLicense()) {
                    return LicenseManager.LicenseInfo.Username;
                }

                return "Product is not licensed yet.";
            }
        }

        private bool HaveFullLicense() {
            return LicenseManager.Status == LicenseStatus.Valid
                   && LicenseManager.LicenseInfo.LicenseType == LicenseType.Full;
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
    }
}
