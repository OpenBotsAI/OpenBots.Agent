using System;
using System.Windows;
using AgentEnums = OpenBots.Agent.Core.Enums;

namespace OpenBots.Agent.Client.Forms
{
    /// <summary>
    /// Interaction logic for AgentSettings.xaml
    /// </summary>
    public partial class AgentSettings : Window
    {
        public OpenBotsSettings OBSettings { get; private set; }
        public bool ChangesSaved { get; private set; } = false;
        private bool _settingsChanged = false;
        private const int heartbeatMinInterval = 30;
        private const int jobLoggingMinInterval = 5;
        public AgentSettings(OpenBotsSettings openBotsSettings)
        {
            InitializeComponent();
            OBSettings = openBotsSettings;
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            updown_HeartbeatInterval.Value = OBSettings.HeartbeatInterval;
            updown_HeartbeatInterval.Minimum = heartbeatMinInterval;
            updown_HeartbeatInterval.ValueChanged += OnHeartbeatIntervalChanged;
            updown_LoggingInterval.Value = OBSettings.JobsLoggingInterval;
            updown_LoggingInterval.Minimum = jobLoggingMinInterval;
            updown_LoggingInterval.ValueChanged += OnLoggingIntervalChanged;

            //cmb_HighDensityAgent.ItemsSource = Enum.GetValues(typeof(AgentEnums.BooleanAlias));
            //cmb_HighDensityAgent.SelectedIndex = Array.IndexOf((Array)cmb_HighDensityAgent.ItemsSource, Enum.Parse(typeof(AgentEnums.BooleanAlias), GetEnumAliasOfBool(OBSettings.HighDensityAgent)));

            cmb_SSLCertificateVerification.ItemsSource = Enum.GetValues(typeof(AgentEnums.BooleanAlias));
            cmb_SSLCertificateVerification.SelectedIndex = Array.IndexOf((Array)cmb_SSLCertificateVerification.ItemsSource, Enum.Parse(typeof(AgentEnums.BooleanAlias), GetEnumAliasOfBool(OBSettings.SSLCertificateVerification)));

            UpdateSaveButtonState();
        }

        private void OnLoggingIntervalChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CheckSettingsChange();
        }

        private void OnHeartbeatIntervalChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CheckSettingsChange();
        }

        private string GetEnumAliasOfBool(bool isTrue)
        {
            return (isTrue ? AgentEnums.BooleanAlias.Yes : AgentEnums.BooleanAlias.No).ToString();
        }

        private bool GetBoolAliasOfEnum(string enumAlias)
        {
            return (AgentEnums.BooleanAlias.Yes.ToString().Equals(enumAlias) ? true : false);
        }

        private void OnDropDownClosed_HighDensityAgent(object sender, EventArgs e)
        {
            CheckSettingsChange();
        }

        private void OnDropDownClosed_SSLCertificateVerification(object sender, EventArgs e)
        {
            CheckSettingsChange();
        }

        private void CheckSettingsChange()
        {
            if (updown_HeartbeatInterval.Value != ((double)OBSettings.HeartbeatInterval) ||
                updown_LoggingInterval.Value != ((double)OBSettings.JobsLoggingInterval) ||
                //!(OBSettings.HighDensityAgent == GetBoolAliasOfEnum(cmb_HighDensityAgent.Text)) ||
                !(OBSettings.SSLCertificateVerification == GetBoolAliasOfEnum(cmb_SSLCertificateVerification.Text)))
                _settingsChanged = true;
            else
                _settingsChanged = false;

            UpdateSaveButtonState();
        }

        private void UpdateSaveButtonState()
        {
            if (_settingsChanged)
                btn_Save.IsEnabled = true;
            else
                btn_Save.IsEnabled = false;
        }

        private void OnClick_SaveBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                OBSettings.HeartbeatInterval = (int)updown_HeartbeatInterval.Value;
                OBSettings.JobsLoggingInterval = (int)updown_LoggingInterval.Value;
                //OBSettings.HighDensityAgent = GetBoolAliasOfEnum(cmb_HighDensityAgent.Text);
                OBSettings.SSLCertificateVerification = GetBoolAliasOfEnum(cmb_SSLCertificateVerification.Text);

                SettingsManager.Instance.UpdateSettings(OBSettings);
                _settingsChanged = false;
                ChangesSaved = true;
                Close();
            }
            catch
            {
                Close();
            }
        }
    }
}
