using System.Windows;

namespace DependencyExplorer.ViewModel
{
    public class WindowViewModelBase : ViewModelBase
    {
        protected Window Window { get; private set; }

        public WindowViewModelBase(Window window)
        {
            Window = window;
        }
    }
}