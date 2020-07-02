using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DS4Windows;

namespace DS4WinWPF.DraculaView.Views
{
    /// <summary>
    /// 
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private void StartStopBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangeService();
        }

        private void ChangeService()
        {
            //StartStopButton.IsEnabled = false;
            App root = Application.Current as App;
            //Tester service = root.rootHubtest;
            Task.Run(() =>
            {
                ControlService service = App.rootHub;
                if (service.running)
                    service.Stop();
                else
                    service.Start();
            });
        }
    }
}
