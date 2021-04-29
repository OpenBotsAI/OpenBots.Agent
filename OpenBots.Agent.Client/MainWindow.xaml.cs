using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenBots.Agent.Client.Forms;
using OpenBots.Agent.Client.Forms.Dialog;
using OpenBots.Agent.Client.Settings;
using OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.Model;
using OpenBots.Agent.Core.Nuget;
using OpenBots.Agent.Core.UserRegistry;
using OpenBots.Agent.Core.Utilities;
using OpenBots.Core.Settings;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Drawing = System.Drawing;
using SystemForms = System.Windows.Forms;

namespace OpenBots.Agent.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OpenBotsSettings _agentSettings;
        private Timer _serviceHeartBeat;
        private RegistryManager _registryManager;

        private SystemForms.NotifyIcon _notifyIcon = null;
        private Dictionary<string, Drawing.Icon> _iconHandles = null;
        private ContextMenu _contextMenuGearIcon;
        private SystemForms.ContextMenu _contextMenuTrayIcon;
        private SystemForms.MenuItem _menuItemQuit;
        private SystemForms.MenuItem _menuItemAgentSettings;
        private SystemForms.MenuItem _menuItemNugetFeedManager;
        private SystemForms.MenuItem _menuItemAttendedExecution;
        
        private bool _minimizeToTray = true;
        private bool _isServiceUP = false;
        private bool _logInfoChanged = false;

        private AttendedExecution _attendedExecutionWindow;
        public MainWindow()
        {
            InitializeComponent();
            ConnectToService();
        }

        #region Window Events / Helper Methods
        private void OnInitialized(object sender, EventArgs e)
        {
            // Initialize Registry Manager
            _registryManager = new RegistryManager();

            InitializeTrayContextMenu();
            _iconHandles = new Dictionary<string, Drawing.Icon>();
            _iconHandles.Add("QuickLaunch", new Drawing.Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"OpenBots.ico")));
            _notifyIcon = new SystemForms.NotifyIcon();
            _notifyIcon.Click += notifyIcon_Click;
            _notifyIcon.ContextMenu = _contextMenuTrayIcon;
            _notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            _notifyIcon.Icon = _iconHandles["QuickLaunch"];
            StateChanged += OnStateChanged;
            Closing += OnClosing;
            Closed += OnClosed;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;

            SetAgentEnvironment();
            RegisterAgent();
            LoadConnectionSettings();
            UpdateConnectButtonState();
            UpdateSaveButtonState();
            StartServiceHeartBeatTimer();
        }
        private void OnUnload(object sender, RoutedEventArgs e)
        {

        }
        //private void OnFocusOut(object sender, EventArgs e)
        //{
        //    if (this.WindowState != WindowState.Minimized)
        //    {
        //        _minimizeToTray = false;
        //        this.WindowState = WindowState.Minimized;
        //    }
        //}
        private void OnStateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Topmost = false;
                this.ShowInTaskbar = !_minimizeToTray;
                _notifyIcon.Visible = true;
            }
            else
            {
                _notifyIcon.Visible = true;
                this.ShowInTaskbar = true;
                this.Topmost = true;
            }
        }
        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                e.Cancel = true;
                _minimizeToTray = true;
                this.WindowState = WindowState.Minimized;
            }
        }
        private void OnClosed(object sender, System.EventArgs e)
        {
            try
            {
                if (_notifyIcon != null)
                {
                    _notifyIcon.Visible = false;
                    _notifyIcon.Dispose();
                    _notifyIcon = null;
                }
            }
            catch (Exception)
            {
            }
        }
        private void OnMouseLeftButtonDown_TitleBar(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void OpenUpInBottomRight()
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }
        private void InitializeTrayContextMenu()
        {
            _contextMenuTrayIcon = new SystemForms.ContextMenu();
            _menuItemAgentSettings = new SystemForms.MenuItem();
            _menuItemNugetFeedManager = new SystemForms.MenuItem();
            _menuItemAttendedExecution = new SystemForms.MenuItem();
            _menuItemQuit = new SystemForms.MenuItem();

            // Initialize contextMenu
            _contextMenuTrayIcon.MenuItems.AddRange(new SystemForms.MenuItem[]
            {
                _menuItemAgentSettings,
                _menuItemNugetFeedManager,
                _menuItemAttendedExecution,
                _menuItemQuit
            });

            // Initialize _menuItemAgentSettings
            _menuItemAgentSettings.Text = menuItemAgentSettings.Header.ToString();
            _menuItemAgentSettings.Click += OnClick_TrayAgentSettings;

            // Initialize _menuItemQuit
            _menuItemNugetFeedManager.Text = menuItemNugetFeedManager.Header.ToString();
            _menuItemNugetFeedManager.Click += OnClick_TrayNugetFeedManager;

            // Initialize _menuItemQuit
            _menuItemAttendedExecution.Text = menuItemAttendedExecution.Header.ToString();
            _menuItemAttendedExecution.Click += OnClick_TrayAttendedExecution;

            // Initialize _menuItemQuit
            _menuItemQuit.Text = menuItemQuit.Header.ToString();
            _menuItemQuit.Click += OnClick_TrayQuit;
        }

        private void RegisterAgent()
        {
            PipeProxy.Instance.AddAgent();
        }
        private void LoadConnectionSettings()
        {
            // Load settings from "OpenBots.Settings" (Config File)
            _agentSettings = SettingsManager.Instance.ReadSettings();
            bool isServerAlive = false;

            // If Server Connection is already Up and Agent has just started.
            if (PipeProxy.Instance.IsServerConnectionUp())
            {
                // Retrieve Connection Settings from Server
                ConnectionSettingsManager.Instance.ConnectionSettings = PipeProxy.Instance.GetConnectionHistory();
                isServerAlive = true;
            }

            if (ConnectionSettingsManager.Instance.ConnectionSettings == null)
            {
                if (!string.IsNullOrEmpty(_agentSettings.AgentId))
                    _agentSettings = SettingsManager.Instance.ResetToDefaultSettings();

                ConnectionSettingsManager.Instance.ConnectionSettings = new ServerConnectionSettings()
                {
                    ServerConnectionEnabled = false,
                    ServerURL = _registryManager.ServerURL ?? string.Empty,          // Load Server URL from User Registry,
                    AgentUsername = _registryManager.AgentUsername ?? string.Empty,  // Load Username from User Registry
                    AgentPassword = _registryManager.AgentPassword ?? string.Empty,  // Load Password from User Registry
                    SinkType = string.IsNullOrEmpty(_agentSettings.SinkType) ? SinkType.File.ToString() : _agentSettings.SinkType,
                    TracingLevel = string.IsNullOrEmpty(_agentSettings.TracingLevel) ? LogEventLevel.Information.ToString() : _agentSettings.TracingLevel,
                    HeartbeatInterval = _agentSettings.HeartbeatInterval,
                    JobsLoggingInterval = _agentSettings.JobsLoggingInterval,
                    ResolutionHeight = (_agentSettings.ResolutionHeight == 0 ? SystemForms.Screen.PrimaryScreen.Bounds.Height : _agentSettings.ResolutionHeight),
                    ResolutionWidth = (_agentSettings.ResolutionWidth == 0 ? SystemForms.Screen.PrimaryScreen.Bounds.Width : _agentSettings.ResolutionWidth),
                    HighDensityAgent = _agentSettings.HighDensityAgent,
                    SingleSessionExecution = _agentSettings.SingleSessionExecution,
                    SSLCertificateVerification = _agentSettings.SSLCertificateVerification,
                    DNSHost = SystemInfo.GetUserDomainName(),
                    UserName = Environment.UserName,
                    WhoAmI = WindowsIdentity.GetCurrent().Name.ToLower(),
                    MachineName = Environment.MachineName,
                    AgentId = string.Empty,
                    MACAddress = SystemInfo.GetMacAddress(),
                    IPAddress = new WebClient().DownloadString("https://ipv4.icanhazip.com/").Trim()
                };
            }

            // Loading settings in UI
            txt_Username.Text = ConnectionSettingsManager.Instance.ConnectionSettings.AgentUsername;
            txt_Password.Password = ConnectionSettingsManager.Instance.ConnectionSettings.AgentPassword;
            txt_ServerURL.Text = ConnectionSettingsManager.Instance.ConnectionSettings.ServerURL;
            cmb_LogLevel.ItemsSource = Enum.GetValues(typeof(LogEventLevel));
            cmb_LogLevel.SelectedIndex = Array.IndexOf((Array)cmb_LogLevel.ItemsSource, Enum.Parse(typeof(LogEventLevel),
                ConnectionSettingsManager.Instance.ConnectionSettings.TracingLevel));
            cmb_SinkType.ItemsSource = Enum.GetValues(typeof(SinkType));
            cmb_SinkType.SelectedIndex = Array.IndexOf((Array)cmb_SinkType.ItemsSource, Enum.Parse(typeof(SinkType),
                ConnectionSettingsManager.Instance.ConnectionSettings.SinkType));

            // Update UI Controls after loading settings
            OnSetRegistryKeys();
            UpdateClearCredentialsUI();
            OnSinkSelectionChange();

            if (isServerAlive)
            {
                UpdateUIOnConnect();
            }
        }

        private void SetEnvironmentVariable()
        {
            try
            {
                // Get Agent's User Environment Variable Path
                string environmentVariablePath = SettingsManager.Instance.EnvironmentSettings.GetEnvironmentVariablePath();

                // Create Environment Variable if It doesn't exist 
                // or Update the existing one it is not valid for the current user
                if (string.IsNullOrEmpty(environmentVariablePath) ||
                    !SettingsManager.Instance.EnvironmentSettings.isValidEnvironmentVariable())
                {
                    SettingsManager.Instance.EnvironmentSettings.SetEnvironmentVariablePath();
                }
            }
            catch (Exception ex)
            {
                ShowErrorDialog("An error occurred while setting up OpenBots Agent Environment Variable.",
                    "",
                    ex.Message,
                    Application.Current.MainWindow);

                ShutDownApplication();
            }
        }

        private void CreateSettingsFile()
        {
            try
            {
                SettingsManager.Instance.CreateAgentSettingsFile();
            }
            catch (Exception ex)
            {
                ShowErrorDialog("An error occurred while creating Agent's Settings File.",
                        "",
                        ex.Message,
                        Application.Current.MainWindow);

                ShutDownApplication();
            }
        }
        private void SetAgentEnvironment()
        {
            MessageDialog messageDialog = new MessageDialog(
                        "Environment Setup",
                        "Please wait while the environment is being set up for the Agent.",
                        false);
            try
            {
                messageDialog.Topmost = true;
                messageDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                messageDialog.Show();

                // Set Environment Variable Path
                SetEnvironmentVariable();

                // Create Settings File
                CreateSettingsFile();

                // Install Default Packages for the First Time
                NugetPackageManager.SetupFirstTimeUserEnvironment(Environment.UserDomainName, Environment.UserName, SystemForms.Application.ProductVersion);

                messageDialog.CloseManually = true;
                messageDialog.Close();
            }
            catch (Exception ex)
            {
                messageDialog.CloseManually = true;
                messageDialog.Close();

                ShowErrorDialog("An error occurred while setting up environment for the Agent.",
                        "",
                        ex.Message,
                        Application.Current.MainWindow);

                ShutDownApplication();
            }
        }
        private void ConnectToService()
        {
            // Connect to WCF Service Endpoint
            _isServiceUP = PipeProxy.Instance.StartServiceEndPoint();

            if (!_isServiceUP)
            {
                ShowErrorDialog("An error occurred while connecting to the OpenBots Agent Service.",
                    "",
                    "OpenBots Agent Service \"OpenBotsSvc\" is not running. " +
                    "Please start the service and try again.");

                ExitApplication();
            }
        }
        #endregion

        #region Service HeartBeat Method(s)
        private void StartServiceHeartBeatTimer()
        {
            _serviceHeartBeat = new Timer();
            _serviceHeartBeat.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            _serviceHeartBeat.Interval = 5000; //number in miliseconds  
            _serviceHeartBeat.Enabled = true;
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    if (_isServiceUP = PipeProxy.Instance.IsServiceAlive())
                    {
                        if (PipeProxy.Instance.IsServerConnectionUp())
                        {
                            UpdateUIOnConnect();
                        }
                        else
                        {
                            UpdateUIOnDisconnect();
                        }
                    }
                }
                catch (Exception)
                {
                    _isServiceUP = PipeProxy.Instance.StartServiceEndPoint();
                }
                finally
                {
                    if (!_isServiceUP)
                    {
                        UpdateUIOnServiceUnavailable();
                    }
                }
                UpdateConnectButtonState();
            });
        }
        private void UpdateUIOnServiceUnavailable()
        {
            btn_Connect.Content = "Connect";
            lbl_StatusValue.Content = "Not Connected";
            lbl_StatusValue.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF31818"));
            lbl_StatusValue.FontWeight = FontWeights.Bold;
        }

        #endregion

        #region TrayIcon Events / Helper Methods
        private void notifyIcon_Click(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
            OpenUpInBottomRight();
            this.Show();
        }
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
        }

        private void OnClick_TrayQuit(object sender, EventArgs e)
        {
            ExitApplication();
        }
        private void OnClick_TrayAttendedExecution(object sender, EventArgs e)
        {
            ShowAttendedExecutionWindow();
        }
        private void OnClick_TrayNugetFeedManager(object sender, EventArgs e)
        {
            ShowNugetFeedManagerWindow();
        }
        private void OnClick_TrayClearCredentials(object sender, EventArgs e)
        {
            ClearCredentials();
        }
        private void OnClick_TrayAgentSettings(object sender, EventArgs e)
        {
            ShowAgentSettingsWindow();
        }
        private void OnClick_TrayMachineInfo(object sender, EventArgs e)
        {
            ShowMachineInfoWindow();
        }

        private void ShowMachineInfoDialog(string serverIP)
        {
            MachineInfo machineInfoDialog = new MachineInfo(
                    ConnectionSettingsManager.Instance.ConnectionSettings.WhoAmI,
                    ConnectionSettingsManager.Instance.ConnectionSettings.MachineName,
                    ConnectionSettingsManager.Instance.ConnectionSettings.MACAddress,
                    ConnectionSettingsManager.Instance.ConnectionSettings.IPAddress,
                    serverIP);

            machineInfoDialog.Owner = Application.Current.MainWindow;
            machineInfoDialog.ShowDialog();
        }
        private void ShowMachineInfoWindow()
        {
            if (!string.IsNullOrEmpty(txt_ServerURL.Text.Trim()))
            {
                try
                {
                    ConnectionSettingsManager.Instance.ConnectionSettings.ServerURL = txt_ServerURL.Text.Trim();

                    var serverResponse = PipeProxy.Instance.PingServer();
                    if (serverResponse?.Data != null)
                    {
                        ConnectionSettingsManager.Instance.ConnectionSettings.ServerIPAddress = (string)serverResponse.Data;
                        ShowMachineInfoDialog(ConnectionSettingsManager.Instance.ConnectionSettings.ServerIPAddress);
                    }
                    else
                    {
                        ShowMachineInfoDialog(string.Empty);
                    }
                }
                catch (Exception)
                {
                    ShowMachineInfoDialog(string.Empty);
                }
            }
            else
                ShowMachineInfoDialog(string.Empty);
        }
        private void ShowAgentSettingsWindow()
        {
            AgentSettings frmAgentSettings = new AgentSettings(_agentSettings);
            frmAgentSettings.Owner = this;
            frmAgentSettings.ShowDialog();

            if (frmAgentSettings.ChangesSaved)
            {
                _agentSettings = frmAgentSettings.OBSettings;
                ConnectionSettingsManager.Instance.ConnectionSettings.HeartbeatInterval = _agentSettings.HeartbeatInterval;
                ConnectionSettingsManager.Instance.ConnectionSettings.JobsLoggingInterval = _agentSettings.JobsLoggingInterval;
                ConnectionSettingsManager.Instance.ConnectionSettings.HighDensityAgent = _agentSettings.HighDensityAgent;
                ConnectionSettingsManager.Instance.ConnectionSettings.SingleSessionExecution = _agentSettings.SingleSessionExecution;
                ConnectionSettingsManager.Instance.ConnectionSettings.SSLCertificateVerification = _agentSettings.SSLCertificateVerification;
            }
        }
        private void ShowNugetFeedManagerWindow()
        {
            string appDataPath = new EnvironmentSettings().GetEnvironmentVariablePath();
            string appSettingsDirPath = Directory.GetParent(appDataPath).FullName;

            var appSettings = new ApplicationSettings().GetOrCreateApplicationSettings(appSettingsDirPath);
            var packageSourcesDT = appSettings.ClientSettings.PackageSourceDT;

            NugetFeedManager nugetFeedManager = new NugetFeedManager(packageSourcesDT);
            nugetFeedManager.Owner = this;
            nugetFeedManager.ShowDialog();

            if (nugetFeedManager.isDataUpdated)
            {
                appSettings.ClientSettings.PackageSourceDT = nugetFeedManager.GetPackageSourcesData();
                appSettings.Save(appSettings, appSettingsDirPath);
            }
        }
        private void ShowAttendedExecutionWindow()
        {
            if (_attendedExecutionWindow == null)
            {
                _attendedExecutionWindow = new AttendedExecution();
                _attendedExecutionWindow.Closed += (s, args) => this._attendedExecutionWindow = null;
                _attendedExecutionWindow.Show();
            }
            else
                _attendedExecutionWindow.Activate();

            this.WindowState = WindowState.Minimized;
        }
        private void ClearCredentials()
        {
            if (!string.IsNullOrEmpty(_registryManager.AgentUsername))
            {
                // Clear Agent Credentials from Registry
                _registryManager.AgentUsername = string.Empty;
                _registryManager.AgentPassword = string.Empty;

                LoadConnectionSettings();

                // Clear TextBoxes
                txt_Username.Text = string.Empty;
                txt_Password.Password = string.Empty;

                MessageDialog messageDialog = new MessageDialog(
                    "Credentials Cleared",
                    "OpenBots Agent Credentials have been cleared.",
                    true);

                messageDialog.Owner = Application.Current.MainWindow;
                messageDialog.ShowDialog();
            }
        }
        private void ExitApplication()
        {
            ShutDownApplication();
        }
        private void ShutDownApplication()
        {
            Application.Current.Shutdown();
        }
        private void RestartApplication()
        {
            SystemForms.Application.Restart();
            ShutDownApplication();
        }
        #endregion

        #region Input Control Events / Helper Methods
        private void OnTextChange_ServerURL(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            UpdateConnectButtonState();
        }
        private bool UpdateHttpSinkURL()
        {
            string endPoint = SettingsManager.Instance.GetDefaultSettings().LoggingValue1;

            // Change Logging Value for Http Sink Type
            if (cmb_SinkType.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last() == "Http" &&
                string.IsNullOrEmpty(txt_SinkType_Logging1.Text))
            {
                Uri baseUri = new Uri(txt_ServerURL.Text);
                txt_SinkType_Logging1.Text = new Uri(baseUri, endPoint).ToString();
                btn_Save.IsEnabled = false;

                return true;
            }
            return false;
        }
        private void OnUpdateHttpSinkURL(bool isSinkURLModified, bool isConnectedToServer)
        {
            if (isSinkURLModified)
            {
                if (isConnectedToServer)
                    _agentSettings.LoggingValue1 = ConnectionSettingsManager.Instance.ConnectionSettings.LoggingValue1;
                else
                {
                    _agentSettings.LoggingValue1 = string.Empty;
                    ConnectionSettingsManager.Instance.ConnectionSettings.LoggingValue1 = string.Empty;
                    txt_SinkType_Logging1.Text = string.Empty;
                    btn_Save.IsEnabled = false;
                }
            }
        }

        private void OnTextChange_AgentUsername(object sender, TextChangedEventArgs e)
        {
            UpdateConnectButtonState();
        }
        private void OnPasswordChange_AgentPassword(object sender, RoutedEventArgs e)
        {
            UpdateConnectButtonState();
        }
        private void OnClick_ConnectBtn(object sender, RoutedEventArgs e)
        {
            if (btn_Connect.Content.ToString() == "Connect")
            {
                // Update Http Sink URL
                var isSinkURLModified = UpdateHttpSinkURL();

                // Server Configuration
                ConnectionSettingsManager.Instance.ConnectionSettings.ServerURL = txt_ServerURL.Text;
                ConnectionSettingsManager.Instance.ConnectionSettings.AgentUsername = txt_Username.Text;
                ConnectionSettingsManager.Instance.ConnectionSettings.AgentPassword = txt_Password.Password;

                // Logging
                ConnectionSettingsManager.Instance.ConnectionSettings.TracingLevel = cmb_LogLevel.Text;
                ConnectionSettingsManager.Instance.ConnectionSettings.SinkType = cmb_SinkType.Text;
                ConnectionSettingsManager.Instance.ConnectionSettings.LoggingValue1 = txt_SinkType_Logging1.Text;

                try
                {
                    // Calling Service Method to Connect to Server
                    var serverResponse = PipeProxy.Instance.ConnectToServer();
                    if (serverResponse != null)
                    {
                        if (serverResponse.Data != null)
                        {
                            ConnectionSettingsManager.Instance.ConnectionSettings = (ServerConnectionSettings)serverResponse.Data;
                            _agentSettings.OpenBotsServerUrl = ConnectionSettingsManager.Instance.ConnectionSettings.ServerURL;
                            _agentSettings.AgentId = ((ServerConnectionSettings)serverResponse.Data).AgentId.ToString();
                            _agentSettings.AgentName = ((ServerConnectionSettings)serverResponse.Data).AgentName.ToString();
                            _agentSettings.HeartbeatInterval = ((ServerConnectionSettings)serverResponse.Data).HeartbeatInterval;
                            _agentSettings.JobsLoggingInterval = ((ServerConnectionSettings)serverResponse.Data).JobsLoggingInterval;
                            _agentSettings.SSLCertificateVerification = ((ServerConnectionSettings)serverResponse.Data).SSLCertificateVerification;
                            OnUpdateHttpSinkURL(isSinkURLModified, true);

                            UpdateUIOnConnect();

                            //Set Registry Keys if NOT already Set
                            if (string.IsNullOrEmpty(_registryManager.AgentUsername) || string.IsNullOrEmpty(_registryManager.AgentPassword) ||
                                string.IsNullOrEmpty(_registryManager.ServerURL))
                            {
                                _registryManager.AgentUsername = ConnectionSettingsManager.Instance.ConnectionSettings.AgentUsername;
                                _registryManager.AgentPassword = ConnectionSettingsManager.Instance.ConnectionSettings.AgentPassword;
                                _registryManager.ServerURL = ConnectionSettingsManager.Instance.ConnectionSettings.ServerURL;

                                OnSetRegistryKeys();
                            }
                            else if (_registryManager.ServerURL != ConnectionSettingsManager.Instance.ConnectionSettings.ServerURL)
                            {
                                // If Server URL is updated
                                _registryManager.ServerURL = ConnectionSettingsManager.Instance.ConnectionSettings.ServerURL;
                            }

                            // Update OpenBots.settings file
                            SettingsManager.Instance.UpdateSettings(_agentSettings);
                        }
                        else
                        {
                            OnUpdateHttpSinkURL(isSinkURLModified, false);

                            ShowErrorDialog("An error occurred while connecting to the server.",
                                serverResponse.StatusCode,
                                serverResponse.Message,
                                Application.Current.MainWindow);
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnUpdateHttpSinkURL(isSinkURLModified, false);

                    ShowErrorDialog("An error occurred while connecting to the server.",
                        ex.GetType().GetProperty("ErrorCode")?.GetValue(ex, null)?.ToString() ?? string.Empty,
                        ex.Message,
                        Application.Current.MainWindow);
                }
            }
            else if (btn_Connect.Content.ToString() == "Disconnect")
            {
                try
                {
                    // Calling Service Method to Disconnect from Server
                    var serverResponse = PipeProxy.Instance.DisconnectFromServer();
                    if (serverResponse != null)
                    {
                        if (serverResponse.StatusCode == "200")
                        {
                            ConnectionSettingsManager.Instance.ConnectionSettings = (ServerConnectionSettings)serverResponse.Data;
                            _agentSettings.OpenBotsServerUrl = "";
                            _agentSettings.AgentId = string.Empty;
                            _agentSettings.AgentName = string.Empty;

                            UpdateUIOnDisconnect();

                            // Update OpenBots.settings file
                            SettingsManager.Instance.UpdateSettings(_agentSettings);
                        }
                        else
                        {
                            string errorMessage = JToken.Parse(serverResponse.Message).ToString(Formatting.Indented);

                            ShowErrorDialog("An error occurred while disconnecting from the server.",
                                serverResponse.StatusCode,
                                errorMessage,
                                Application.Current.MainWindow);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorDialog("An error occurred while disconnecting from the server.",
                        ex.GetType().GetProperty("ErrorCode")?.GetValue(ex, null)?.ToString() ?? string.Empty,
                        ex.Message,
                        Application.Current.MainWindow);
                }
            }
        }
        private void OnDropDownClosed_LogLevel(object sender, EventArgs e)
        {
            if (!_agentSettings.TracingLevel.Equals(cmb_LogLevel.Text) || !_agentSettings.SinkType.Equals(cmb_SinkType.Text))
                _logInfoChanged = true;
            else
                _logInfoChanged = false;
            UpdateSaveButtonState();
        }
        private void OnDropDownClosed_SinkType(object sender, EventArgs e)
        {
            // Update UI OnSinkSelectionChange
            OnSinkSelectionChange();

            if (!_agentSettings.SinkType.Equals(cmb_SinkType.Text) || !_agentSettings.TracingLevel.Equals(cmb_LogLevel.Text))
                _logInfoChanged = true;
            else
                _logInfoChanged = false;
            UpdateSaveButtonState();
        }
        private void OnTextChange_Logging1(object sender, TextChangedEventArgs e)
        {
            if (!_agentSettings.LoggingValue1.Equals(txt_SinkType_Logging1.Text))
                _logInfoChanged = true;
            else
                _logInfoChanged = false;

            SetToolTip(txt_SinkType_Logging1);
            UpdateSaveButtonState();
        }
        private void OnClick_SaveBtn(object sender, RoutedEventArgs e)
        {
            _agentSettings.TracingLevel = cmb_LogLevel.Text;
            _agentSettings.SinkType = cmb_SinkType.Text;
            _agentSettings.LoggingValue1 = txt_SinkType_Logging1.Text;

            SettingsManager.Instance.UpdateSettings(_agentSettings);
            _logInfoChanged = false;
            UpdateSaveButtonState();
        }

        private void SetToolTip(TextBox txtLogging)
        {
            if (txtLogging.Text.Length > 36)
                txtLogging.ToolTip = txtLogging.Text;
            else
                txtLogging.ClearValue(TextBox.ToolTipProperty);
        }
        private void UpdateUIOnConnect()
        {
            btn_Connect.Content = "Disconnect";
            lbl_StatusValue.Content = "Connected";
            lbl_StatusValue.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF4FE823"));
            lbl_StatusValue.FontWeight = FontWeights.Bold;

            // Disable Input Controls
            txt_ServerURL.IsEnabled = false;

            cmb_LogLevel.IsEnabled = false;
            cmb_SinkType.IsEnabled = false;
            txt_SinkType_Logging1.IsEnabled = false;

            // Disable and Hide menuItemClearCredentials
            UpdateClearCredentialsUI();

            // Disable and Hide menuItemAgentSettings
            UpdateAgentSettingsUI();
        }
        private void UpdateUIOnDisconnect()
        {
            btn_Connect.Content = "Connect";
            lbl_StatusValue.Content = "Offline";
            lbl_StatusValue.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFBBB5B5"));
            lbl_StatusValue.FontWeight = FontWeights.Normal;

            // Enable Input Controls
            txt_ServerURL.IsEnabled = true;

            cmb_LogLevel.IsEnabled = true;
            cmb_SinkType.IsEnabled = true;
            txt_SinkType_Logging1.IsEnabled = true;

            // Enable and Show menuItemClearCredentials
            UpdateClearCredentialsUI();

            // Disable and Hide menuItemAgentSettings
            UpdateAgentSettingsUI();
        }
        private void UpdateClearCredentialsUI()
        {
            if (!string.IsNullOrEmpty(_registryManager.AgentUsername) && 
                !ConnectionSettingsManager.Instance.ConnectionSettings.ServerConnectionEnabled)
            {
                img_ClearCredentials.IsEnabled = true;
            }
            else
            {
                img_ClearCredentials.IsEnabled = false;
            }
        }
        private void UpdateAgentSettingsUI()
        {
            if (ConnectionSettingsManager.Instance.ConnectionSettings.ServerConnectionEnabled)
            {
                menuItemAgentSettings.IsEnabled = _menuItemAgentSettings.Enabled = false;
                menuItemAgentSettings.Visibility = Visibility.Collapsed;
                _menuItemAgentSettings.Visible = false;
            }
            else
            {
                menuItemAgentSettings.IsEnabled = _menuItemAgentSettings.Enabled = true;
                menuItemAgentSettings.Visibility = Visibility.Visible;
                _menuItemAgentSettings.Visible = true;
            }
        }
        private void UpdateConnectButtonState()
        {
            if (lbl_StatusValue.Content.ToString() != "Not Connected" && !string.IsNullOrEmpty(txt_ServerURL.Text)
                && !string.IsNullOrEmpty(txt_Username.Text) && !string.IsNullOrEmpty(txt_Password.Password)
                && !btn_Save.IsEnabled)
                btn_Connect.IsEnabled = true;
            else
                btn_Connect.IsEnabled = false;
        }
        private void UpdateSaveButtonState()
        {
            if (_logInfoChanged && _agentSettings.SinkType.Equals("File") && !string.IsNullOrEmpty(txt_SinkType_Logging1.Text))
                btn_Save.IsEnabled = true;
            else if (_logInfoChanged && _agentSettings.SinkType.Equals("Http") && !string.IsNullOrEmpty(txt_SinkType_Logging1.Text))
                btn_Save.IsEnabled = true;
            else
                btn_Save.IsEnabled = false;

            UpdateConnectButtonState();
        }
        private void OnSinkSelectionChange()
        {
            switch (cmb_SinkType.SelectedItem.ToString().Split(new string[] { ": " }, StringSplitOptions.None).Last())
            {
                case "File":
                    // Update Label Properties
                    lbl_SinkType_Logging1.Content = "File Path";
                    lbl_SinkType_Logging1.ToolTip = "File Path to write logs to";

                    // Update UI for SinkType_Save Button
                    pnl_SinkType_Save.SetValue(Grid.RowProperty, 2);

                    txt_SinkType_Logging1.Text = _agentSettings.SinkType.Equals("File") ? _agentSettings.LoggingValue1 : string.Empty;

                    break;
                case "Http":
                    // Update Label Properties
                    lbl_SinkType_Logging1.Content = "URI";
                    lbl_SinkType_Logging1.ToolTip = "URI to send logs to";

                    // Update UI for SinkType_Save Button
                    pnl_SinkType_Save.SetValue(Grid.RowProperty, 2);

                    txt_SinkType_Logging1.Text = _agentSettings.SinkType.Equals("Http") ? _agentSettings.LoggingValue1 : string.Empty;

                    break;
            }
        }
        private void OnSetRegistryKeys()
        {
            // If Agent's Credentials (Username, Password) exist in the Registry
            if (!string.IsNullOrEmpty(_registryManager.AgentUsername) && !string.IsNullOrEmpty(_registryManager.AgentPassword))
            {
                // Disable Credentials Controls
                txt_Username.IsEnabled = false;
                txt_Password.IsEnabled = false;
            }
            else
            {
                // Enable Credentials Controls
                txt_Username.IsEnabled = true;
                txt_Password.IsEnabled = true;
            }
        }
        #endregion

        #region Dialog Windows Handler
        private void ShowErrorDialog(string generalMessage, string errorCode, string errorMessage, Window parentWindow = null)
        {
            ErrorDialog errorDialog = new ErrorDialog(generalMessage, errorCode, errorMessage);
            if (parentWindow != null)
                errorDialog.Owner = parentWindow;
            errorDialog.ShowDialog();
        }
        #endregion

        #region Top MenuBar Controls Events
        private void OnClick_MachineInfo(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ShowMachineInfoWindow();
        }
        private void OnClick_AgentSettings(object sender, RoutedEventArgs e)
        {
            ShowAgentSettingsWindow();
        }
        private void OnClick_NugetFeedManager(object sender, RoutedEventArgs e)
        {
            ShowNugetFeedManagerWindow();
        }
        private void OnClick_AttendedExecution(object sender, RoutedEventArgs e)
        {
            ShowAttendedExecutionWindow();
        }
        private void OnClick_ClearCredentials(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ClearCredentials();
        }
        private void OnClick_Quit(object sender, RoutedEventArgs e)
        {
            // Close the application.
            ExitApplication();
        }
        private void OnClick_Settings(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            _contextMenuGearIcon = image.ContextMenu;
            _contextMenuGearIcon.PlacementTarget = image;
            _contextMenuGearIcon.IsOpen = true;
        }
        private void OnClick_Close(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }


        #endregion
    }
}
