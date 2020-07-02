using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using DS4WinWPF.DraculaView.MVVMHandler;
using JetBrains.Annotations;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using PropertyChanged;

namespace DS4WinWPF.DraculaView.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region private Member

        private ICommand _openMenu;
        private ICommand _itemClick;
        private ICommand _optionItemClick;

        private HamburgerMenuItemCollection _menuItems;
        private HamburgerMenuItemCollection _menuOptionItems;

        public bool IsPaneOpened { get; set; }
        public int SelectedIndex { get; set; }

        #endregion

        public MainViewModel()
        {
            CreateMenuItems();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnCustomPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Hamburger Menu

        private void CreateMenuItems()
        {
            MenuItems = new HamburgerMenuItemCollection
            {
                new HamburgerMenuIconItem()
                {
                    Icon = new PackIconMaterial() {Kind = PackIconMaterialKind.Home},
                    Label = "Home",
                    ToolTip = "The Home view.",
                    Tag = new HomeViewModel()
                }
            };

            MenuOptionItems = new HamburgerMenuItemCollection
            {
            };
        }

        [DoNotNotify]
        public HamburgerMenuItemCollection MenuItems
        {
            get => _menuItems;
            set
            {
                if (Equals(value, _menuItems)) return;
                _menuItems = value;
                OnCustomPropertyChanged();
            }
        }

        [DoNotNotify]
        public HamburgerMenuItemCollection MenuOptionItems
        {
            get { return _menuOptionItems; }
            set
            {
                if (Equals(value, _menuOptionItems)) return;
                _menuOptionItems = value;
                OnCustomPropertyChanged();
            }
        }

        public ICommand OpenMenu
        {
            get
            {
                if (_openMenu == null)
                {
                    _openMenu = new RelayCommand(
                        param => SaveOpenMenuCommand(),
                        param => CanSaveOpenMenuCommand()
                    );
                }

                return _openMenu;
            }
        }

        private bool CanSaveOpenMenuCommand()
        {
            return true;
        }

        private void SaveOpenMenuCommand()
        {
            IsPaneOpened = !IsPaneOpened;
        }

        public ICommand ItemClick
        {
            get
            {
                if (_itemClick == null)
                {
                    _itemClick = new RelayCommand<int>(
                        param => ItemClickCommand(),
                        param => CanItemClickCommand()
                    );
                }

                return _itemClick;
            }
        }

        private bool CanItemClickCommand()
        {
            return true;
        }

        private void ItemClickCommand()
        {
            IsPaneOpened = false;
        }

        public ICommand OptionItemClick
        {
            get
            {
                if (_optionItemClick == null)
                {
                    _optionItemClick = new RelayCommand<int>(
                        param => OptionItemClickCommand(),
                        param => CanOptionItemClickCommand()
                    );
                }

                return _optionItemClick;
            }
        }

        private bool CanOptionItemClickCommand()
        {
            return true;
        }

        private void OptionItemClickCommand()
        {
            IsPaneOpened = false;
        }

        #endregion
    }
}
