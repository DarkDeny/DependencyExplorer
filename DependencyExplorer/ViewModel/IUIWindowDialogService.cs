using System.Windows;

namespace DependencyExplorer.ViewModel
{
    public interface IUIWindowDialogService
    {
        bool? ShowDialog(string title, object datacontext, Window ownerWindow);
    }
}