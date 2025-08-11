using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using TheMagic;

namespace Magic_Episode_Sort_v2
{
    public partial class EditSkipDirectories : Window
    {
        public EditSkipDirectories()
        {
            InitializeComponent();
            lstDirectories.ItemsSource = SettingsManager.DirectoriesManager.SkipDirectoryPaths;
        }

        private void btnAddDirectory_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.DirectoriesManager.AddSkipDirectory(txtAddDirectory.Text);

            txtAddDirectory.Text = "";
            btnAddDirectory.IsEnabled = false;

            lstDirectories.ItemsSource = SettingsManager.DirectoriesManager.SkipDirectoryPaths;
        }

        private void btnSelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                Multiselect = false,
                EnsurePathExists = true,
                Title = "Select Directory to Skip"
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtAddDirectory.Text = dialog.FileName;
                btnAddDirectory.IsEnabled = true;
            }
            else
            {
                txtAddDirectory.Text = "";
                btnAddDirectory.IsEnabled = false;
            }

            Focus();
        }

        private void ctxDeleteDirectory_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = lstDirectories.SelectedItem;
            if (selectedItem == null) return;
            
            SettingsManager.DirectoriesManager.RemoveSkipDirectory(selectedItem.ToString());
            lstDirectories.ItemsSource = SettingsManager.DirectoriesManager.SkipDirectoryPaths;
        }
    }
}
