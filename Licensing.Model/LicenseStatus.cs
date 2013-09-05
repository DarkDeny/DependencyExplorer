namespace Licensing.Model
{
    public enum LicenseStatus
    {
        NoLicense,
        Valid,
        Expired,
        OlderVersionLicense,
        
        // Invalid license treated as No License case
        //InvalidLicense,
    }
}