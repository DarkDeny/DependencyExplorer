using System.Windows;

namespace DependencyExplorer.Infrastructure
{
    public interface IUIWindowDialogService
    {
        bool? ShowLicenseDialog(string title, Window parentWindow);
        void Show<TView>(string title, Window parent) where TView : Window;
    }
}