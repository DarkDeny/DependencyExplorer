using System;
using System.Globalization;
using System.Threading;

namespace Licensing.Model {
    public class LicenseInfo {
        public string ProductName { get; set; }

        public string Username { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime ValidUntil { private get; set; }

        public string LicenseKey { get; set; }

        public Version LicensedVersion { get; set; }

        public string ValidThroughText {
            get {
                var oldCulture = Thread.CurrentThread.CurrentCulture;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                var validUntil = ValidUntil.ToShortDateString();
                Thread.CurrentThread.CurrentCulture = oldCulture;

                return validUntil;
            }
        }

        public LicenseType LicenseType { get; set; }

        public override string ToString() {
            return
                String.Format(
                    "Product: {0}\nVersion: {1}.{2}\nRegistered to: {3}\nValid through: {4}\nLicense type: {5}",
                    ProductName,
                    LicensedVersion.Major,
                    LicensedVersion.Minor,
                    Username,
                    ValidThroughText,
                    LicenseType
                    );
        }

        public string ToFullString() {
            return String.Format("{0}\nLicense key:\n{1}", ToString(), LicenseKey);
        }
    }
}