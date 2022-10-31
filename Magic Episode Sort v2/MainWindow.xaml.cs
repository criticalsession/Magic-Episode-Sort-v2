using System;
using System.Threading;
using System.Windows;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(() => StartSearch()).Start();
        }

        private void StartSearch()
        {
            this.Dispatcher.Invoke(() =>
            {
                progressBar.IsIndeterminate = true;
                lblStatus.Text = "Searching...";
                lblSeasonsFound.Text = "Seasons: --";
                lblSeriesFound.Text = "Series: --";
                lblEpisodesFound.Text = "Episodes: --";
            });

            Directree directories = new Directree();
            directories.DirectorySearched += (sender, e) => OnDirectorySearched(directories.Directories.Count);
            directories.FoundVideoFile += (sender, e) => OnFoundVideoFile(directories.VideoFiles.Count);

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

        private void OnFoundVideoFile(int totalVideoFiles)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblEpisodesFound.Text = "Episodes: " + totalVideoFiles.ToString();
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
            StartSearch();
        }
    }
}
