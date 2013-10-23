using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using Licensing.Model;
using Microsoft.Win32;

using StructureMap;

namespace DependencyExplorer.Infrastructure
{
    public class LicenseManager
    {
        private static readonly string RegistryKeyPath = Path.Combine(App.CompanyName, App.ProductName); 
        private const string PublicKey = "<RSAKeyValue><Modulus>xnwFxcvemQz9vXrS/a18lZUx1vgxFPdi8MMOpI/ed5nUZV2pyX+B6oDQ2lddXDbbq0qhPA9K9lPZtafxGeGH6Tc2hV4ERYlW2ED+5kgZaaU5SivNKHId6RhJkUmKdIbjFYAyY3oWeeYnPCqqvf8wDNEkdHE1VNL5P0k9LtFq12M=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private static readonly string HashAlgo = CryptoConfig.MapNameToOID("SHA1");

        private const string LicenseRegistryValueName = "License";

        private static LicenseManager Instance { get; set; }

        private LicenseManager() {
            LicenseInfo = new LicenseInfo { Status = LicenseStatus.NoLicense };
        }

        static LicenseManager()
        {
            // TODO: no valid license => try to connect to api.softtiny.com and retrieve trial license.

            LoadLicense();
            CheckExpiration(Instance.LicenseInfo);
        }

        private static void CheckExpiration(LicenseInfo license) {
            if (license.ValidUntil < DateTime.Now) {
                if (license.LicenseType == LicenseType.Trial) {
                    license.Status = LicenseStatus.ExpiredTrial;
                }
            }
        }

        private static void LoadLicense() {
            Instance = new LicenseManager();

            var rkCurrentUser = Registry.CurrentUser;
            var rkSoftware = rkCurrentUser.OpenSubKey("Software", RegistryKeyPermissionCheck.ReadSubTree);
            if (rkSoftware != null) {
                var rkDepExp = rkSoftware.OpenSubKey(RegistryKeyPath, RegistryKeyPermissionCheck.ReadSubTree);
                if (null == rkDepExp) {
                    return;
                }
                var licenseStrings = rkDepExp.GetValue(LicenseRegistryValueName) as string[];
                if (null != licenseStrings) {
                    Instance.LicenseInfo = ParseLicense(licenseStrings);
                }
            }
        }

        public LicenseInfo LicenseInfo { get; private set; }

        public LicenseInfo ParseLicense(string licenseUnderCheck)
        {
            var licenseStrings = licenseUnderCheck.Split('\n');
            return ParseLicense(licenseStrings);
        }

        public static LicenseInfo ParseLicense(string[] licenseStrings) {
            for (int index = 0; index < licenseStrings.Length; index++) {
                licenseStrings[index] = licenseStrings[index].Trim('\r');
            }

            var license = new LicenseInfo();
            if (licenseStrings.Length < 7) {
                license.Status = LicenseStatus.Incomplete;
                return license;
            }

            var productName = ExtractValue(licenseStrings[0]);
            license.Username = ExtractValue(licenseStrings[2]);
            license.ProductName = productName;
            if (App.ProductName != productName) {
                license.InvalidateStatus("Wrong license - product name mismatched.");
                return license;
            }

            DateTime validThrough;

            var oldCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var dateString = ExtractValue(licenseStrings[3]);
            var dateParsed = DateTime.TryParse(dateString, out validThrough);
            Thread.CurrentThread.CurrentCulture = oldCulture;
            
            if (!dateParsed) {
                license.InvalidateStatus("Wrong license - unable to parse date.");
                return license;
            }
            license.ValidUntil = validThrough;

            LicenseType type;
            var typeString = ExtractValue(licenseStrings[4]);
            var typeParsed = Enum.TryParse(typeString, out type);
            if (!typeParsed)
            {
                license.InvalidateStatus("Wrong license - unable to parse license type.");
                return license;
            }
            license.LicenseType = type;

            Version version;
            var versionString = ExtractValue(licenseStrings[1]);
            var versionParsed = Version.TryParse(versionString, out version);
            if (!versionParsed)
            {
                license.InvalidateStatus("Wrong license - unable to parse registered version.");
                return license;
            }
            
            if (version.Major < App.Version.Major )
            {
                license.InvalidateStatus("Wrong license - license version mismatch product version.");
                return license;
            }
            license.LicensedVersion = version;

            license.LicenseKey = String.Concat(licenseStrings.Skip(6));
            var licenseCheckResult = CheckLicense(license);

            if (!licenseCheckResult)
            {
                license.InvalidateStatus("Wrong license - bad license key.");
            }
            else
            {
                license.Status = LicenseStatus.Valid;
                CheckExpiration(license);
            }
            return license;
        }

        private static string ExtractValue(string licenseString) {
            var start = licenseString.IndexOf(": ", StringComparison.Ordinal) + 2;
            return licenseString.Substring(start);
        }

        private static bool CheckLicense(LicenseInfo licenseInfo) {
            using (var rsa = new RSACryptoServiceProvider()) {
                rsa.FromXmlString(PublicKey);

                var licenseText = licenseInfo.ToString();
                var hashBytes = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(licenseText));

                var signatureBytes = Convert.FromBase64String(licenseInfo.LicenseKey);

                var result = rsa.VerifyHash(hashBytes, HashAlgo, signatureBytes);
                return result;
            }
        }

        public void PersistLicense(LicenseInfo license) {
            try {
                LicenseInfo = license;
                var rkCurrentUser = Registry.CurrentUser;
                var rkSoftware = rkCurrentUser.OpenSubKey("Software", RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (rkSoftware != null) {
                    var rkDepExp = rkSoftware.CreateSubKey(RegistryKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
                    if (null != rkDepExp) {
                        rkDepExp.SetValue(LicenseRegistryValueName, LicenseInfo.ToFullString().Split('\n'), RegistryValueKind.MultiString);
                    }
                }

                rkCurrentUser.Close();
            } catch {
                //TODO: log this error and let user know!
            }
        }

        public static LicenseManager GetInstance() {
            return Instance;
        }
    }
}
