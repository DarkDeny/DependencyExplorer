﻿using System;
using System.Security.Cryptography;
using System.Text;
using Licensing.Model;

namespace DependencyExplorer.Infrastructure
{
    public class LicenseManager
    {
        private const string PublicKey = "<RSAKeyValue><Modulus>xnwFxcvemQz9vXrS/a18lZUx1vgxFPdi8MMOpI/ed5nUZV2pyX+B6oDQ2lddXDbbq0qhPA9K9lPZtafxGeGH6Tc2hV4ERYlW2ED+5kgZaaU5SivNKHId6RhJkUmKdIbjFYAyY3oWeeYnPCqqvf8wDNEkdHE1VNL5P0k9LtFq12M=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private static readonly string HashAlgo = CryptoConfig.MapNameToOID("SHA1");

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
            Instance = new LicenseManager
            {
                LicenseInfo = new LicenseInfo(),
                Status = LicenseStatus.InvalidLicense
            };
        }

        public LicenseStatus Status { get; private set; }

        public LicenseInfo LicenseInfo { get; private set; }

        public void ParseLicense(string licenseUnderCheck)
        {
            var licenseStrings = licenseUnderCheck.Split('\n');

            LicenseInfo = new LicenseInfo { Username = licenseStrings[1] };
            Status = LicenseStatus.InvalidLicense;
            DateTime validThrough;
            var dateParsed = DateTime.TryParse(licenseStrings[3], out validThrough);
            if (!dateParsed)
            {
                LicenseInfo.ErrorMessage = "Wrong license - unable to parse date.";
                return;
            }
            LicenseInfo.ValidUntil = validThrough;

            LicenseType type;
            var typeParsed = Enum.TryParse(licenseStrings[5], out type);
            if (!typeParsed)
            {
                LicenseInfo.ErrorMessage = "Wrong license - unable to parse license type.";
                return;
            }
            LicenseInfo.LicenseType = type;

            Version version;
            var versionParsed = Version.TryParse(licenseStrings[7], out version);
            if (!versionParsed)
            {
                LicenseInfo.ErrorMessage = "Wrong license - unable to parse registered version.";
                return;
            }
            LicenseInfo.LicensedVersion = version;
            LicenseInfo.LicenseKey = licenseStrings[9];

            var licenseCheckResult = CheckLicense(LicenseInfo);

            if (!licenseCheckResult)
            {
                LicenseInfo.ErrorMessage = "Wrong license - bad license key.";
            }
            else
            {
                LicenseInfo.ErrorMessage = "License ACCEPTED! Thanks for buying our product. Please do not forget to let us know what you think about it!";
                Status = LicenseStatus.Valid;
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
    }
}
