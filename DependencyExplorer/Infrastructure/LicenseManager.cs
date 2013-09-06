using System;

using Licensing.Model;

namespace DependencyExplorer.Infrastructure
{
    public class LicenseManager
    {
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
                LicenseInfo =
                    new LicenseInfo
                    {
                        Username = "Cowabunga User",
                        ValidUntil = DateTime.Now.AddYears(1),
                        LicenseType = LicenseType.Full,
                    },
                Status = LicenseStatus.Valid
            };
        }

        public LicenseStatus Status { get; private set; }
        public LicenseInfo LicenseInfo { get; private set; }
    }
}
