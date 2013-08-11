using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using DependencyVisualizer.Commands;
using DependencyVisualizer.Model;

namespace DependencyExplorer.ViewModel
{
    public class DependencyViewModel : ViewModelBase
    {
        // TODO: make tree filterable by selected item in the left list 
        // TODO: save results of analysis, so that user can make diffs between versions of solution
        // TODO: set background of selected item to b0d8ff or afd7ff or d6d6d6
        // TODO: make analyze run in background with status messages on how many assemblies processed
        public DependencyViewModel()
        {
            Assemblies = new ObservableCollection<AssemblyTreeItemViewModel>();
        }

        private string _selectedFile;
        public string SelectedFile
        {
            get
            {
                return _selectedFile;
            }
            set
            {
                if (value != _selectedFile)
                {
                    _selectedFile = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SelectFileCommand
        {
            get
            {
                return new DelegateCommand(
                    p => String.IsNullOrEmpty(SelectedFile),
                    () =>
                    {
                        var dialog = new Microsoft.Win32.OpenFileDialog();
                        var result = dialog.ShowDialog();
                        if (true == result)
                        {
                            SelectedFile = dialog.FileName;
                            Analyze();
                        }
                    });
            }
        }

        public ObservableCollection<AssemblyTreeItemViewModel> Assemblies { get; private set; }

        public IEnumerable<AssemblyTreeItemViewModel> AnalyzedAssemblies
        {
            get
            {
                var list = new List<AssemblyTreeItemViewModel>();
                list.AddRange(_analyzedAssemblies);
                list.AddRange(_notFoundAssemblies);

                return list.OrderBy(asm => asm.Name).ToList();
            }
        }

        public string Title
        {
            get
            {
                return String.Format("{0} v.{1}", App.Name, App.Version);
            }
        }

        private readonly HashSet<AssemblyTreeItemViewModel> _analyzedAssemblies = new HashSet<AssemblyTreeItemViewModel>();
        private readonly HashSet<AssemblyTreeItemViewModel> _notFoundAssemblies = new HashSet<AssemblyTreeItemViewModel>();

        private void Analyze(Assembly assembly, AssemblyTreeItemViewModel parentViewModel, string rootFolder)
        {
            try
            {
                var references = assembly.GetReferencedAssemblies();
                foreach (var assemblyName in references.OrderBy(assmName => assmName.Name))
                {
                    AssemblyTreeItemViewModel referencedAssemblyModel;

                    var possibleFileNames = new[] { assemblyName.Name + ".dll", assemblyName.Name + ".exe" };
                    int foundAssemblies = 0;
                    foreach (var fileName in possibleFileNames)
                    {
                        var fullPath = Path.Combine(rootFolder, fileName);
                        if (File.Exists(fullPath))
                        {
                            try
                            {
                                var referencedAssembly = Assembly.ReflectionOnlyLoadFrom(fullPath);
                                // check for match
                                if (AssemblyName.ReferenceMatchesDefinition(assemblyName, referencedAssembly.GetName()))
                                {
                                    var file = Path.GetFileNameWithoutExtension(referencedAssembly.Location);
                                    referencedAssemblyModel = new AssemblyTreeItemViewModel(
                                        parentViewModel, new AssemblyModel(file, Location.SameDir));
                                    parentViewModel.References.Add(referencedAssemblyModel);

                                    var referencedAssemblyName = referencedAssembly.GetName().Name;
                                    Debug.WriteLine("Starting to analyze " + referencedAssemblyName);
                                    if (_analyzedAssemblies.Any(assm => assm.Name == referencedAssemblyName))
                                    {
                                        continue;
                                    }

                                    _analyzedAssemblies.Add(referencedAssemblyModel);
                                    this.Analyze(referencedAssembly, referencedAssemblyModel, rootFolder);

                                    //AppDomain dom = AppDomain.CreateDomain("some");  
                                    //dom.
                                    
                                    foundAssemblies++;
                                }
                            }
                            catch (Exception ex)
                            {
                                // TODO: logging =)
                                MessageBox.Show(ex.ToString());
                            }
                        }
                    }

                    if (this._analyzedAssemblies.All(assm => assm.Name != assemblyName.Name)
                        && 0 == foundAssemblies)
                    {
                        // We have a reference that is not in the same dir as base assembly. It is either in GAC or it's location is unknown
                        // Currently we are not checking GAC, so Location is Unknown
                        referencedAssemblyModel = new AssemblyTreeItemViewModel(parentViewModel,
                            new AssemblyModel(assemblyName.Name, Location.Unknown));
                        parentViewModel.References.Add(referencedAssemblyModel);
                        if (this._notFoundAssemblies.All(assm => assm.Name != referencedAssemblyModel.Name))
                        {
                            _notFoundAssemblies.Add(referencedAssemblyModel);
                        }
                    }
                }
                parentViewModel.SortReferences();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File not found. Please select another file to analyze.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error during loading file you selected. Error text is {0}.", ex.Message));
            }
        }

        private void Analyze()
        {
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

            OnPropertyChanged("AnalyzedAssemblies");
            OnPropertyChanged("Assemblies");
        }
    }
}
