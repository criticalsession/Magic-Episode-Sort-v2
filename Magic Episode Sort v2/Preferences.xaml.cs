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

            chkAskForNewSeriesNames.IsChecked = SettingsManager.AskForNewSeriesNames;
            chkRecursiveSearchSubFolders.IsChecked = SettingsManager.RecursiveSearchSubFolders;
            chkSearchSubFolders.IsChecked = SettingsManager.SearchSubFolders;
            chkOpenOutputDirectory.IsChecked = SettingsManager.OpenOutputDirectoryAfterSort;
            chkUseTVMazeAPI.IsChecked = SettingsManager.UseTVMazeAPI;
            txtTargetDirectory.Text = SettingsManager.OutputDirectory;

            if (!chkSearchSubFolders.IsChecked.GetValueOrDefault(false))
            {
                chkRecursiveSearchSubFolders.IsChecked = false;
                chkRecursiveSearchSubFolders.IsEnabled = false;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager.AskForNewSeriesNames = chkAskForNewSeriesNames.IsChecked.GetValueOrDefault(false);
            SettingsManager.RecursiveSearchSubFolders = chkRecursiveSearchSubFolders.IsChecked.GetValueOrDefault(false);
            SettingsManager.SearchSubFolders = chkSearchSubFolders.IsChecked.GetValueOrDefault(false);
            SettingsManager.OpenOutputDirectoryAfterSort = chkOpenOutputDirectory.IsChecked.GetValueOrDefault(false);
            SettingsManager.UseTVMazeAPI = chkUseTVMazeAPI.IsChecked.GetValueOrDefault(false);
            SettingsManager.OutputDirectory = txtTargetDirectory.Text;

            SettingsManager.SaveSettings();
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

        private void chkSearchSubFolders_Click(object sender, RoutedEventArgs e)
        {
            if (!chkSearchSubFolders.IsChecked.GetValueOrDefault(false))
            {
                chkRecursiveSearchSubFolders.IsChecked = false;
                chkRecursiveSearchSubFolders.IsEnabled = false;
            } 
            else
            {
                chkRecursiveSearchSubFolders.IsEnabled = true;
            }
        }
    }
}
