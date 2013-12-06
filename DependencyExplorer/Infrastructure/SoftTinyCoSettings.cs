using System.Configuration;

namespace DependencyExplorer.Infrastructure {
    public class SoftTinyCoSettings : ConfigurationSection {
        private static readonly SoftTinyCoSettings SettingsInstance =
            ConfigurationManager.GetSection("SoftTinyCoSettings") as SoftTinyCoSettings;

        public static SoftTinyCoSettings Settings {
            get {
                return SettingsInstance;
            }
        }

        [ConfigurationProperty("ServerUrl", IsRequired = true)]
        public string ServerUrl {
            get { return (string)this["ServerUrl"]; }
            set { this["ServerUrl"] = value; }
        }

        [ConfigurationProperty("BuyUrl", IsRequired = true)]
        public string BuyUrl {
            get { return (string)this["BuyUrl"]; }
            set { this["BuyUrl"] = value; }
        }
    }
}
