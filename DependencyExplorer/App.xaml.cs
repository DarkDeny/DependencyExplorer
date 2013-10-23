using System;
using System.IO;
using System.Reflection;
using System.Text;
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
            DispatcherUnhandledException += AppDispatcherUnhandledException;
        }

        void AppDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {

            var timeHappened = DateTime.Now;
            var date = String.Format("{0}{1}{2}", timeHappened.Year.ToString("D4"), timeHappened.Month.ToString("D2"), timeHappened.Day.ToString("D2"));
            var time = String.Format("{0}{1}{2}", timeHappened.Hour.ToString("D2"), timeHappened.Minute.ToString("D2"), timeHappened.Second.ToString("D2"));
            var timeStamp = String.Format("{0}-{1}", date, time);

            var errorDetails = UnwindException(e.Exception);

            var errorReport = String.Format(
                "Product: {0}\n  Timestamp: {1}\n  Version: {2}\n{3}",
                ProductName,
                timeStamp,
                FullVersionString,
                errorDetails);

            var errorFilename = String.Format("CrashReport-{0}.txt", timeStamp);

            try {
                var companyFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    CompanyName);
                var productFolder = Path.Combine(companyFolder, ProductName);
                if (!Directory.Exists(companyFolder)) {
                    Directory.CreateDirectory(companyFolder);
                }
                if (!Directory.Exists(productFolder)) {
                    Directory.CreateDirectory(productFolder);
                }
                Directory.SetCurrentDirectory(productFolder);

                var errorFile = File.CreateText(errorFilename);
                errorFile.WriteLine(errorReport);
                errorFile.Close();

                MessageBox.Show(
                    String.Format("{0} encountered an error.\n\nError report {1} was created.\n\nNext time you run {0}, we will try to send this report to developer.", ProductName, Path.Combine(productFolder, errorFilename)),
                    "Error");
            } catch {
                MessageBox.Show(
                    "Something very strange happened - We were not able to create an error report.\n\nPlease contact us if problem persists!",
                    "Unrecoverable error");
            }

            e.Handled = true;
            Shutdown();
        }

        private const string Spacing = "  ";

        private string UnwindException(Exception exception) {
            var sb = new StringBuilder();
            var currentException = exception;
            var currentSpacings = String.Empty;
            while (null != currentException) {
                currentSpacings += Spacing;
                sb.AppendFormat(
                    "{0}Unhandled exception: ({1}) {2}\n{0}StackTrace:\n{3}\n\n",
                    currentSpacings,
                    currentException.GetType(),
                    currentException.Message,
                    currentException.StackTrace);
                currentException = currentException.InnerException;
            }

            return sb.ToString();
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
