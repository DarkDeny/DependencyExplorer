namespace DependencyExplorer.ViewModel
{
    public class LicenseInfoViewModel : ViewModelBase
    {
        private string _LicenseStatus;

        public string LicenseStatus
        {
            get
            {
                return _LicenseStatus;
            }
            set
            {
                _LicenseStatus = value;
                OnPropertyChanged();
            }
        }

        public LicenseInfoViewModel()
        {
            LicenseStatus = "cowabunga!";
        }
    }
}
