using DependencyExplorer.View;

namespace DependencyExplorer.ViewModel
{
    public class UIWindowDialogService : IUIWindowDialogService
    {
        public bool? ShowDialog(string title, object datacontext)
        {
            var win = new WindowDialog { Title = title, DataContext = datacontext };

            return win.ShowDialog();
        }

    }
}