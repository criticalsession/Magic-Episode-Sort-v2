using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TheMagic;

namespace Magic_Episode_Sort_v2
{
    /// <summary>
    /// Interaction logic for FirstTime.xaml
    /// </summary>
    public partial class FirstTime : Window
    {
        public bool migrationComplete = false;

        public FirstTime()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (OldSettingsMigrator.MigrationResults result, int totalSeriesTitles) = OldSettingsMigrator.Migrate();

            if (result == OldSettingsMigrator.MigrationResults.Successful)
            {
                MessageBox.Show("Settings migration successful! " + totalSeriesTitles + " custom titles migrated.", "Migration", MessageBoxButton.OK, MessageBoxImage.Information);
                migrationComplete = true;
            }
            else if (result == OldSettingsMigrator.MigrationResults.Error)
                MessageBox.Show("An unexpected error occured while migrating settings.", "Migration", MessageBoxButton.OK, MessageBoxImage.Error);

            this.Close();
        }
    }
}
