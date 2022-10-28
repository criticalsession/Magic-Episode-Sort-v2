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

        void RetrieveData()
        {
            StartSearch();
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
            new Thread(() => RetrieveData()).Start();
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

            FinishedSearch();

            //TestEvents ev = new TestEvents();
            //ev.DirectorySearched += OnDirectorySearched;
            //ev.DoAThing();
        }

        //int totalDirectories = 0;
        //private void OnDirectorySearched(object? sender, System.EventArgs e)
        //{
        //    totalDirectories++;
        //    this.Dispatcher.Invoke(() =>
        //    {
        //        lblDirectoriesSearched.Text = "Directories Searched: " + totalDirectories.ToString();
        //    });
        //}

        private void FinishedSearch()
        {
            this.Dispatcher.Invoke(() =>
            {
                progressBar.IsIndeterminate = false;
            });

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
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            StartSearch();
        }
    }
}
