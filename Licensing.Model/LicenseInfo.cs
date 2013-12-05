using System;
using System.Globalization;
using System.Threading;

namespace Licensing.Model {
    public class LicenseInfo {
        public string ProductName { get; set; }

        public string Username { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime ValidUntil { get; set; }

        public string LicenseKey { get; set; }

        public Version LicensedVersion { get; set; }

        public string ValidThroughText {
            get {
                return String.Format("{0:D2}/{1:D2}/{2}", ValidUntil.Month, ValidUntil.Day, ValidUntil.Year);
            }
        }

        private LicenseStatus status;
        public LicenseStatus Status {
            get {
                return status;
            }
            set {
                status = value;
                switch (status) {
                    case LicenseStatus.ExpiredTrial:
                        ErrorMessage = "Your trial license has expired.";
                        break;
                    case LicenseStatus.Incomplete:
                        ErrorMessage = "Wrong license - incomplete license text.";
                        break;
                    case LicenseStatus.Valid:
                        ErrorMessage = LicenseType == LicenseType.Full
                                           ? "License ACCEPTED! Thanks for buying our product. Please do not forget to let us know what you think about it!"
                                           : "You have trial license.";
                        break;
                }
            }
        }

        public void InvalidateStatus(string errorMessage) {
            Status = LicenseStatus.InvalidLicense;
            ErrorMessage = errorMessage;
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