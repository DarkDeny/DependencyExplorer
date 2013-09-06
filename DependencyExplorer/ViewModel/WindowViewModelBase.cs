using System.Windows;

namespace DependencyExplorer.ViewModel
{
    public class WindowViewModelBase : ViewModelBase
    {
        protected Window Window { get; set; }

        public WindowViewModelBase(Window window)
        {
            this.Window = window;
        }
    }
}