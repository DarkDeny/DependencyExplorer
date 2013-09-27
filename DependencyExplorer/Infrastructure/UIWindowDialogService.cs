using System.Windows;

using DependencyExplorer.View;
using DependencyExplorer.View.LicenseForms;
using DependencyExplorer.ViewModel;
using Licensing.Model;
using StructureMap;

namespace DependencyExplorer.Infrastructure
{
    public class UIWindowDialogService : IUIWindowDialogService
    {
        public bool? ShowDialog(string title, object datacontext, Window ownerWindow)
        {
            var win = new DialogWindow { Title = title, DataContext = datacontext, Owner = ownerWindow };
            return win.ShowDialog();
        }
        
        public bool? ShowLicenseDialog(string title, Window ownerWindow) 
        {
            Window win = null;
            LicenseViewModelBase viewModel = null;
            switch (LicenseManager.Instance.LicenseInfo.LicenseType)
            {
                case LicenseType.Full:
                    viewModel = ObjectFactory.GetInstance<TrialLicenseInfoViewModel>();
                    win = new FullLicenseInfo { Title = title, DataContext = viewModel, Owner = ownerWindow };
                    break;
                case LicenseType.Trial:
                    viewModel = ObjectFactory.GetInstance<FullLicenseInfoViewModel>();
                    win = new TrialLicenseInfo { Title = title, DataContext = viewModel, Owner = ownerWindow };
                    break;
            }

            return null != win ? win.ShowDialog() : null;
        }
    }
}