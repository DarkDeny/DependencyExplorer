using System.Diagnostics;
using System.Reflection;

namespace DependencyExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
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
