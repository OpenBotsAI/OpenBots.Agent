using OpenBots.Agent.Core.Model;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace OpenBots.Agent.Client.Forms
{
    /// <summary>
    /// Interaction logic for NugetFeedManager.xaml
    /// </summary>
    public partial class NugetFeedManager : Window
    {
        public bool isDataUpdated { get; private set; } = false;
        public NugetFeedManager(List<NugetPackageSource> packageSources)
        {
            InitializeComponent();
            dtGrd_NugetSources.ItemsSource = packageSources;
        }

        public List<NugetPackageSource> GetPackageSourcesData()
        {
            var packageSources = dtGrd_NugetSources.ItemsSource.Cast<NugetPackageSource>().ToList();
            return packageSources;
        }

        private void OnClick_OKBtn(object sender, RoutedEventArgs e)
        {
            isDataUpdated = true;
            this.Close();
        }

        private void OnClick_CancelBtn(object sender, RoutedEventArgs e)
        {
            isDataUpdated = false;
            this.Close();
        }
    }
}
