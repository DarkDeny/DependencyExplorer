namespace Licensing.Model
{
    public enum LicenseStatus
    {
        NoLicense,
        Valid,
        Expired,
        OlderVersionLicense,
        
        // Invalid license means that user entered non valid value as a license. This differs from case when there is no license at all
        InvalidLicense,
    }
}