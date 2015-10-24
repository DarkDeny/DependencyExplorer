using System;
using System.Windows;
using StructureMap;

namespace DependencyExplorer.Infrastructure {
    public class UIWindowDialogService : IUIWindowDialogService {
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