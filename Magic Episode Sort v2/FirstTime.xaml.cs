using System;
using System.Windows;
using TheMagic;

namespace Magic_Episode_Sort_v2
{
    /// <summary>
    /// Interaction logic for FirstTime.xaml
    /// </summary>
    public partial class FirstTime : Window
    {
        public bool MigrationComplete;

        public FirstTime()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var (result, totalSeriesTitles) = OldSettingsMigrator.Migrate();

            switch (result)
            {
                case OldSettingsMigrator.MigrationResults.Successful:
                    MessageBox.Show($"Settings migration successful! {totalSeriesTitles} custom titles migrated.", "Migration", MessageBoxButton.OK, MessageBoxImage.Information);
                    MigrationComplete = true;
                    break;
                case OldSettingsMigrator.MigrationResults.Error:
                    MessageBox.Show("An unexpected error occured while migrating settings.", "Migration", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case OldSettingsMigrator.MigrationResults.NothingToMigrate:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Close();
        }
    }
}
