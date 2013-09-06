using System.Diagnostics;
using System.Reflection;
using System.Windows;

using DependencyExplorer.Infrastructure;
using DependencyExplorer.View;
using DependencyExplorer.ViewModel;

using StructureMap;

namespace DependencyExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            ContainerBootstrapper.BootstrapStructureMap();

            Startup += AppStartup;
        }

        private void AppStartup(object sender, StartupEventArgs e)
        {
            var uiService = ObjectFactory.GetInstance<IUIWindowDialogService>();

            var window = new MainWindow();
            var dependencyExplorerViewModel = new DependencyExplorerViewModel(uiService, window);
            window.DataContext = dependencyExplorerViewModel;
            window.Show();
        }

        public static string Name
        {
            get
            {
                return "Dependency Explorer";
            }
        }

        public static string Version
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fileVersionInfo.ProductVersion;
            }
        }
    }
}
