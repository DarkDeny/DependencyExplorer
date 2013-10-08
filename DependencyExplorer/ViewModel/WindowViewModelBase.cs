using System.Windows;

using DependencyExplorer.Infrastructure;

namespace DependencyExplorer.ViewModel
{
    public class WindowViewModelBase : ViewModelBase {
        public WindowViewModelBase(IUIWindowDialogService dialogService, Window window) {
            Window = window;
            DialogService = dialogService;
        }

        protected Window Window { get; private set; }

        protected IUIWindowDialogService DialogService { get; set; }
    }
}