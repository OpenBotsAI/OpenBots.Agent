﻿using AgentEnums = OpenBots.Agent.Core.Enums;
using OpenBots.Agent.Core.Model;
using OpenBots.Core.Enums;
using OpenBots.Core.IO;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using OpenBots.Agent.Client.Settings;

namespace OpenBots.Agent.Client.Forms
{
    /// <summary>
    /// Interaction logic for AttendedExecution.xaml
    /// </summary>
    public partial class AttendedExecution : Window
    {
        private string _lastTask;
        private bool _isEngineBusy;
        private string[] _automationProjects;
        private FileSystemWatcher _publishedProjectsWatcher;
        private ServerConnectionSettings _connectionSettings;
        private string _automationSource;

        public AttendedExecution(bool isEngineBusy)
        {
            InitializeComponent();
            _publishedProjectsWatcher = new FileSystemWatcher();
            _isEngineBusy = isEngineBusy;

            _connectionSettings = new ServerConnectionSettings()
            {
                SinkType = SinkType.File.ToString(),
                TracingLevel = LogEventLevel.Information.ToString(),
                LoggingValue1 = Path.Combine(new EnvironmentSettings().GetEnvironmentVariable(), "Logs", "Attended Execution", "log.txt"),
                DNSHost = Dns.GetHostName(),
                UserName = Environment.UserName
            };
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            // Load Automation Sources
            LoadAutomationSources();

            // Published Projects Directory Watcher
            var publishedProjectsDir = Folders.GetFolder(FolderType.PublishedFolder);
            _publishedProjectsWatcher.Path = publishedProjectsDir;
            _publishedProjectsWatcher.Filter = "*.nupkg";
            _publishedProjectsWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
            _publishedProjectsWatcher.Created += new FileSystemEventHandler(OnFileCreated);
            _publishedProjectsWatcher.Deleted += new FileSystemEventHandler(OnFileDeleted);
            _publishedProjectsWatcher.Renamed += new RenamedEventHandler(OnFileRenamed);
            _publishedProjectsWatcher.EnableRaisingEvents = true;

            lbl_Status.Visibility = Visibility.Collapsed;
            LoadAutomations();
            OpenUpInBottomRight();
        }

        private void LoadAutomationSources()
        {
            cmb_Source.ItemsSource = Enum.GetValues(typeof(AgentEnums.AutomationSource));
            if (!string.IsNullOrEmpty(_automationSource))
                cmb_Source.SelectedIndex = Array.IndexOf((Array)cmb_Source.ItemsSource, Enum.Parse(typeof(AgentEnums.AutomationSource), _automationSource));
            else
            {
                cmb_Source.SelectedIndex = 0;
                _automationSource = cmb_Source.Text;
            }
        }

        private void LoadAutomations()
        {
            cmb_PublishedProjects.ItemsSource = Enumerable.Empty<string>();

            switch (_automationSource)
            {
                case "Local":
                    LoadLocalAutomations();
                    break;
                case "Server":
                    LoadServerAutomations();
                    break;
            }
        }

        private void OpenUpInBottomRight()
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }
        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            LoadLocalAutomations();
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            LoadLocalAutomations();
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            LoadLocalAutomations();
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            LoadLocalAutomations();
        }

        private async void OnClick_RunBtn(object sender, RoutedEventArgs e)
        {
            string projectPackage = string.Empty;
            switch (_automationSource)
            {
                case "Local":
                    _lastTask = cmb_PublishedProjects.SelectedItem.ToString();
                    projectPackage = _automationProjects.Where(x => x.EndsWith(_lastTask)).FirstOrDefault();
                    break;
                case "Server":
                    projectPackage = _lastTask = cmb_PublishedProjects.SelectedItem.ToString();
                    break;
            }
            
            PipeProxy.Instance.TaskFinishedEvent += OnAttendedTaskFinished;
            await Task.Run(()=>PipeProxy.Instance.ExecuteAttendedTask(projectPackage, _connectionSettings, projectPackage.Equals(_lastTask)));

            string executionStatus = "Running {0} . . .";
            lbl_Status.Content = string.Format(executionStatus, $"\"{_lastTask}\"");
            lbl_Status.Visibility = Visibility.Visible;
        }

        private void OnAttendedTaskFinished(object sender, bool isJobSuccessful)
        {
            Dispatcher.Invoke(() => 
            {
                string lastRunStatus = "Last Run: {0} - Status: {1}";
                if (isJobSuccessful)
                    lbl_Status.Content = string.Format(lastRunStatus, _lastTask, "Successful");
                else
                    lbl_Status.Content = string.Format(lastRunStatus, _lastTask, "Failed");
            });
        }

        private void LoadLocalAutomations()
        {
            Dispatcher.Invoke(() =>
            {
                var publishedProjectsDir = Folders.GetFolder(FolderType.PublishedFolder);
                _automationProjects = Directory.GetFiles(publishedProjectsDir, "*.nupkg");
                var automationNames = from fileName in _automationProjects select Path.GetFileName(fileName);

                cmb_PublishedProjects.ItemsSource = automationNames;
                cmb_PublishedProjects.SelectedIndex = 0;
            });
        }

        private void LoadServerAutomations()
        {
            if (ConnectionSettingsManager.Instance.ConnectionSettings.ServerConnectionEnabled)
            {
                // Fetch Server Automations
                var automationNames = PipeProxy.Instance.GetAutomations();

                cmb_PublishedProjects.ItemsSource = automationNames;
                cmb_PublishedProjects.SelectedIndex = 0;
            }
        }

        private void OnDropDownClosed_Source(object sender, EventArgs e)
        {
            // Update Automations List on Source Selection Change
            if(_automationSource != cmb_Source.SelectedItem.ToString())
            {
                _automationSource = cmb_Source.SelectedItem.ToString();
                UpdateStatus();
                LoadAutomations();
            }
        }

        private void UpdateStatus()
        {
            if (!ConnectionSettingsManager.Instance.ConnectionSettings.ServerConnectionEnabled && _automationSource == "Server")
            {
                lbl_Status.Content = "To get the server automations, please connect the Agent first.";
                lbl_Status.Foreground = new SolidColorBrush(Colors.Red);
                lbl_Status.Visibility = Visibility.Visible;
            }
            else
            {
                lbl_Status.Foreground = new SolidColorBrush(Colors.Green);
                lbl_Status.Visibility = Visibility.Collapsed;
            }
        }
    }
}
