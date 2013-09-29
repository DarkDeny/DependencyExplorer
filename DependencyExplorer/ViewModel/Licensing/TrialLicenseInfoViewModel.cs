using System.Windows.Input;
using DependencyExplorer.Infrastructure;
using DependencyExplorer.View.Licensing;
using DependencyVisualizer.Commands;

namespace DependencyExplorer.ViewModel.Licensing {
    public class TrialLicenseInfoViewModel : LicenseViewModelBase {
        public TrialLicenseInfoViewModel(LicenseManager licenseManager, IUIWindowDialogService dialogService)
            : base(licenseManager) {
            DialogService = dialogService;
        }

        private IUIWindowDialogService DialogService { get; set; }

        public ICommand BuyCommand {
            get {
                return new DelegateCommand(canExecute => true,
                    () => {
                        // TODO: redirect user to buying site!
                    });
            }
        }

        public ICommand EnterLicenseCommand {
            get {
                return new DelegateCommand(canExecute => true,
                    () =>
                        // TODO: pass this.window as parent!
                        DialogService.Show<EnterLicenseView>("Enter license", null)
                    );
            }
        }
    }
}
