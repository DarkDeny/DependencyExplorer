using System.Windows.Input;
using DependencyExplorer.Infrastructure;
using DependencyExplorer.View.LicenseForms;
using DependencyVisualizer.Commands;
using StructureMap;
using LicenseManager = DependencyExplorer.Infrastructure.LicenseManager;

namespace DependencyExplorer.ViewModel {
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
                    () => {
                        var vm = ObjectFactory.GetInstance<EnterLicenseViewModel>();
                        var view = ObjectFactory.GetInstance<EnterLicenseView>();
                        DialogService.ShowDialog("Enter license", vm, view);
                    });
            }
        }
    }
}
