using OpenBots.Agent.Core.Model;
using System;
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
        public NugetFeedManager(DataTable packageSourcesDataTable)
        {
            InitializeComponent();
            packageSourcesDataTable = UpdateDataTableColumnType(packageSourcesDataTable, typeof(bool));
            dtGrd_NugetSources.DataContext = packageSourcesDataTable.DefaultView;
        }

        public DataTable GetPackageSourcesData()
        {
            var packageSources = ((DataView)dtGrd_NugetSources.DataContext).ToTable();
            packageSources = UpdateDataTableColumnType(packageSources, typeof(string));
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

        private DataTable UpdateDataTableColumnType(DataTable dataTable, Type type)
        {
            DataTable dtCloned = dataTable.Clone();
            dtCloned.Columns[0].DataType = type;
            foreach (DataRow row in dataTable.Rows)
            {
                dtCloned.ImportRow(row);
            }

            return dtCloned;
        }
    }
}
