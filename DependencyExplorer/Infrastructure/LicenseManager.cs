namespace DependencyExplorer.Infrastructure
{
    internal class LicenseManager
    {
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
        }
    }

    public enum LicenseStatus
    {
        NoLicense,
        ValidTrial,
        ExpiredTrial,
        ValidLicense,
        OlderVersionLicense,
    }
}
