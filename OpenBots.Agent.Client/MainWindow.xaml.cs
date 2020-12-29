using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenBots.Agent.Client.Forms.Dialog;
using OpenBots.Agent.Client.Utilities;
using OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.UserRegistry;
using OpenBots.Agent.Core.Model;
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
        private ServerConnectionSettings _connectionSettings;
        private OpenBotsSettings _agentSettings;
        private Timer _serviceHeartBeat;
        private RegistryManager _registryManager;

        private SystemForms.NotifyIcon _notifyIcon = null;
        private Dictionary<string, Drawing.Icon> _iconHandles = null;
        private ContextMenu _contextMenuTrayIcon;

        private bool _minimizeToTray = true;
        private bool _isServiceUP = false;
        private bool _logInfoChanged = false;

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

            _iconHandles = new Dictionary<string, Drawing.Icon>();
            _iconHandles.Add("QuickLaunch", new Drawing.Icon(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"OpenBots.ico")));
            _notifyIcon = new SystemForms.NotifyIcon();
            _notifyIcon.Click += notifyIcon_Click;
            _notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            _notifyIcon.Icon = _iconHandles["QuickLaunch"];
            StateChanged += OnStateChanged;
            Closing += OnClosing;
            Closed += OnClosed;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            SetConfigFilePath();
            LoadConnectionSettings();
            UpdateConnectButtonState();
            UpdateSaveButtonState();
            StartServiceHeartBeatTimer();

            this.WindowState = WindowState.Minimized;
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
        private void LoadConnectionSettings()
        {
            // Load settings from "OpenBots.Settings" (Config File)
            _agentSettings = SettingsManager.Instance.ReadSettings();
            bool isServerAlive = false;

            // If Server Connection is already Up and Agent has just started.
            if (PipeProxy.Instance.IsServerConnectionUp())
            {
                // Retrieve Connection Settings from Server
                _connectionSettings = PipeProxy.Instance.GetConnectionHistory();
                isServerAlive = true;
            }

            if (_connectionSettings == null)
            {
                if (!string.IsNullOrEmpty(_agentSettings.AgentId))
                    _agentSettings = SettingsManager.Instance.ResetToDefaultSettings();

                _connectionSettings = new ServerConnectionSettings()
                {
                    ServerConnectionEnabled = false,
                    ServerURL = _registryManager.ServerURL ?? string.Empty,          // Load Server URL from User Registry,
                    AgentUsername = _registryManager.AgentUsername ?? string.Empty,  // Load Username from User Registry
                    AgentPassword = _registryManager.AgentPassword ?? string.Empty,  // Load Password from User Registry
                    SinkType = string.IsNullOrEmpty(_agentSettings.SinkType) ? SinkType.File.ToString() : _agentSettings.SinkType,
                    TracingLevel = string.IsNullOrEmpty(_agentSettings.TracingLevel) ? LogEventLevel.Information.ToString() : _agentSettings.TracingLevel,
                    DNSHost = Dns.GetHostName(),
                    WhoAmI = WindowsIdentity.GetCurrent().Name.ToLower(),
                    MachineName = Environment.MachineName,
                    AgentId = string.Empty,
                    MACAddress = AgentHelper.GetMacAddress(),
                    IPAddress = new WebClient().DownloadString("https://ipv4.icanhazip.com/").Trim()
                };
            }

            // Loading settings in UI
            txt_Username.Text = _connectionSettings.AgentUsername;
            txt_Password.Password = _connectionSettings.AgentPassword;
            txt_ServerURL.Text = _connectionSettings.ServerURL;
            cmb_LogLevel.ItemsSource = Enum.GetValues(typeof(LogEventLevel));
            cmb_LogLevel.SelectedIndex = Array.IndexOf((Array)cmb_LogLevel.ItemsSource, Enum.Parse(typeof(LogEventLevel), _connectionSettings.TracingLevel));
            cmb_SinkType.ItemsSource = Enum.GetValues(typeof(SinkType));
            cmb_SinkType.SelectedIndex = Array.IndexOf((Array)cmb_SinkType.ItemsSource, Enum.Parse(typeof(SinkType), _connectionSettings.SinkType));

            // Update UI Controls after loading settings
            OnSetRegistryKeys();
            UpdateClearCredentialsUI();
            OnSinkSelectionChange();

            if (isServerAlive)
            {
                UpdateUIOnConnect();
            }
        }
        private void SetConfigFilePath()
        {
            if (_isServiceUP)
            {
                try
                {
                    // Get Settings file Path from Environment Variable
                    string environmentVariableValue = SettingsManager.Instance.EnvironmentSettings.GetEnvironmentVariable();

                    // Create Environment Variable if It doesn't exist
                    if (string.IsNullOrEmpty(environmentVariableValue))
                    {
                        string settingsFilePath = SettingsManager.Instance.GetSettingsFilePath();

                        if (File.Exists(settingsFilePath))
                            PipeProxy.Instance.SetConfigFilePath(SettingsManager.Instance.EnvironmentSettings.EnvironmentVariableName, settingsFilePath);
                        else
                            throw new FileNotFoundException($"OpenBots Agent Settings file not found at \"{settingsFilePath}\"");
                    }
                }
                catch (FileNotFoundException ex)
                {
                    ShowErrorDialog("An error occurred while setting up OpenBots Agent Settings File Path " +
                        "to an Environment Variable.",
                        "",
                        ex.Message,
                        Application.Current.MainWindow);

                    ShutDownApplication();
                }
                catch (Exception ex)
                {
                    ShowErrorDialog("An error occurred while setting up OpenBots.Settings (Config File Path)" +
                        $"to an Environment Variable. Please add the variable \"{SettingsManager.Instance.EnvironmentSettings.EnvironmentVariableName}\" " +
                        "manually and re-run the agent.",
                        "",
                        ex.Message,
                        Application.Current.MainWindow);

                    ShutDownApplication();
                }
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
            this.Show();
        }
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
        }
        private void ShowMachineInfoDialog(string serverIP)
        {
            MachineInfo machineInfoDialog = new MachineInfo(
                    _connectionSettings.WhoAmI,
                    _connectionSettings.DNSHost,
                    _connectionSettings.MACAddress,
                    _connectionSettings.IPAddress,
                    serverIP);

            machineInfoDialog.Owner = Application.Current.MainWindow;
            machineInfoDialog.ShowDialog();
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
        private void ClearCredentials()
        {
            // Clear from Registry
            _registryManager.AgentUsername = string.Empty;
            _registryManager.AgentPassword = string.Empty;
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
                    _agentSettings.LoggingValue1 = _connectionSettings.LoggingValue1;
                else
                {
                    _agentSettings.LoggingValue1 = string.Empty;
                    _connectionSettings.LoggingValue1 = string.Empty;
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
                _connectionSettings.ServerURL = txt_ServerURL.Text;
                _connectionSettings.AgentUsername = txt_Username.Text;
                _connectionSettings.AgentPassword = txt_Password.Password;

                // Logging
                _connectionSettings.TracingLevel = cmb_LogLevel.Text;
                _connectionSettings.SinkType = cmb_SinkType.Text;
                _connectionSettings.LoggingValue1 = txt_SinkType_Logging1.Text;

                try
                {
                    // Calling Service Method to Connect to Server
                    var serverResponse = PipeProxy.Instance.ConnectToServer(_connectionSettings);
                    if (serverResponse != null)
                    {
                        if (serverResponse.Data != null)
                        {
                            _agentSettings.OpenBotsServerUrl = _connectionSettings.ServerURL;
                            _agentSettings.AgentId = ((ServerConnectionSettings)serverResponse.Data).AgentId.ToString();
                            _agentSettings.AgentName = ((ServerConnectionSettings)serverResponse.Data).AgentName.ToString();
                            OnUpdateHttpSinkURL(isSinkURLModified, true);

                            UpdateUIOnConnect();

                            //Set Registry Keys if NOT already Set
                            if (string.IsNullOrEmpty(_registryManager.AgentUsername) || string.IsNullOrEmpty(_registryManager.AgentPassword) ||
                                string.IsNullOrEmpty(_registryManager.ServerURL))
                            {
                                _registryManager.AgentUsername = _connectionSettings.AgentUsername;
                                _registryManager.AgentPassword = _connectionSettings.AgentPassword;
                                _registryManager.ServerURL = _connectionSettings.ServerURL;

                                OnSetRegistryKeys();
                            }
                            else if(_registryManager.ServerURL != _connectionSettings.ServerURL)
                            {
                                // If Server URL is updated
                                _registryManager.ServerURL = _connectionSettings.ServerURL;
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
                    var serverResponse = PipeProxy.Instance.DisconnectFromServer(_connectionSettings);
                    if (serverResponse != null)
                    {
                        if (serverResponse.StatusCode == "200")
                        {
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
        }
        private void UpdateClearCredentialsUI()
        {
            if (!string.IsNullOrEmpty(_registryManager.AgentUsername))
            {
                menuItemClearCredentials.IsEnabled = true;
                menuItemClearCredentials.Visibility = Visibility.Visible;
            }
            else
            {
                menuItemClearCredentials.IsEnabled = false;
                menuItemClearCredentials.Visibility = Visibility.Collapsed;
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
        private void OnClick_MachineInfo(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_ServerURL.Text.Trim()))
            {
                try
                {
                    _connectionSettings.ServerURL = txt_ServerURL.Text.Trim();

                    var serverResponse = PipeProxy.Instance.PingServer(_connectionSettings);
                    if (serverResponse?.Data != null)
                    {
                        _connectionSettings.ServerIPAddress = (string)serverResponse.Data;
                        ShowMachineInfoDialog(_connectionSettings.ServerIPAddress);
                    }
                    else
                    {
                        ShowErrorDialog("An error occurred while pinging the server",
                            serverResponse.StatusCode,
                            serverResponse.Message,
                            Application.Current.MainWindow);
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorDialog("An error occurred while pinging the server",
                            "",
                            ex.Message,
                            Application.Current.MainWindow);
                }
            }
            else
                ShowMachineInfoDialog(string.Empty);
        }
        private void OnClick_ClearCredentials(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_registryManager.AgentUsername))
            {
                ClearCredentials();
                LoadConnectionSettings();

                // Clear TextBoxes
                txt_Username.Text = string.Empty;
                txt_Password.Password = string.Empty;

                MessageDialog messageDialog = new MessageDialog(
                    "Credentials Cleared",
                    "OpenBots Agent Credentials have been cleared.");

                messageDialog.Owner = Application.Current.MainWindow;
                messageDialog.ShowDialog();
            }
        }
        private void OnClick_Quit(object sender, RoutedEventArgs e)
        {
            // Close the application.
            ExitApplication();
        }
        private void OnClick_Settings(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            _contextMenuTrayIcon = image.ContextMenu;
            _contextMenuTrayIcon.PlacementTarget = image;
            _contextMenuTrayIcon.IsOpen = true;
        }
        private void OnClick_Close(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
