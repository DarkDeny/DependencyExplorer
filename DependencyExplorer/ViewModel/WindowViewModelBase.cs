using System.Windows;

using DependencyExplorer.Infrastructure;

namespace DependencyExplorer.ViewModel
{
    public class WindowViewModelBase : ViewModelBase {
        public WindowViewModelBase(IUIWindowDialogService dialogService, Window window) {
            this.Window = window;
            this.DialogService = dialogService;
        }

        protected Window Window { get; set; }

        protected IUIWindowDialogService DialogService { get; set; }
    }
}