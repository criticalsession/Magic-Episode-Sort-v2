using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using TheMagic;

namespace Magic_Episode_Sort_v2
{
    public partial class EditSources : Window
    {
        public EditSources()
        {
            InitializeComponent();
            lstSources.ItemsSource = SettingsManager.DirectoriesManager.SourceDirectoryPaths;
        }

        private void btnAddSource_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.DirectoriesManager.AddSourceDirectory(txtAddSource.Text);

            txtAddSource.Text = "";
            btnAddSource.IsEnabled = false;

            lstSources.ItemsSource = SettingsManager.DirectoriesManager.SourceDirectoryPaths;
        }

        private void btnAddSourceDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                Multiselect = false,
                EnsurePathExists = true,
                Title = "Select Source Directory"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtAddSource.Text = dialog.FileName;
                btnAddSource.IsEnabled = true;
            }
            else
            {
                txtAddSource.Text = "";
                btnAddSource.IsEnabled = false;
            }

            Focus();
        }

        private void ctxDeleteSource_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstSources.SelectedItem;
            if (selectedItem == null) return;
            
            SettingsManager.DirectoriesManager.RemoveSourceDirectory(selectedItem.ToString());
            lstSources.ItemsSource = SettingsManager.DirectoriesManager.SourceDirectoryPaths;
        }
    }
}
