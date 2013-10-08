using System.Windows;

namespace DependencyExplorer.Infrastructure
{
    public interface IUIWindowDialogService
    {
        bool? ShowLicenseDialog(string caption, Window parentWindow);
        void Show<TView>(string title, Window parent) where TView : Window;
    }
}