using System;

namespace Licensing.Model
{
    public class LicenseInfo
    {
        public string Username { get; set; }

        public DateTime ValidUntil { get; set; }

        public LicenseType LicenseType { get; set; }

        public override string ToString()
        {
            return String.Format("Registered to:\n{0}\nValid until:{1}\nLicense type:{2}", this.Username, this.ValidUntil, this.LicenseType);
        }
    }
}