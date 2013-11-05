using System;
using System.Collections.Specialized;
using System.Management;
using System.Net;
using System.Text;
using System.Windows;

using DependencyExplorer.Infrastructure;

using Licensing.Model;

namespace DependencyExplorer.ViewModel.Licensing {
    public class GetTrialLicenseViewModel : LicenseViewModelBase {
        public GetTrialLicenseViewModel(
            LicenseManager licenseManager,
            IUIWindowDialogService dialogService,
            Window window) : base(licenseManager, dialogService, window) {
                window.Loaded += OnWindowLoaded;
        }

        void OnWindowLoaded(object sender, RoutedEventArgs e) {
            var cpuID = GetCpuID();
            var uri = new Uri(SoftTinyCoSettings.Settings.ServerUrl);
            var webClient = new WebClient();
            var reqparm = new NameValueCollection
                {
                    {"hardware_id", cpuID},
                    {"product_id", "1"},
                };
            webClient.UploadValuesCompleted += OnUploadCompleted;
            webClient.UploadValuesAsync(uri, "POST", reqparm);
        }

        private void OnUploadCompleted(object sender, UploadValuesCompletedEventArgs e) {
            var response = Encoding.UTF8.GetString(e.Result);
            var license = LicenseManager.ParseLicense(response);
            if (license.Status == LicenseStatus.Valid || license.Status == LicenseStatus.ExpiredTrial) {
                LicenseManager.PersistLicense(license);
            }
        }

        private static string GetCpuID() {
            var search = new ManagementObjectSearcher(String.Format("SELECT * FROM Win32_Processor"));
            foreach (var manObj in search.Get()) {
                return manObj.GetPropertyValue("ProcessorID").ToString();
            }
            throw new InvalidOperationException("Oh noes! There are no CPU!");
        }
    }
}
