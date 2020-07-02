using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DS4Windows;

namespace DS4WinWPF.DraculaView.Views.Controls
{
    /// <summary>
    /// Interaction logic for DsController.xaml
    /// </summary>
    public partial class DsController : UserControl
    {
        #region Properties

        public bool IsCharging
        {
            get => (bool)GetValue(IsChargingProperty);
            set => SetValue(IsChargingProperty, value);
        }

        public static readonly DependencyProperty IsChargingProperty =
            DependencyProperty.Register("IsCharging", typeof(bool),
                typeof(DsController), new PropertyMetadata());

        public string ControllerId
        {
            get => (string)GetValue(ControllerIdProperty);
            set => SetValue(ControllerIdProperty, value);
        }

        public static readonly DependencyProperty ControllerIdProperty =
            DependencyProperty.Register("ControllerId", typeof(string),
                typeof(DsController), new PropertyMetadata(""));

        public double BatteryChargeLevel
        {
            get => (double)GetValue(BatteryChargeLevelProperty);
            set
            {
                SetValue(BatteryChargeLevelProperty, value);
                BatteryLevel.Text = $"{value} %";
            }
        }

        public static readonly DependencyProperty BatteryChargeLevelProperty =
            DependencyProperty.Register("BatteryChargeLevel", typeof(double),
                typeof(DsController), new PropertyMetadata());

        public ConnectionType ConnectionType
        {
            get => (ConnectionType)GetValue(ConnectionTypeProperty);
            set => SetValue(ConnectionTypeProperty, value);
        }

        public static readonly DependencyProperty ConnectionTypeProperty =
            DependencyProperty.Register("ConnectionType", typeof(ConnectionType),
                typeof(DsController), new PropertyMetadata());

        public string SelectedProfile
        {
            get => (string)GetValue(SelectedProfileProperty);
            set => SetValue(SelectedProfileProperty, value);
        }

        public static readonly DependencyProperty SelectedProfileProperty =
            DependencyProperty.Register("SelectedProfile", typeof(string),
                typeof(DsController), new PropertyMetadata(""));

        #endregion


        #region Events

        public static readonly RoutedEvent ControllerOnClickEvent = EventManager.RegisterRoutedEvent(
            "ControllerClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DsController));

        public event RoutedEventHandler ControllerClick
        {
            add { AddHandler(ControllerOnClickEvent, value); }
            remove { RemoveHandler(ControllerOnClickEvent, value); }
        }

        public static readonly RoutedEvent ProfileOnClickEvent = EventManager.RegisterRoutedEvent(
            "ProfileClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DsController));

        public event RoutedEventHandler ProfileClick
        {
            add { AddHandler(ProfileOnClickEvent, value); }
            remove { RemoveHandler(ProfileOnClickEvent, value); }
        }

        #endregion

        public DsController()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void DsController_OnClick(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(DsController.ControllerOnClickEvent);
            RaiseEvent(newEventArgs);
        }

        private void Profile_OnClick(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs newEventArgs = new RoutedEventArgs(DsController.ProfileOnClickEvent);
            RaiseEvent(newEventArgs);
        }
    }
}
