using System;
using System.IO;
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
            ContainerBootstrapper.BootstrapStructureMap(this);

            Startup += AppStartup;
            DispatcherUnhandledException += AppDispatcherUnhandledException;
        }

        public void LogFailure(string situation, string userErrorMessage, Exception exception) {
            ErrorManager.LogException(ProductName, FullVersionString, GetProductFolder(), situation, exception);
            MessageBox.Show(userErrorMessage, "Error occured");
            Shutdown();
        }

        public void LogError(string situation, string userErrorMessage) {
            ErrorManager.LogError(ProductName, FullVersionString, GetProductFolder(), situation, userErrorMessage);
        }

        private void AppDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            var userErrorMessage =
                String.Format(
                    "{0} encountered an error.\n\nError report was created in the {1} folder.\n\nNext time you run {0}, we will try to send this report to developer.",
                    ProductName,
                    GetProductFolder());
            e.Handled = true;
            LogFailure("AppCrash", userErrorMessage, e.Exception);
        }

        private static string GetProductFolder() {
            var companyFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                CompanyName);
            
            var productFolder = Path.Combine(companyFolder, ProductName);
            if (!Directory.Exists(productFolder)) {
                Directory.CreateDirectory(productFolder);
            }
            return productFolder;
        }

        private void AppStartup(object sender, StartupEventArgs e) {
            var licenseManager = ObjectFactory.GetInstance<LicenseManager>();
            var uiService = ObjectFactory.GetInstance<IUIWindowDialogService>();
            var window = new MainWindow();
            var dependencyExplorerViewModel = new DependencyExplorerViewModel(licenseManager, uiService, window);
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

        public static string FullVersionString {
            get {
                return Version.ToString(4);
            }
        }

        public static Version Version {
            get {
                return Assembly.GetCallingAssembly().GetName().Version;
            }
        }
    }
}
