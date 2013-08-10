using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using DependencyVisualizer.Commands;
using DependencyVisualizer.Model;

namespace DependencyExplorer.ViewModel
{
    public class AssemblyTreeItemViewModel: ViewModelBase
    {
        private readonly AssemblyTreeItemViewModel _parent;
        private readonly AssemblyModel _model;

        public AssemblyTreeItemViewModel(AssemblyTreeItemViewModel parent, AssemblyModel model)
        {
            _parent = parent;
            _model = model;

            References = new ObservableCollection<AssemblyTreeItemViewModel>();
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _IsExpanded;
        public bool IsExpanded
        {
            get
            {
                return _IsExpanded;
            }
            set
            {
                _IsExpanded = value;
                OnPropertyChanged();
                // Expand all the way up to the root.
                if (_IsExpanded && _parent != null)
                {
                    _parent.IsExpanded = true;
                }
            }
        }

        public ICommand ShowInTreeCommand
        {
            get
            {
                return new DelegateCommand(p => true,
                    () =>
                    {
                        this.IsExpanded = true;
                        this.IsSelected = true;
                    });
            }
        }

        public Brush ColorState
        {
            get
            {
                switch (_model.Location)
                {
                    case Location.SameDir:
                        return new SolidColorBrush(Color.FromRgb(0, 155, 0));
                    default:
                        return new SolidColorBrush(Color.FromRgb(155, 0, 0));
                }
            }
        }

        public ObservableCollection<AssemblyTreeItemViewModel> References { get; private set; }

        public string Name { get { return _model.Name; } }

        public void SortReferences()
        {
            var items = References.OrderByDescending(r => r._model.Location);
            References = new ObservableCollection<AssemblyTreeItemViewModel>(items);
            this.OnPropertyChanged("References");
        }
    }
}
