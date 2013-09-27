using System.Windows;

namespace DependencyExplorer.Infrastructure
{
    public interface IUIWindowDialogService
    {
        bool? ShowDialog(string title, object datacontext, Window ownerWindow);
        bool? ShowLicenseDialog(string title, Window ownerWindow);
    }
}