namespace DependencyExplorer.ViewModel
{
    public interface IUIWindowDialogService
    {
        bool? ShowDialog(string title, object datacontext);
    }
}