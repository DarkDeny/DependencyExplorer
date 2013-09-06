using System.Windows;

using DependencyExplorer.View;
using DependencyExplorer.ViewModel;

namespace DependencyExplorer.Infrastructure
{
    public class UIWindowDialogService : IUIWindowDialogService
    {
        public bool? ShowDialog(string title, object datacontext, Window ownerWindow)
        {
            var win = new DialogWindow { Title = title, DataContext = datacontext, Owner = ownerWindow };
            return win.ShowDialog();
        }
    }
}