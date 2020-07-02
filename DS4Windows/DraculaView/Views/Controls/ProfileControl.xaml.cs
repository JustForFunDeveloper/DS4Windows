using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DS4WinWPF.DraculaView.Views.Controls
{
    public class ProfileButtonArgs : RoutedEventArgs
    {
        private readonly string _profileName;

        public string ProfileName
        {
            get { return _profileName; }
        }

        public ProfileButtonArgs(RoutedEvent routedEvent, string profileName) : base(routedEvent)
        {
            this._profileName = profileName;
        }
    }

    /// <summary>
    /// Interaction logic for ProfileControl.xaml
    /// </summary>
    public partial class ProfileControl : UserControl
    {
        #region Properties

        public Brush ForeGroundBrush
        {
            get => (Brush)GetValue(ForeGroundBrushProperty);
            set => SetValue(ForeGroundBrushProperty, value);
        }

        public static readonly DependencyProperty ForeGroundBrushProperty =
            DependencyProperty.Register("ForeGroundBrush", typeof(Brush),
                typeof(ProfileControl), new PropertyMetadata());

        public bool IsEmptyProfile
        {
            get => (bool)GetValue(IsEmptyProfileProperty);
            set => SetValue(IsEmptyProfileProperty, value);
        }

        public static readonly DependencyProperty IsEmptyProfileProperty =
            DependencyProperty.Register("IsEmptyProfile", typeof(bool),
                typeof(ProfileControl), new PropertyMetadata());

        public string ProfileName
        {
            get => (string)GetValue(ProfileNameProperty);
            set => SetValue(ProfileNameProperty, value);
        }

        public static readonly DependencyProperty ProfileNameProperty =
            DependencyProperty.Register("ProfileName", typeof(string),
                typeof(ProfileControl), new PropertyMetadata(""));

        public bool IsActiveProfile
        {
            get => (bool)GetValue(IsActiveProfileProperty);
            set
            {
                if (value)
                {
                    ForeGroundBrush = Brushes.White;
                    ProfileButton.ToolTip = "Current Profile";
                }
                else
                {
                    ForeGroundBrush = Brushes.DimGray;
                    ProfileButton.ToolTip = "Set Profile";
                }
                SetValue(IsActiveProfileProperty, value);
            }
        }

        public static readonly DependencyProperty IsActiveProfileProperty =
            DependencyProperty.Register("IsActiveProfile", typeof(bool),
                typeof(ProfileControl), new PropertyMetadata());

        #endregion

        #region Events

        public static readonly RoutedEvent EmptyProfileClickEvent = EventManager.RegisterRoutedEvent(
            "EmptyProfileClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ProfileControl));

        public event RoutedEventHandler EmptyProfileClick
        {
            add { AddHandler(EmptyProfileClickEvent, value); }
            remove { RemoveHandler(EmptyProfileClickEvent, value); }
        }

        public static readonly RoutedEvent ProfileClickEvent = EventManager.RegisterRoutedEvent(
            "ProfileClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ProfileControl));

        public event RoutedEventHandler ProfileClick
        {
            add { AddHandler(ProfileClickEvent, value); }
            remove { RemoveHandler(ProfileClickEvent, value); }
        }

        public static readonly RoutedEvent EditProfileClickEvent = EventManager.RegisterRoutedEvent(
            "EditProfileClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ProfileControl));

        public event RoutedEventHandler EditProfileClick
        {
            add { AddHandler(EditProfileClickEvent, value); }
            remove { RemoveHandler(EditProfileClickEvent, value); }
        }

        public static readonly RoutedEvent DuplicateProfileClickEvent = EventManager.RegisterRoutedEvent(
            "DuplicateProfileClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ProfileControl));

        public event RoutedEventHandler DuplicateProfileClick
        {
            add { AddHandler(DuplicateProfileClickEvent, value); }
            remove { RemoveHandler(DuplicateProfileClickEvent, value); }
        }

        public static readonly RoutedEvent DeleteProfileClickEvent = EventManager.RegisterRoutedEvent(
            "DeleteProfileClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ProfileControl));

        public event RoutedEventHandler DeleteProfileClick
        {
            add { AddHandler(DeleteProfileClickEvent, value); }
            remove { RemoveHandler(DeleteProfileClickEvent, value); }
        }

        #endregion

        public ProfileControl()
        {
            InitializeComponent();
            this.DataContext = this;
            InitializeControl(IsEmptyProfile);
        }

        public ProfileControl(bool isEmptyProfile)
        {
            InitializeComponent();
            this.DataContext = this;
            InitializeControl(isEmptyProfile);
        }

        public ProfileControl(string profileName, bool isEmptyProfile = false)
        {
            InitializeComponent();
            this.DataContext = this;
            IsEmptyProfile = isEmptyProfile;
            ProfileName = profileName;

            if (IsEmptyProfile)
            {
                ProfileButton.ToolTip = "Add Profile";
                DefaultProfileImage.Visibility = Visibility.Hidden;
                DetailsPanel.Visibility = Visibility.Hidden;
                ForeGroundBrush = Brushes.White;
            }
            else
            {
                ProfileButton.ToolTip = "Set Profile";
                AddProfileIcon.Visibility = Visibility.Hidden;
                AddProfileText.Visibility = Visibility.Hidden;
                ForeGroundBrush = Brushes.DimGray;
            }
        }

        public void InitializeControl(bool isEmptyProfile)
        {
            IsEmptyProfile = isEmptyProfile;

            if (IsEmptyProfile)
            {
                ProfileButton.ToolTip = "Add Profile";
                DefaultProfileImage.Visibility = Visibility.Hidden;
                DetailsPanel.Visibility = Visibility.Hidden;
                ForeGroundBrush = Brushes.White;
            }
            else
            {
                ProfileButton.ToolTip = "Set Profile";
                ProfileName = "Name not set";
                AddProfileIcon.Visibility = Visibility.Hidden;
                AddProfileText.Visibility = Visibility.Hidden;
                ForeGroundBrush = Brushes.DimGray;
            }
        }

        private void ProfileButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsEmptyProfile)
            {
                ProfileButtonArgs newEventArgs = new ProfileButtonArgs(ProfileControl.EmptyProfileClickEvent, ProfileName);
                RaiseEvent(newEventArgs);
            }
            else
            {
                ProfileButtonArgs newEventArgs = new ProfileButtonArgs(ProfileControl.ProfileClickEvent, ProfileName);
                RaiseEvent(newEventArgs);
            }
        }

        private void ProfileEdit_OnClick(object sender, RoutedEventArgs e)
        {
            ProfileButtonArgs newEventArgs = new ProfileButtonArgs(ProfileControl.EditProfileClickEvent, ProfileName);
            RaiseEvent(newEventArgs);
        }

        private void ProfileDuplicate_OnClick(object sender, RoutedEventArgs e)
        {
            ProfileButtonArgs newEventArgs = new ProfileButtonArgs(ProfileControl.DuplicateProfileClickEvent, ProfileName);
            RaiseEvent(newEventArgs);
        }

        private void ProfileDelete_OnClick(object sender, RoutedEventArgs e)
        {
            ProfileButtonArgs newEventArgs = new ProfileButtonArgs(ProfileControl.DeleteProfileClickEvent, ProfileName);
            RaiseEvent(newEventArgs);
        }
    }
}
