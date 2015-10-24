using System.Windows;

namespace DependencyExplorer.Infrastructure {
    public interface IUIWindowDialogService {
        void Show<TView>(string title, Window parent) where TView : Window;
    }
}