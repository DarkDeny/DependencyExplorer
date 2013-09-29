using System;
using System.Windows;
using DependencyExplorer.View.Licensing;
using Licensing.Model;
using StructureMap;

namespace DependencyExplorer.Infrastructure {
    public class UIWindowDialogService : IUIWindowDialogService {
        public bool? ShowLicenseDialog(string title, Window parent) {
            Window win = null;
            switch (LicenseManager.Instance.LicenseInfo.LicenseType) {
                case LicenseType.Full:
                    win = Create<FullLicenseInfoView>().With(title).And(parent);
                    break;
                case LicenseType.Trial:
                    win = Create<TrialLicenseInfoView>().With(title).And(parent);
                    break;
            }

            return null != win ? win.ShowDialog() : null;
        }

        public void Show<TView>(string title, Window parent) where TView : Window {
            Create<TView>().With(title).And(parent).ShowDialog();
        }

        private Window ViewBeingCreated { get; set; }

        private UIWindowDialogService Create<TView>() where TView : Window {
            ViewBeingCreated = ObjectFactory.GetInstance<TView>();
            var viewModelTypeName = ViewBeingCreated.GetType().ToString().Replace("View", "ViewModel");
            var viewModelType = Type.GetType(viewModelTypeName);
            var viewModel = ObjectFactory.GetInstance(viewModelType);
            ViewBeingCreated.DataContext = viewModel;
            return this;
        }

        private UIWindowDialogService With(string title) {
            ViewBeingCreated.Title = title;
            return this;
        }

        private Window And(Window parent) {
            ViewBeingCreated.Owner = parent;
            return ViewBeingCreated;
        }
   }
}