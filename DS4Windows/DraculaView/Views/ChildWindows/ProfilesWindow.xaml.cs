using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DS4WinWPF.DraculaView.Views.Controls;
using MahApps.Metro.SimpleChildWindow;

namespace DS4WinWPF.DraculaView.Views.ChildWindows
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfilesWindow : ChildWindow
    {
        private ObservableCollection<ProfileControl> _profileControlCollection;
        private ObservableCollection<ProfileControl> _autoProfileControlCollection;

        public struct ChildWindowData
        {
            public string Id;
        }

        public ProfilesWindow(DsController data)
        {
            InitializeComponent();
            _profileControlCollection = new ObservableCollection<ProfileControl>();
            for (int i = 0; i < 7; i++)
            {
                ProfileControl profile = new ProfileControl($"Profile {i}");
                if (i == 0)
                {
                    profile.IsActiveProfile = true;
                    profile.ProfileName = data.SelectedProfile;
                }

                profile.ProfileClick += OnProfileClick;
                profile.EditProfileClick += OnEditProfileClick;
                profile.DuplicateProfileClick += OnDuplicateProfileClick;
                profile.DeleteProfileClick += OnDeleteProfileClick;
                _profileControlCollection.Add(profile);
            }
            var emptyProfile = new ProfileControl(true);
            emptyProfile.EmptyProfileClick += OnEmptyProfileClick;
            _profileControlCollection.Add(emptyProfile);

            _autoProfileControlCollection = new ObservableCollection<ProfileControl>();

            for (int i = 0; i < 7; i++)
            {
                ProfileControl autoProfile = new ProfileControl($"Profile 2nd {i}");
                if (i == 0)
                {
                    autoProfile.IsActiveProfile = true;
                }

                autoProfile.ProfileClick += OnAutoProfileClick;
                autoProfile.EditProfileClick += OnAutoEditProfileClick;
                autoProfile.DuplicateProfileClick += OnAutoDuplicateProfileClick;
                autoProfile.DeleteProfileClick += OnAutoDeleteProfileClick;
                _autoProfileControlCollection.Add(autoProfile);
            }
            var emptyAutoProfile = new ProfileControl(true);
            emptyAutoProfile.EmptyProfileClick += OnEmptyAutoProfileClick;
            _autoProfileControlCollection.Add(emptyAutoProfile);

            ProfileItemsControl.ItemsSource = _profileControlCollection;
            AutoProfileControl.ItemsSource = _autoProfileControlCollection;
        }

        private void OnProfileClick(object sender, RoutedEventArgs e)
        {
            ProfileButtonArgs args = (ProfileButtonArgs)e;
            SetCurrentProfile(_profileControlCollection, ProfileScroll, args.ProfileName);
        }

        private void OnAutoProfileClick(object sender, RoutedEventArgs e)
        {
            ProfileButtonArgs args = (ProfileButtonArgs)e;
            SetCurrentProfile(_autoProfileControlCollection, AutoProfileScroll, args.ProfileName);
        }

        private void SetCurrentProfile(ObservableCollection<ProfileControl> collection, ScrollViewer scrollViewer,
            string profileName)
        {
            ProfileControl currentProfile = collection.FirstOrDefault(profile => profile.ProfileName == profileName);
            var currentIndex = collection.IndexOf(currentProfile);

            if (currentProfile != null)
            {
                if (!currentProfile.IsActiveProfile && collection.Count > 1)
                {
                    currentProfile.IsActiveProfile = true;
                    collection[0].IsActiveProfile = false;
                    collection.Move(currentIndex, 0);
                    scrollViewer.ScrollToLeftEnd();
                }
            }
        }

        private void OnEmptyProfileClick(object sender, RoutedEventArgs e)
        {

        }

        private void OnEmptyAutoProfileClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnEditProfileClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnAutoEditProfileClick(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnDuplicateProfileClick(object sender, RoutedEventArgs e)
        {
            ProfileButtonArgs args = (ProfileButtonArgs)e;
            DuplicateProfile(_profileControlCollection, ProfileScroll, args.ProfileName, true);
        }

        private void OnAutoDuplicateProfileClick(object sender, RoutedEventArgs e)
        {
            ProfileButtonArgs args = (ProfileButtonArgs)e;
            DuplicateProfile(_autoProfileControlCollection, AutoProfileScroll, args.ProfileName, false);
        }

        private void DuplicateProfile(ObservableCollection<ProfileControl> collection, ScrollViewer scrollViewer,
            string profileName, bool isProfile)
        {
            ProfileControl currentProfile = collection.FirstOrDefault(profile => profile.ProfileName == profileName);
            ProfileControl newProfile;
            if (currentProfile != null)
            {
                newProfile = new ProfileControl();
                newProfile.ProfileName = currentProfile.ProfileName + "_copy";

                if (collection.FirstOrDefault(profile => profile.ProfileName == newProfile.ProfileName) != null)
                {
                    // TODO: Perhaps show error?
                    return;
                }

                if (isProfile)
                {
                    newProfile.ProfileClick += OnProfileClick;
                    newProfile.EditProfileClick += OnEditProfileClick;
                    newProfile.DuplicateProfileClick += OnDuplicateProfileClick;
                    newProfile.DeleteProfileClick += OnDeleteProfileClick;
                }
                else
                {
                    newProfile.ProfileClick += OnAutoProfileClick;
                    newProfile.EditProfileClick += OnAutoEditProfileClick;
                    newProfile.DuplicateProfileClick += OnAutoDuplicateProfileClick;
                    newProfile.DeleteProfileClick += OnAutoDeleteProfileClick;
                }

                collection.Add(newProfile);
                collection.Move(collection.Count - 2, collection.Count - 1);
                scrollViewer.ScrollToRightEnd();
            }
        }

        private void OnDeleteProfileClick(object sender, RoutedEventArgs e)
        {
            ProfileButtonArgs args = (ProfileButtonArgs)e;
            DeleteProfile(_profileControlCollection, args.ProfileName);
        }

        private void OnAutoDeleteProfileClick(object sender, RoutedEventArgs e)
        {
            ProfileButtonArgs args = (ProfileButtonArgs)e;
            DeleteProfile(_autoProfileControlCollection, args.ProfileName);
        }

        private void DeleteProfile(ObservableCollection<ProfileControl> collection, string profileName)
        {
            ProfileControl currentProfile = collection.FirstOrDefault(profile => profile.ProfileName == profileName);
            if (currentProfile != null) collection.Remove(currentProfile);
        }

        private void CloseFirst_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ScrollViewer_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                ProfileScroll.ScrollToHorizontalOffset(ProfileScroll.ContentHorizontalOffset - scHBar1.SmallChange);
            else
                ProfileScroll.ScrollToHorizontalOffset(ProfileScroll.ContentHorizontalOffset + scHBar1.SmallChange);
            e.Handled = true;
        }

        private void HScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            AutoProfileScroll.ScrollToHorizontalOffset(e.NewValue);
        }

        private void HScrollBar_Scroll1(object sender, ScrollEventArgs e)
        {
            ProfileScroll.ScrollToHorizontalOffset(e.NewValue);
        }

        private void AutoProfileControl_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                AutoProfileScroll.ScrollToHorizontalOffset(AutoProfileScroll.ContentHorizontalOffset - scHBar.SmallChange);
            else
                AutoProfileScroll.ScrollToHorizontalOffset(AutoProfileScroll.ContentHorizontalOffset + scHBar.SmallChange);
            e.Handled = true;
        }
    }
}
