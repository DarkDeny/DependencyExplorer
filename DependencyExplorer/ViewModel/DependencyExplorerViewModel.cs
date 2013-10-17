using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using DependencyExplorer.Commands;
using DependencyExplorer.Infrastructure;
using DependencyExplorer.Model;
using DependencyExplorer.Properties;

using Licensing.Model;

namespace DependencyExplorer.ViewModel {
    public class DependencyExplorerViewModel : WindowViewModelBase {
        public DependencyExplorerViewModel(IUIWindowDialogService anUIService, Window window)
            : base(anUIService, window) {
            Assemblies = new ObservableCollection<AssemblyTreeItemViewModel>();
            UIService = anUIService;
            CreateCommands();
        }

        private void CreateCommands() {
            SelectFileCommand = new DelegateCommand(
                canExecute => LicenseManager.Instance.Status == LicenseStatus.Valid,
                () => {
                    var dialog = new Microsoft.Win32.OpenFileDialog();
                    var result = dialog.ShowDialog();
                    if (true == result) {
                        SelectedFile = dialog.FileName;
                        Analyze();
                    }
                });

            ShowLicenseInfoCommand = new DelegateCommand(
                canExecute => true,
                () => {
                    UIService.ShowLicenseDialog(Resources.LicenseInformationWindowTitle, Window);
                    SelectFileCommand.FireCanExecuteChanged();
                    OnPropertyChanged("NoLicenseMessageVisibility");
                });
        }

        private IUIWindowDialogService UIService { get; set; }

        private string _SelectedFile;
        public virtual string SelectedFile {
            get { return _SelectedFile; }
            protected set {
                _SelectedFile = value;
                OnPropertyChanged();
            }
        }

        public ICommand ExitCommand {
            get {
                return new DelegateCommand(canExecute => true, () => Application.Current.Shutdown());
            }
        }

        public DelegateCommand ShowLicenseInfoCommand { get; private set; }

        public DelegateCommand SelectFileCommand { get; private set; }

        public virtual ObservableCollection<AssemblyTreeItemViewModel> Assemblies { get; private set; }

        public IEnumerable<AssemblyTreeItemViewModel> AllAnalyzedAssemblies {
            get {
                var list = new List<AssemblyTreeItemViewModel>();
                list.AddRange(AnalyzedAssemblies);
                list.AddRange(NotFoundAssemblies);

                return list.OrderBy(asm => asm.Name).ToList();
            }
        }

        public string Title {
            get {
                return String.Format("{0} v.{1}", App.ProductName, App.VersionString);
            }
        }

        public Visibility NoLicenseMessageVisibility {
            get {
                return LicenseManager.Instance.Status == LicenseStatus.Valid ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        protected readonly HashSet<AssemblyTreeItemViewModel> AnalyzedAssemblies = new HashSet<AssemblyTreeItemViewModel>();
        protected readonly HashSet<AssemblyTreeItemViewModel> NotFoundAssemblies = new HashSet<AssemblyTreeItemViewModel>();

        private void Analyze(Assembly assembly, AssemblyTreeItemViewModel parentViewModel, string rootFolder) {
            try {
                var references = assembly.GetReferencedAssemblies();
                foreach (var assemblyName in references.OrderBy(assmName => assmName.Name)) {
                    AssemblyTreeItemViewModel referencedAssemblyModel;

                    var possibleFileNames = new[] { assemblyName.Name + ".dll", assemblyName.Name + ".exe" };
                    int foundAssemblies = 0;
                    foreach (var fileName in possibleFileNames) {
                        var fullPath = Path.Combine(rootFolder, fileName);
                        if (File.Exists(fullPath)) {
                            try {
                                var referencedAssembly = Assembly.ReflectionOnlyLoadFrom(fullPath);
                                // check for match
                                if (AssemblyName.ReferenceMatchesDefinition(assemblyName, referencedAssembly.GetName())) {
                                    var file = Path.GetFileNameWithoutExtension(referencedAssembly.Location);
                                    referencedAssemblyModel = new AssemblyTreeItemViewModel(
                                        parentViewModel, new AssemblyModel(file, Location.SameDir));
                                    parentViewModel.References.Add(referencedAssemblyModel);

                                    var referencedAssemblyName = referencedAssembly.GetName().Name;
                                    Debug.WriteLine("Starting to analyze " + referencedAssemblyName);
                                    if (AnalyzedAssemblies.Any(assm => assm.Name == referencedAssemblyName)) {
                                        continue;
                                    }

                                    AnalyzedAssemblies.Add(referencedAssemblyModel);
                                    Analyze(referencedAssembly, referencedAssemblyModel, rootFolder);

                                    foundAssemblies++;
                                }
                            } catch (Exception ex) {
                                // TODO: logging =)
                                MessageBox.Show(ex.ToString());
                            }
                        }
                    }

                    if (AnalyzedAssemblies.All(assm => assm.Name != assemblyName.Name)
                        && 0 == foundAssemblies) {
                        // We have a reference that is not in the same dir as the base assembly. It is either in GAC or it's location is unknown
                        // Currently we are not checking GAC, so Location is Unknown
                        referencedAssemblyModel = new AssemblyTreeItemViewModel(parentViewModel,
                            new AssemblyModel(assemblyName.Name, Location.Unknown));
                        parentViewModel.References.Add(referencedAssemblyModel);
                        if (NotFoundAssemblies.All(assm => assm.Name != referencedAssemblyModel.Name)) {
                            NotFoundAssemblies.Add(referencedAssemblyModel);
                        }
                    }
                }
                parentViewModel.SortReferences();
            } catch (FileNotFoundException) {
                MessageBox.Show("File not found. Please select another file to analyze.");
            } catch (Exception ex) {
                MessageBox.Show(String.Format("Error during loading file you selected. Error text is {0}.", ex.Message));
            }
        }

        private void Analyze() {
            Assemblies.Clear();

            var directory = Path.GetDirectoryName(SelectedFile);
            var rootAssembly = new AssemblyTreeItemViewModel(null,
                new AssemblyModel(Path.GetFileName(SelectedFile), Location.SameDir));
            // ReSharper disable once AssignNullToNotNullAttribute
            Directory.SetCurrentDirectory(directory);

            var assm = Assembly.ReflectionOnlyLoadFrom(SelectedFile);
            Analyze(assm, rootAssembly, directory);
            rootAssembly.IsExpanded = true;
            Assemblies.Add(rootAssembly);

            OnPropertyChanged(propertyName: "AnalyzedAssemblies");
            OnPropertyChanged(propertyName: "Assemblies");
        }
    }
}
