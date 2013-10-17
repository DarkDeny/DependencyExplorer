using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Licensing.Model;
using Microsoft.Win32;

namespace DependencyExplorer.Infrastructure
{
    public class LicenseManager
    {
        private static readonly string RegistryKeyPath = Path.Combine(App.CompanyName, App.ProductName); 
        private const string PublicKey = "<RSAKeyValue><Modulus>xnwFxcvemQz9vXrS/a18lZUx1vgxFPdi8MMOpI/ed5nUZV2pyX+B6oDQ2lddXDbbq0qhPA9K9lPZtafxGeGH6Tc2hV4ERYlW2ED+5kgZaaU5SivNKHId6RhJkUmKdIbjFYAyY3oWeeYnPCqqvf8wDNEkdHE1VNL5P0k9LtFq12M=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private static readonly string HashAlgo = CryptoConfig.MapNameToOID("SHA1");
        private const string LicenseRegistryValueName = "License";

        public static LicenseManager Instance { get; private set; }

        private LicenseManager() { }

        static LicenseManager()
        {
            // TODO: check registry key existance
            // TODO: if no key found => LicenseStatus = NoLicense
            // TODO: if key exist => check license data:
                // TODO: correct trial license => LicenseStatus = Valid
                // TODO: correct full license => LicenseStatus = Valid
                // TODO: expired trial license => LicenseStatus = ExpiredTrial
                // TODO: expired full license => check version:
                    // TODO: expired full, same version => LicenseStatus = Valid
                    // TODO: expired full, version is newer => LicenseStatus = Invalid


            // TODO: no valid license => warn user, give link to site, refuse to work
            LoadLicense();
        }

        private static void LoadLicense()
        {
            Instance = new LicenseManager
            {
                LicenseInfo = new LicenseInfo(),
                Status = LicenseStatus.InvalidLicense
            };

            var rkCurrentUser = Registry.CurrentUser;
            var rkSoftware = rkCurrentUser.OpenSubKey("Software", RegistryKeyPermissionCheck.ReadSubTree);
            var rkDepExp = rkSoftware.OpenSubKey(RegistryKeyPath, RegistryKeyPermissionCheck.ReadSubTree);
            if (null == rkDepExp) {
                return;
            }
            var licenseStrings = rkDepExp.GetValue(LicenseRegistryValueName) as string[];
            if (null != licenseStrings) {
                ParseLicense(licenseStrings);
            }
        }

        public LicenseStatus Status { get; private set; }

        public LicenseInfo LicenseInfo { get; private set; }

        public void ParseLicense(string licenseUnderCheck)
        {
            var licenseStrings = licenseUnderCheck.Split('\n');
            ParseLicense(licenseStrings);
        }

        public static void ParseLicense(string[] licenseStrings) {
            for (int index = 0; index < licenseStrings.Length; index++) {
                licenseStrings[index] = licenseStrings[index].Trim('\r');
            }

            if (licenseStrings.Length < 10) {
                Instance.LicenseInfo.ErrorMessage = "Wrong license - incomplete license text.";
                return;
            }

            Instance.LicenseInfo = new LicenseInfo { Username = licenseStrings[1] };
            Instance.Status = LicenseStatus.InvalidLicense;
            DateTime validThrough;

            var oldCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var dateParsed = DateTime.TryParse(licenseStrings[3], out validThrough);
            Thread.CurrentThread.CurrentCulture = oldCulture;
            
            if (!dateParsed)
            {
                Instance.LicenseInfo.ErrorMessage = "Wrong license - unable to parse date.";
                return;
            }
            Instance.LicenseInfo.ValidUntil = validThrough;

            LicenseType type;
            var typeParsed = Enum.TryParse(licenseStrings[5], out type);
            if (!typeParsed)
            {
                Instance.LicenseInfo.ErrorMessage = "Wrong license - unable to parse license type.";
                return;
            }
            Instance.LicenseInfo.LicenseType = type;

            Version version;
            var versionParsed = Version.TryParse(licenseStrings[7], out version);
            if (!versionParsed)
            {
                Instance.LicenseInfo.ErrorMessage = "Wrong license - unable to parse registered version.";
                return;
            }
            
            if (version.Major < App.Version.Major )
            {
                Instance.LicenseInfo.ErrorMessage = "Wrong license - license version mismatch product version.";
                return;
            }
            Instance.LicenseInfo.LicensedVersion = version;

            Instance.LicenseInfo.LicenseKey = String.Concat(licenseStrings.Skip(9));

            var licenseCheckResult = CheckLicense(Instance.LicenseInfo);

            if (!licenseCheckResult)
            {
                Instance.LicenseInfo.ErrorMessage = "Wrong license - bad license key.";
            }
            else
            {
                Instance.LicenseInfo.ErrorMessage = "License ACCEPTED! Thanks for buying our product. Please do not forget to let us know what you think about it!";
                Instance.Status = LicenseStatus.Valid;
            }
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

        public void PersistLicense() {
            try {
                var rkCurrentUser = Registry.CurrentUser;
                var rkSoftware = rkCurrentUser.OpenSubKey("Software", RegistryKeyPermissionCheck.ReadWriteSubTree);
                var rkDepExp = rkSoftware.CreateSubKey(RegistryKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (null != rkDepExp) {
                    rkDepExp.SetValue(LicenseRegistryValueName, LicenseInfo.ToFullString().Split('\n'), RegistryValueKind.MultiString);
                }

                rkCurrentUser.Close();
            } catch (Exception ex) {
                //TODO: log this error and let user know!
            }
        }
    }
}
