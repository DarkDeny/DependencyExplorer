using System;

namespace DependencyExplorer.Model {
    public enum Location {
        Unknown,
        GAC,
        SameDir,
    }

    public class AssemblyModel {
        public AssemblyModel(string name, Location location) {
            Location = location;

            DisplayName = String.Format("{0} {1}", this.Location, name);
            Name = name;
        }

        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public Location Location { get; set; }
    }
}
