using System;
using System.Reflection;
using System.Windows;

using DependencyExplorer.Infrastructure;
using DependencyExplorer.View;
using DependencyExplorer.ViewModel;

using StructureMap;

namespace DependencyExplorer {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App {
        public App() {
            ContainerBootstrapper.BootstrapStructureMap();

            Startup += AppStartup;
        }

        private void AppStartup(object sender, StartupEventArgs e) {
            var uiService = ObjectFactory.GetInstance<IUIWindowDialogService>();

            var window = new MainWindow();
            var dependencyExplorerViewModel = new DependencyExplorerViewModel(uiService, window);
            window.DataContext = dependencyExplorerViewModel;
            window.Show();
        }

        public static string ProductName {
            get {
                return DependencyExplorer.Properties.Resources.ApplicationName;
            }
        }

        public static string CompanyName {
            get {
                return DependencyExplorer.Properties.Resources.CompanyName;
            }
        }

        public static string VersionString {
            get {
                return Version.ToString(2);
            }
        }

        public static Version Version {
            get {
                return Assembly.GetCallingAssembly().GetName().Version;
            }
        }
    }
}
