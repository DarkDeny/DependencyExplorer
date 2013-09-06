using System;
using System.Globalization;
using System.Threading;

namespace Licensing.Model
{
    public class LicenseInfo
    {
        public string Username { get; set; }

        public DateTime ValidUntil { get; set; }

        public LicenseType LicenseType { get; set; }

        public override string ToString()
        {
            var oldCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var validUntil = this.ValidUntil.ToShortDateString();
            Thread.CurrentThread.CurrentCulture = oldCulture;

            return String.Format("Registered to:\n{0}\nValid until:\n{1}\nLicense type:\n{2}", this.Username, validUntil, this.LicenseType);
        }
    }
}