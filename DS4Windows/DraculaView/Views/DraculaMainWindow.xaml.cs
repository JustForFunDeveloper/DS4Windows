using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DS4Windows;
using DS4WinWPF.DraculaView.MVVMHandler;
using DS4WinWPF.DraculaView.Views.ChildWindows;
using DS4WinWPF.DraculaView.Views.Controls;
using DS4WinWPF.DS4Forms;
using DS4WinWPF.DS4Forms.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using NonFormTimer = System.Timers.Timer;

namespace DS4WinWPF.DraculaView.Views
{
    public partial class DraculaMainWindow : MetroWindow
    {
        private static MetroWindow _mainView;
        private bool _closeMe;

        private ProfileList profileListHolder = new ProfileList();
        private ControllerListViewModel conLvViewModel;

        private TrayIconViewModel trayIconVM;
        private SettingsViewModel settingsWrapVM;

        private AutoProfileHolder autoProfileHolder;
        private NonFormTimer hotkeysTimer;
        private NonFormTimer autoProfilesTimer;
        private AutoProfileChecker autoprofileChecker;
        AutoProfiles autoProfControl = new AutoProfiles();

        private ManagementEventWatcher managementEvWatcher;

        private bool wasrunning = false;
        private bool showAppInTaskbar = false;

        public DraculaMainWindow(ArgumentParser parser)
        {
            InitializeComponent();
            _mainView = this;
            Closing += WindowClosing;
            Closed += WindowClosed;
            StateChanged += WindowStateChanged;
            Mediator.Register(MediatorStrings.EditProfileWindowCalled, EditProfileWindowCalled);
            Mediator.Register(MediatorStrings.ProfileWindowCalled, ProfileWindowCalled);

            //mainWinVM = new MainWindowsViewModel();
            //DataContext = mainWinVM;

            App root = Application.Current as App;
            //settingsWrapVM = new SettingsViewModel();
            //settingsTab.DataContext = settingsWrapVM;
            //logvm = new LogViewModel(App.rootHub);
            //logListView.ItemsSource = logvm.LogItems;
            //logListView.DataContext = logvm;
            //lastMsgLb.DataContext = lastLogMsg;

            profileListHolder.Refresh();
            //profilesListBox.ItemsSource = profileListHolder.ProfileListCol;


            conLvViewModel = new ControllerListViewModel(App.rootHub, profileListHolder);
            //controllerLV.DataContext = conLvViewModel;
            //ChangeControllerPanel();
            //trayIconVM = new TrayIconViewModel(App.rootHub, profileListHolder);
            //notifyIcon.DataContext = trayIconVM;

            if (Global.StartMinimized || parser.Mini)
            {
                WindowState = WindowState.Minimized;
            }

            //bool isElevated = Global.IsAdministrator();
            //if (isElevated)
            //{
            //    uacImg.Visibility = Visibility.Collapsed;
            //}

            //this.Width = Global.FormWidth;
            //this.Height = Global.FormHeight;
            //WindowStartupLocation = WindowStartupLocation.Manual;
            //Left = Global.FormLocationX;
            //Top = Global.FormLocationY;

            autoProfileHolder = autoProfControl.AutoProfileHolder;
            autoProfControl.SetupDataContext(profileListHolder);

            autoprofileChecker = new AutoProfileChecker(autoProfileHolder);

            SetupEvents();

            Thread timerThread = new Thread(() =>
            {
                hotkeysTimer = new NonFormTimer();
                hotkeysTimer.Interval = 20;
                hotkeysTimer.AutoReset = false;

                autoProfilesTimer = new NonFormTimer();
                autoProfilesTimer.Interval = 1000;
                autoProfilesTimer.AutoReset = false;
            });
            timerThread.IsBackground = true;
            timerThread.Priority = ThreadPriority.Lowest;
            timerThread.Start();
            timerThread.Join();
        }

        private async void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel) return;
            e.Cancel = !_closeMe;
            if (_closeMe) return;

            //if (editor != null)
            //{
            //    editor.Close();
            //    e.Cancel = true;
            //    return;
            //}
            //else if (contextclose)
            //{
            //    return;
            //}

            //if (Global.CloseMini)
            //{
            //    WindowState = WindowState.Minimized;
            //    e.Cancel = true;
            //    return;
            //}

            var mySettings = new MetroDialogSettings()
            {
                AnimateShow = true,
                AnimateHide = false,
            };
            var result = await this.ShowMessageAsync(
                Properties.Resources.Confirm,
                Properties.Resources.CloseConfirm,
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            _closeMe = result == MessageDialogResult.Affirmative;
            if (_closeMe) Close();
        }

        public static MetroWindow GetInstance
        {
            get => _mainView;
        }

        private async void EditProfileWindowCalled(object obj)
        {
            var data = obj is EditProfileChildWindow.ChildWindowData ? (EditProfileChildWindow.ChildWindowData)obj : default;
            await this.ShowChildWindowAsync(new EditProfileChildWindow(data), this.RootGrid);
        }

        private async void ProfileWindowCalled(object obj)
        {
            var data = obj is DsController ? (DsController)obj : default;
            await this.ShowChildWindowAsync(new ProfilesWindow(data), this.RootGrid);
        }

        private void SetupEvents()
        {
            App root = Application.Current as App;
            App.rootHub.ServiceStarted += ControlServiceStarted;
            App.rootHub.RunningChanged += ControlServiceChanged;
            App.rootHub.PreServiceStop += PrepareForServiceStop;
            //root.rootHubtest.RunningChanged += ControlServiceChanged;
            conLvViewModel.ControllerCol.CollectionChanged += ControllerCol_CollectionChanged;
            //AppLogger.TrayIconLog += ShowNotification;
            //AppLogger.GuiLog += UpdateLastStatusMessage;
            //logvm.LogItems.CollectionChanged += LogItems_CollectionChanged;
            //App.rootHub.Debug += UpdateLastStatusMessage;
            //trayIconVM.RequestShutdown += TrayIconVM_RequestShutdown;
            //trayIconVM.ProfileSelected += TrayIconVM_ProfileSelected;
            //trayIconVM.RequestMinimize += TrayIconVM_RequestMinimize;
            //trayIconVM.RequestOpen += TrayIconVM_RequestOpen;
            //trayIconVM.RequestServiceChange += TrayIconVM_RequestServiceChange;
            //autoProfControl.AutoDebugChanged += AutoProfControl_AutoDebugChanged;
            //autoprofileChecker.RequestServiceChange += AutoprofileChecker_RequestServiceChange;
            //autoProfileHolder.AutoProfileColl.CollectionChanged += AutoProfileColl_CollectionChanged;
            //autoProfControl.AutoProfVM.AutoProfileSystemChange += AutoProfVM_AutoProfileSystemChange;
            //mainWinVM.FullTabsEnabledChanged += MainWinVM_FullTabsEnabledChanged;

            bool wmiConnected = false;
            WqlEventQuery q = new WqlEventQuery();
            ManagementScope scope = new ManagementScope("root\\CIMV2");
            q.EventClassName = "Win32_PowerManagementEvent";

            try
            {
                scope.Connect();
            }
            catch (COMException) { }

            if (scope.IsConnected)
            {
                wmiConnected = true;
                managementEvWatcher = new ManagementEventWatcher(scope, q);
                managementEvWatcher.EventArrived += PowerEventArrive;
                try
                {
                    managementEvWatcher.Start();
                }
                catch (ManagementException) { wmiConnected = false; }
            }

            if (!wmiConnected)
            {
                AppLogger.LogToGui(@"Could not connect to Windows Management Instrumentation service.
Suspend support not enabled.", true);
            }
        }

        private void ControlServiceStarted(object sender, EventArgs e)
        {
            if (Global.SwipeProfiles)
            {
                ChangeHotkeysStatus(true);
            }

            CheckAutoProfileStatus();
        }

        private void ChangeHotkeysStatus(bool state)
        {
            if (state)
            {
                hotkeysTimer.Elapsed += HotkeysTimer_Elapsed;
                hotkeysTimer.Start();
            }
            else
            {
                hotkeysTimer.Stop();
                hotkeysTimer.Elapsed -= HotkeysTimer_Elapsed;
            }
        }

        private void HotkeysTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            hotkeysTimer.Stop();

            if (Global.SwipeProfiles)
            {
                foreach (CompositeDeviceModel item in conLvViewModel.ControllerCol)
                //for (int i = 0; i < 4; i++)
                {
                    string slide = App.rootHub.TouchpadSlide(item.DevIndex);
                    if (slide == "left")
                    {
                        //int ind = i;
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            if (item.SelectedIndex <= 0)
                            {
                                item.SelectedIndex = item.ProfileListCol.Count - 1;
                            }
                            else
                            {
                                item.SelectedIndex--;
                            }
                        }));
                    }
                    else if (slide == "right")
                    {
                        //int ind = i;
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            if (item.SelectedIndex == (item.ProfileListCol.Count - 1))
                            {
                                item.SelectedIndex = 0;
                            }
                            else
                            {
                                item.SelectedIndex++;
                            }
                        }));
                    }

                    if (slide.Contains("t"))
                    {
                        //int ind = i;
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            string temp = Properties.Resources.UsingProfile.Replace("*number*",
                                (item.DevIndex + 1).ToString()).Replace("*Profile name*", item.SelectedProfile);
                            //ShowHotkeyNotification(temp);
                        }));
                    }
                }
            }

            hotkeysTimer.Start();
        }

        private void CheckAutoProfileStatus()
        {
            int pathCount = autoProfileHolder.AutoProfileColl.Count;
            bool timerEnabled = autoprofileChecker.Running;
            if (pathCount > 0 && !timerEnabled)
            {
                ChangeAutoProfilesStatus(true);
            }
            else if (pathCount == 0 && timerEnabled)
            {
                ChangeAutoProfilesStatus(false);
            }
        }

        private void ChangeAutoProfilesStatus(bool state)
        {
            if (state)
            {
                autoProfilesTimer.Elapsed += AutoProfilesTimer_Elapsed;
                autoProfilesTimer.Start();
                autoprofileChecker.Running = true;
            }
            else
            {
                autoProfilesTimer.Stop();
                autoProfilesTimer.Elapsed -= AutoProfilesTimer_Elapsed;
                autoprofileChecker.Running = false;
            }
        }

        private void AutoProfilesTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            autoProfilesTimer.Stop();
            //Console.WriteLine("Event triggered");
            autoprofileChecker.Process();

            if (autoprofileChecker.Running)
            {
                autoProfilesTimer.Start();
            }
        }

        private void ControlServiceChanged(object sender, EventArgs e)
        {
            //Tester service = sender as Tester;
            ControlService service = sender as ControlService;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                if (service != null && service.running)
                {
                    Mediator.NotifyColleagues(MediatorStrings.StartStopBtnText, false);
                }
                else
                {
                    Mediator.NotifyColleagues(MediatorStrings.StartStopBtnText, true);
                }
            }));
        }

        private void PrepareForServiceStop(object sender, EventArgs e)
        {
            //Dispatcher.BeginInvoke((Action)(() =>
            //{
            //    trayIconVM.ClearContextMenu();
            //}));

            ChangeHotkeysStatus(false);
        }

        private void ControllerCol_CollectionChanged(object sender,
            System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                //ChangeControllerPanel();
                System.Collections.IList newitems = e.NewItems;
                if (newitems != null)
                {
                    foreach (CompositeDeviceModel item in newitems)
                    {
                        item.LightContext = new ContextMenu();
                        item.AddLightContextItems();
                        item.Device.SyncChange += DS4Device_SyncChange;
                        item.RequestColorPicker += Item_RequestColorPicker;
                        //item.LightContext.Items.Add(new MenuItem() { Header = "Use Profile Color", IsChecked = !item.UseCustomColor });
                        //item.LightContext.Items.Add(new MenuItem() { Header = "Use Custom Color", IsChecked = item.UseCustomColor });
                    }
                }

                Mediator.NotifyColleagues(MediatorStrings.UpdateController, newitems);

                //if (App.rootHub.running)
                //    trayIconVM.PopulateContextMenu();
            }));
        }

        private void Item_RequestColorPicker(CompositeDeviceModel sender)
        {
            ColorPickerWindow dialog = new ColorPickerWindow();
            dialog.Owner = this;
            dialog.colorPicker.SelectedColor = sender.CustomLightColor;
            dialog.ColorChanged += (sender2, color) =>
            {
                sender.UpdateCustomLightColor(color);
            };
            dialog.ShowDialog();
        }

        private void DS4Device_SyncChange(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                trayIconVM.PopulateContextMenu();
            }));
        }

        private void PowerEventArrive(object sender, EventArrivedEventArgs e)
        {
            short evType = Convert.ToInt16(e.NewEvent.GetPropertyValue("EventType"));
            switch (evType)
            {
                // Wakeup from Suspend
                case 7:
                    DS4LightBar.shuttingdown = false;
                    App.rootHub.suspending = false;

                    if (wasrunning)
                    {
                        wasrunning = false;
                        Thread.Sleep(16000);
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            // TODO: Hook this up!
                            //StartStopBtn.IsEnabled = false;
                        }));

                        App.rootHub.Start();
                    }

                    break;
                // Entering Suspend
                case 4:
                    DS4LightBar.shuttingdown = true;
                    Program.rootHub.suspending = true;

                    if (App.rootHub.running)
                    {
                        Dispatcher.BeginInvoke((Action)(() =>
                        {
                            // TODO: Hook this up!
                            //StartStopBtn.IsEnabled = false;
                        }));

                        App.rootHub.Stop();
                        wasrunning = true;
                    }

                    break;

                default: break;
            }
        }

        public void LateChecks(ArgumentParser parser)
        {
            Task tempTask = Task.Run(() =>
            {
                CheckDrivers();
                if (!parser.Stop)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        // TODO: Implement this
                        //StartStopBtn.IsEnabled = false;
                    }));
                    Thread.Sleep(500);
                    App.rootHub.Start();
                    //root.rootHubtest.Start();
                }

                //UpdateTheUpdater();
            });

            Util.LogAssistBackgroundTask(tempTask);

            tempTask = Task.Delay(100).ContinueWith((t) =>
            {
                int checkwhen = Global.CheckWhen;
                if (checkwhen > 0 && DateTime.Now >= Global.LastChecked + TimeSpan.FromHours(checkwhen))
                {
                    //DownloadUpstreamVersionInfo();
                    //Check_Version();

                    Global.LastChecked = DateTime.Now;
                }
            });
            Util.LogAssistBackgroundTask(tempTask);
        }

        public void CheckMinStatus()
        {
            bool minToTask = Global.MinToTaskbar;
            if (WindowState == WindowState.Minimized && !minToTask)
            {
                Hide();
                showAppInTaskbar = false;
            }
            else if (WindowState == WindowState.Normal && !minToTask)
            {
                Show();
                showAppInTaskbar = true;
            }
        }

        private void CheckDrivers()
        {
            bool deriverinstalled = Global.IsViGEmBusInstalled();
            if (!deriverinstalled)
            {
                Process p = new Process();
                p.StartInfo.FileName = $"{Global.exelocation}";
                p.StartInfo.Arguments = "-driverinstall";
                p.StartInfo.Verb = "runas";
                try { p.Start(); }
                catch { }
            }
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            hotkeysTimer.Stop();
            autoProfilesTimer.Stop();
            //autoProfileHolder.Save();
            //Util.UnregisterNotify(regHandle);
            Application.Current.Shutdown();
        }

        private void WindowStateChanged(object _sender, EventArgs _e)
        {
            CheckMinStatus();
        }

        private void NotifyIcon_TrayRightMouseUp(object sender, RoutedEventArgs e)
        {
            //notifyIcon.ContextMenu = trayIconVM.ContextMenu;
        }

        private void NotifyIcon_TrayMiddleMouseDown(object sender, RoutedEventArgs e)
        {
            //contextclose = true;
            Close();
        }

        private void NotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (!showAppInTaskbar)
            {
                Show();
            }

            WindowState = WindowState.Normal;
        }
    }
}
