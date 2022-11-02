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
        Directree directories = new Directree();

        public MainWindow()
        {
            InitializeComponent();

            if (!Settings.SettingsFileExists)
            {
                new FirstTime().ShowDialog();
                OpenPreferences();
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (Settings.SettingsChanged)
            {
                new Thread(() => StartSearch()).Start();
                Settings.SettingsChanged = false;
            }
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            if (directories.SearchComplete)
            {
                new Thread(() => StartSort()).Start();
            }
        }

        private void RefreshEpisodeList()
        {
            this.Dispatcher.Invoke(() =>
            {
                lstFiles.Items.Refresh();
            });

            UpdateStatusBar();

            if (directories.VideoFiles.Count == 0)
                UpdateNoEpisodesFound();
        }

        private VideoFile? GetSelectedEpisode()
        {
            return lstFiles.SelectedItem == null ? null : lstFiles.SelectedItem as VideoFile;
        }

        private void UpdateStatusBar()
        {
            this.Dispatcher.Invoke(() =>
            {
                lblEpisodesFound.Text = "Episodes: " + directories.VideoFiles.Count.ToString();
                lblSeriesFound.Text = "Series: " + directories.DistingSeriesTitles.Count.ToString();
            });
        }

        private void UpdateNoEpisodesFound()
        {
            this.Dispatcher.Invoke(() =>
            {
                btnSort.IsEnabled = false;
                lblStatus.Text = "No New Episodes Found";
            });
        }

        #region *** Menu ***
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
        #endregion

        #region *** Search ***
        private void StartSearch()
        {
            if (Settings.SourceDirectories.Count > 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.IsEnabled = false;
                    progressBar.IsIndeterminate = true;
                    lblStatus.Text = "Searching...";
                    lblSeriesFound.Text = "Series: --";
                    lblEpisodesFound.Text = "Episodes: --";
                });

                directories = new Directree();
                directories.DirectorySearched += (sender, e) => OnDirectorySearched();
                directories.FoundVideoFile += (sender, e) => OnFoundVideoFile();

                directories.Build(Settings.SourceDirectories, Settings.SearchSubFolders, Settings.RecursiveSearchSubFolders);

                FinishedSearch();
            } 
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.IsEnabled = true;
                    lstFiles.ItemsSource = new string[0];
                    btnSort.IsEnabled = false;
                    lblStatus.Text = "No Sources Set";
                    lblDirectoriesSearched.Text = "";
                    lblEpisodesFound.Text = "Episodes: --";
                    lblSeriesFound.Text = "Series: --";
                });
            }
        }

        private void OnDirectorySearched()
        {
            this.Dispatcher.Invoke(() =>
            {
                lblDirectoriesSearched.Text = "Directories Searched: " + directories.Directories.Count.ToString();
            });
        }

        private void OnFoundVideoFile()
        {
            UpdateStatusBar();
        }

        private void FinishedSearch()
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
                if (directories.VideoFiles.Count > 0)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        btnSort.IsEnabled = true;
                        lblStatus.Text = "Search Complete";
                    });
                } 
                else
                {
                    UpdateNoEpisodesFound();
                }
            }

            this.Dispatcher.Invoke(() =>
            {
                this.IsEnabled = true;
                progressBar.IsIndeterminate = false;
                lstFiles.ItemsSource = directories.VideoFiles;
            });
        }
        #endregion

        #region *** Sort ***
        private void StartSort()
        {
            this.Dispatcher.Invoke(() =>
            {
                this.IsEnabled = false;
                progressBar.IsIndeterminate = true;
                lblStatus.Text = "Sorting...";
            });

            new Thread(() =>
            {
                Targetree targetree = new Targetree();
                targetree.BuildDirectoryTreeInTarget(directories.VideoFiles, Settings.TargetDirectory);

                FinishedSort();
            }).Start();
        }

        private void FinishedSort()
        {
            this.Dispatcher.Invoke(() =>
            {
                btnSort.IsEnabled = false;
                lblStatus.Text = "Sort Complete, Refreshing...";
            });

            Settings.SaveSettings();

            Thread.Sleep(1500);

            StartSearch();
        }
        #endregion

        private void ctxIgnoreEpisode_Click(object sender, RoutedEventArgs e)
        {
            VideoFile? selected = GetSelectedEpisode();
            if (selected != null)
            {
                directories.VideoFiles.Remove(selected);
                RefreshEpisodeList();
            }
        }

        private void ctxIgnoreSeries_Click(object sender, RoutedEventArgs e)
        {
            VideoFile? selected = GetSelectedEpisode();
            if (selected != null)
            {
                directories.VideoFiles.RemoveAll(p => p.SeriesTitle.CustomTitle == selected.SeriesTitle.CustomTitle || p.SeriesTitle.OriginalTitle == selected.SeriesTitle.OriginalTitle);
                RefreshEpisodeList();
            }
        }
    }
}
