using System.Diagnostics;
using System.Windows.Navigation;

namespace DependencyExplorer.View.Controls {
    /// <summary>
    /// Interaction logic for SendFeedbackUserControl.xaml
    /// </summary>
    public partial class SendFeedbackUserControl {
        public SendFeedbackUserControl() {
            InitializeComponent();
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
