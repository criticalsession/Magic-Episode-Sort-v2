using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using TheMagic;

namespace Magic_Episode_Sort_v2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (!Settings.SettingsFileExists)
            {
                new FirstTime().ShowDialog();
                OpenPreferences();
            }
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void EditSources_Click(object sender, RoutedEventArgs e)
        {
            new EditSources().ShowDialog();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            new About().ShowDialog();
        }

        private void Preferences_Click(object sender, RoutedEventArgs e)
        {
            OpenPreferences();
        }

        private void OpenPreferences()
        {
            new Preferences().ShowDialog();
        }

        private void StartSearch()
        {
            this.Dispatcher.Invoke(() =>
            {
                progressBar.IsIndeterminate = true;
                lblStatus.Text = "Searching...";
                lblSeriesFound.Text = "Series: --";
                lblEpisodesFound.Text = "Episodes: --";
            });

            Directree directories = new Directree();
            directories.DirectorySearched += (sender, e) => OnDirectorySearched(directories.Directories.Count);
            directories.FoundVideoFile += (sender, e) => OnFoundVideoFile(directories.VideoFiles);

            directories.Build(Settings.SourceDirectories, Settings.SearchSubFolders, Settings.RecursiveSearchSubFolders);

            FinishedSearch(directories);
        }

        private void OnDirectorySearched(int totalDirectories)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblDirectoriesSearched.Text = "Directories Searched: " + totalDirectories.ToString();
            });
        }

        private void OnFoundVideoFile(List<VideoFile> videoFiles)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblEpisodesFound.Text = "Episodes: " + videoFiles.Count.ToString();
                lblSeriesFound.Text = "Series: " + videoFiles.Select(p => p.CustomSeriesName).Distinct().ToList().Count.ToString();
            });
        }

        private void FinishedSearch(Directree directree)
        {
            if (String.IsNullOrEmpty(Settings.TargetDirectory))
            {
                this.Dispatcher.Invoke(() =>
                {
                    btnSort.IsEnabled = false;
                    lblStatus.Text = "ERROR: NO OUTPUT DIRECTORY SET!";
                });
            } 
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    btnSort.IsEnabled = true;
                    lblStatus.Text = "Search Complete";
                });
            }

            this.Dispatcher.Invoke(() =>
            {
                progressBar.IsIndeterminate = false;
                lstFiles.ItemsSource = directree.VideoFiles;
            });
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (Settings.SettingsChanged)
            {
                new Thread(() => StartSearch()).Start();
                Settings.SettingsChanged = false;
            }
        }
    }
}
