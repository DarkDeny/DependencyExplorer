using System;
using System.Windows;
using DependencyExplorer.View.Licensing;
using StructureMap;

namespace DependencyExplorer.Infrastructure {
    public class UIWindowDialogService : IUIWindowDialogService {
        public bool? ShowLicenseDialog(string caption, Window parent) {
            var win = Create<LicenseInfoView>().TitledWith(caption).OwnedBy(parent);

            return null != win ? win.ShowDialog() : null;
        }

        public void Show<TView>(string caption, Window parent) where TView : Window {
            Create<TView>().TitledWith(caption).OwnedBy(parent).ShowDialog();
        }

        private bool? ShowDialog() {
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