using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

using DependencyExplorer.Infrastructure;
using DependencyExplorer.Model;

namespace DependencyExplorer.ViewModel.Mocks {
    public class MockDependencyExplorerViewModel : DependencyExplorerViewModel {

        internal class MockUIService : IUIWindowDialogService {
            public bool? ShowLicenseDialog(string caption, Window parentWindow) {
                return false;
            }

            public void Show<TView>(string title, Window parent) where TView : Window { }
        }

        public MockDependencyExplorerViewModel()
            : base(new MockUIService(), new Window()) {
            _MockAssemblies = new ObservableCollection<MockAssemblyTreeItemViewModel>();
            _MockAssemblies.Add(CreateTopLevelAssemblyViewModel(@"DataAccessLayer.dll", Location.SameDir));

            var model1 = new AssemblyModel(@"UIFramework.dll", Location.SameDir);
            var model2 = new AssemblyModel(@"MVVM.dll", Location.Unknown);
            _MockAssemblies.Add(CreateDependentAssemblyViewModel(model2, model1));
        }

        private MockAssemblyTreeItemViewModel CreateDependentAssemblyViewModel(AssemblyModel model1, AssemblyModel model2) {
            var vm1 = new MockAssemblyTreeItemViewModel(null, model1);
            var vm2 = new MockAssemblyTreeItemViewModel(vm1, model2);
            vm2.SetReferences(new [] {vm1});
            return vm2;
        }

        private MockAssemblyTreeItemViewModel CreateTopLevelAssemblyViewModel(string name, Location location) {
            var model = new AssemblyModel(name, location);
            return new MockAssemblyTreeItemViewModel(null, model);
        }

        public override string SelectedFile {
            get {
                return @"d:\Sources\Bin\Product.exe";
            }
            protected set {
                //ignoring
            }
        }

        private readonly ObservableCollection<MockAssemblyTreeItemViewModel> _MockAssemblies;

        public override ObservableCollection<AssemblyTreeItemViewModel> Assemblies {
            get { return new ObservableCollection<AssemblyTreeItemViewModel>(this._MockAssemblies); }
        }
    }

    internal class MockAssemblyTreeItemViewModel : AssemblyTreeItemViewModel {
        public MockAssemblyTreeItemViewModel(AssemblyTreeItemViewModel parent, AssemblyModel model)
            : base(parent, model) {
            
        }

        public void SetReferences(IEnumerable<AssemblyTreeItemViewModel> references) {
            References = new ObservableCollection<AssemblyTreeItemViewModel>(references);
        }
    }
}
