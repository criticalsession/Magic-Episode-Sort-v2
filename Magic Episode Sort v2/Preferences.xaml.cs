using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Media.Animation;

namespace Magic_Episode_Sort_v2
{
    public partial class Preferences : Window
    {
        public Preferences()
        {
            InitializeComponent();

            chkAskForNewSeriesNames.IsChecked = Settings.AskForNewSeriesNames;
            chkRecursiveSearchSubFolders.IsChecked = Settings.RecursiveSearchSubFolders;
            chkSearchSubFolders.IsChecked = Settings.SearchSubFolders;
            txtTargetDirectory.Text = Settings.TargetDirectory;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Settings.AskForNewSeriesNames = chkAskForNewSeriesNames.IsChecked.GetValueOrDefault(false);
            Settings.RecursiveSearchSubFolders = chkRecursiveSearchSubFolders.IsChecked.GetValueOrDefault(false);
            Settings.SearchSubFolders = chkSearchSubFolders.IsChecked.GetValueOrDefault(false);
            Settings.TargetDirectory = txtTargetDirectory.Text;

            Settings.SaveSettings();
            this.Close();
        }

        private void SelectTargetDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                txtTargetDirectory.Text = dialog.FileName;
                this.Focus();
            }
        }

        private void btnEditSources_Click(object sender, RoutedEventArgs e)
        {
            new EditSources().ShowDialog();
        }
    }
}
