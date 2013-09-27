using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Licensing.Model;

namespace DependencyExplorer.Helpers {
    public class LicenseStatusToBrushConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var typedValue = value is LicenseStatus ? (LicenseStatus)value : LicenseStatus.NoLicense;
            switch (typedValue) {
                case LicenseStatus.Valid:
                    // Green light on valid license
                    return new SolidColorBrush(Colors.ForestGreen);
                case LicenseStatus.NoLicense:
                    // No license entered - nothing to signal about yet
                    return new SolidColorBrush(Colors.Transparent);
                case LicenseStatus.Expired:
                case LicenseStatus.InvalidLicense:
                case LicenseStatus.OlderVersionLicense:
                    // Red light on invalid license
                    return new SolidColorBrush(Colors.Firebrick);

                default:
                    // Should not be here
                    throw new ArgumentException("Wrong License status!");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
