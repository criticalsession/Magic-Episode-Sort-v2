using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using TheMagic;

namespace Magic_Episode_Sort_v2
{
    public partial class EditSources : Window
    {
        public EditSources()
        {
            InitializeComponent();
            lstSources.ItemsSource = Settings.SourceDirectories;
        }

        private void btnAddSource_Click(object sender, RoutedEventArgs e)
        {
            Settings.AddSourceDirectory(txtAddSource.Text);
            Settings.SaveSettings();

            txtAddSource.Text = "";
            btnAddSource.IsEnabled = false;

            lstSources.Items.Refresh();
        }

        private void btnAddSourceDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
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

            this.Focus();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //FillSources();
        }

        private void lstSources_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void ctxDeleteSource_Click(object sender, RoutedEventArgs e)
        {
            if (lstSources.SelectedItem != null)
            {
                Settings.RemoveDirectory(lstSources.SelectedItem.ToString());
                Settings.SaveSettings();

                lstSources.Items.Refresh();
            }
        }
    }
}
