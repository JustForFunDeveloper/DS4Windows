using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DS4Windows;
using DS4WinWPF.DraculaView.MVVMHandler;
using DS4WinWPF.DraculaView.Views.ChildWindows;
using DS4WinWPF.DraculaView.Views.Controls;
using DS4WinWPF.DS4Forms.ViewModels;
using MahApps.Metro.IconPacks;

namespace DS4WinWPF.DraculaView.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private ICommand _startStop;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DsController> ControllerCollection { get; set; }
        public PackIconMaterialKind StartStopIcon { get; set; }
        public string StartStopText { get; set; }
        public bool IsStartStopBtnEnabled { get; set; }

        public ICommand StartStop
        {
            get
            {
                if (_startStop == null)
                    _startStop = new RelayCommand(
                        param => StartStopCommand(),
                        param => CanStartStopCommand()
                    );
                return _startStop;
            }
        }

        private bool CanStartStopCommand()
        {
            return true;
        }

        private void StartStopCommand()
        {
            ChangeService();
        }

        private async void ChangeService()
        {
            IsStartStopBtnEnabled = false;
            //App root = Application.Current as App;

            await Task.Run(() =>
            {
                ControlService service = App.rootHub;

                if (service.running)
                    service.Stop();
                else
                    service.Start();
            });
        }

        public HomeViewModel()
        {
            //InitialiseProperties();
            ControllerCollection = new ObservableCollection<DsController>();
            Mediator.Register(MediatorStrings.UpdateController, OnUpdateController);
            Mediator.Register(MediatorStrings.StartStopBtnText, OnStartStopBtnTextChanged);

            DsController dsController = new DsController()
            {
                IsCharging = true,
                ConnectionType = ConnectionType.BT,
                BatteryChargeLevel = 20,
                ControllerId = "12:34:56:78:9A",
                SelectedProfile = "ThisProfile"
            };
            dsController.ControllerClick += DsControllerOnClick;
            dsController.ProfileClick += DsProfileOnClick;
            ControllerCollection.Add(dsController);

            OnStartStopBtnTextChanged(App.rootHub.running);
        }

        private void DsControllerOnClick(object sender, RoutedEventArgs e)
        {
            var controllerData = (e.OriginalSource as DsController)?.ControllerId;
            EditProfileChildWindow.ChildWindowData data;
            data.Id = controllerData;
            Mediator.NotifyColleagues(MediatorStrings.EditProfileWindowCalled, data);
        }

        private void DsProfileOnClick(object sender, RoutedEventArgs e)
        {
            var controllerData = (e.OriginalSource as DsController)?.ControllerId;
            DsController dsController = ControllerCollection.FirstOrDefault(controller => controller.ControllerId == controllerData);

            Mediator.NotifyColleagues(MediatorStrings.ProfileWindowCalled, dsController);
        }

        private void OnUpdateController(object obj)
        {
            try
            {
                if (obj is IList newItems)
                {
                    foreach (CompositeDeviceModel item in newItems)
                    {
                        bool foundNewItem = true;
                        foreach (var dsController in ControllerCollection)
                        {
                            if (dsController.ControllerId.Equals(item.IdText))
                            {
                                dsController.BatteryChargeLevel = item.BatteryChargeLevel;
                                dsController.IsCharging = item.BatteryCharging;
                                item.Device.Removal += OnDeviceRemoval;
                                foundNewItem = false;
                                break;
                            }
                        }

                        if (foundNewItem)
                        {
                            var newController = new DsController()
                            {
                                ControllerId = item.IdText,
                                BatteryChargeLevel = item.BatteryChargeLevel,
                                IsCharging = item.BatteryCharging,
                                ConnectionType = item.ConnectionType,
                                SelectedProfile = item.SelectedProfile
                            };

                            newController.ControllerClick += DsControllerOnClick;
                            newController.ProfileClick += DsProfileOnClick;

                            ControllerCollection.Add(newController);
                        }
                    }

                    var toRemove = ControllerCollection.Where(
                        item => newItems.Cast<CompositeDeviceModel>().All(item2 => item2.IdText != item.ControllerId));

                    foreach (var dsController in toRemove)
                    {
                        ControllerCollection.Remove(dsController);
                    }
                }
                else
                {
                    ControllerCollection.Clear();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnDeviceRemoval(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnStartStopBtnTextChanged(object obj)
        {
            var isRunning = (bool)obj;

            if (isRunning)
            {
                StartStopText = Translations.Strings.StartText;
                StartStopIcon = PackIconMaterialKind.FlashOff;
            }
            else
            {
                StartStopText = Translations.Strings.StopText;
                StartStopIcon = PackIconMaterialKind.Flash;
            }

            IsStartStopBtnEnabled = true;
        }
    }
}
