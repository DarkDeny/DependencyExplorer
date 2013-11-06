using System;
using System.Collections.Specialized;
using System.Management;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;

using DependencyExplorer.Infrastructure;

using Licensing.Model;

namespace DependencyExplorer.ViewModel.Licensing {
    public class GetTrialLicenseViewModel : LicenseViewModelBase {
        public GetTrialLicenseViewModel(
            LicenseManager licenseManager,
            IUIWindowDialogService dialogService,
            App app,
            Window window) : base(licenseManager, dialogService, window) {
            window.Loaded += OnWindowLoaded;
            _app = app;
        }

        private readonly App _app;

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
            // TODO: Check with timeout for this operation!
            webClient.UploadValuesAsync(uri, "POST", reqparm);
        }

        private const string OnGetTrialFailedUserMessage = "There was an error during trying to get trial license for you. Please check that you have internet connection. If problem persists, please contact us!";

        private void OnUploadCompleted(object sender, UploadValuesCompletedEventArgs e) {
            try {
                var response = Encoding.UTF8.GetString(e.Result);
                var license = LicenseManager.ParseLicense(response);
                if (license.Status == LicenseStatus.Valid) {
                    LicenseManager.PersistLicense(license);
                    MessageBox.Show("We have successfully acquired your trial license!");
                    Window.Close();
                } else if (license.Status == LicenseStatus.ExpiredTrial) {
                    MessageBox.Show(
                        "We have successfully acquired your trial license, but it is already expired. Please consider buying product if you need it!");
                    Window.Close();
                } else {
                    _app.LogError("GetTrial", license.ErrorMessage);
                    MessageBox.Show(
                        "We were not able to get you a valid trial license. If problem persist, please contact us!");
                    Window.Close();
                }
            } catch (TargetInvocationException targetInvocationException) {
                if (targetInvocationException.InnerException is WebException) {
                    MessageBox.Show(OnGetTrialFailedUserMessage);
                    Window.Close();
                } else {
                    throw;
                }
            }
            catch (Exception ex) {
                _app.LogFailure("GetTrial", OnGetTrialFailedUserMessage, ex);
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
