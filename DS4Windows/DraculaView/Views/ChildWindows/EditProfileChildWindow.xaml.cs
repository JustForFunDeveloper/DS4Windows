using System.Windows;
using MahApps.Metro.SimpleChildWindow;

namespace DS4WinWPF.DraculaView.Views.ChildWindows
{
    /// <summary>
    /// Interaction logic for EditProfileChildWindow.xaml
    /// </summary>
    public partial class EditProfileChildWindow : ChildWindow
    {
        public struct ChildWindowData
        {
            public string Id;
        }

        public EditProfileChildWindow(ChildWindowData data)
        {
            InitializeComponent();
            this.TextBlock.Text = data.Id;
        }

        private void CloseFirst_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
