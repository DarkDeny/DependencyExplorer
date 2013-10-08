using System;
using System.Windows;
using DependencyExplorer.View.Licensing;
using Licensing.Model;
using StructureMap;

namespace DependencyExplorer.Infrastructure {
    public class UIWindowDialogService : IUIWindowDialogService {
        public bool? ShowLicenseDialog(string caption, Window parent) {
            UIWindowDialogService win = null;
            switch (LicenseManager.Instance.LicenseInfo.LicenseType) {
                case LicenseType.Full:
                    win = Create<FullLicenseInfoView>().TitledWith(caption).OwnedBy(parent);
                    break;
                case LicenseType.Trial:
                    win = Create<TrialLicenseInfoView>().TitledWith(caption).OwnedBy(parent);
                    break;
            }

            return null != win ? win.ShowDialog() : null;
        }

        public void Show<TView>(string caption, Window parent) where TView : Window {
            Create<TView>().TitledWith(caption).OwnedBy(parent).ShowDialog();
        }

        private bool? ShowDialog()
        {
            return ViewBeingCreated.ShowDialog();
        }

        private Window ViewBeingCreated { get; set; }

        private UIWindowDialogService Create<TView>() where TView : Window {
            ViewBeingCreated = ObjectFactory.GetInstance<TView>();
            var viewModelTypeName = ViewBeingCreated.GetType().ToString().Replace("View", "ViewModel");
            var viewModelType = Type.GetType(viewModelTypeName);
            var viewModel = ObjectFactory.With(ViewBeingCreated).GetInstance(viewModelType);
            ViewBeingCreated.DataContext = viewModel;
            return this;
        }

        private UIWindowDialogService TitledWith(string title) {
            ViewBeingCreated.Title = title;
            return this;
        }

        private UIWindowDialogService OwnedBy(Window parent) {
            ViewBeingCreated.Owner = parent;
            return this;
        }
   }
}