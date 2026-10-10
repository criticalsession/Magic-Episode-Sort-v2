using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using TheMagic;

namespace Magic_Episode_Sort_v2
{
    public partial class MainWindow : Window
    {
        private Directree _directories = new();
        private readonly Stopwatch _stopwatch = new();
        private bool _startedEpisodeTitleSearch;

        public MainWindow()
        {
            InitializeComponent();

            if (SettingsManager.FirstTime)
            {
                var firstTime = new FirstTime();
                firstTime.ShowDialog();

                if (!firstTime.MigrationComplete)
                {
                    OpenPreferences();
                }
            }
            else
            {
                SettingsManager.RunDBUpdates();
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (!SettingsManager.SettingsChanged) return;
            
            new Thread(StartSearch).Start();
            SettingsManager.SettingsChanged = false;
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            if (_directories.SearchComplete)
            {
                new Thread(StartSort).Start();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            new Thread(StartSearch).Start();
        }

        private void RefreshEpisodeList()
        {
            this.Dispatcher.Invoke(() =>
            {
                lstFiles.Items.Refresh();
            });

            UpdateStatusBar();

            if (_directories.VideoFiles.Count == 0)
                UpdateNoEpisodesFound();
        }

        private VideoFile? GetSelectedEpisode()
        {
            return lstFiles.SelectedItem as VideoFile;
        }

        private DateTime _lastTimeUpdate = DateTime.MinValue;
        private void UpdateStatusBar(bool forceRender = false)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (!_startedEpisodeTitleSearch || forceRender)
                {
                    lblSeriesFound.Text = "Series: " + _directories.DistinctSeriesTitles.Count;
                }

                if (DateTime.Now.Subtract(_lastTimeUpdate).TotalSeconds >= 1 || !_startedEpisodeTitleSearch || forceRender)
                {
                    lblEpisodesFound.Text = "Episodes: " + _directories.TotalVideoFiles
                        + $" (⌚ {(_stopwatch.ElapsedMilliseconds > 1000 ? (_stopwatch.ElapsedMilliseconds / 1000.0).ToString("N1") + "s" : _stopwatch.ElapsedMilliseconds + "ms")})";

                    _lastTimeUpdate = DateTime.Now;
                }
            });
        }

        private void UpdateNoEpisodesFound()
        {
            Dispatcher.Invoke(() =>
            {
                btnSort.IsEnabled = false;
                lblStatus.Text = "No new video files found.";
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

        private void EditCustomTitles_Click(object sender, RoutedEventArgs e)
        {
            new EditCustomSeriesTitles().ShowDialog();
        }

        private void EditSkipDirectories_Click(object sender, RoutedEventArgs e)
        {
            new EditSkipDirectories().ShowDialog();
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

        private void Github_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/criticalsession/Magic-Episode-Sort-v2") { UseShellExecute = true });
        }
        #endregion

        #region *** Search ***
        private void StartSearch()
        {
            _stopwatch.Reset();
            _stopwatch.Start();
            _startedEpisodeTitleSearch = false;

            if (SettingsManager.DirectoriesManager.SourceDirectories.Count > 0)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.IsEnabled = false;
                    progressBar.IsIndeterminate = true;
                    lblStatus.Text = "Searching...";
                    lblSeriesFound.Text = "Series: --";
                    lblEpisodesFound.Text = "Episodes: --";
                });

                _directories = new Directree();
                _directories.DirectorySearched += (sender, e) => OnDirectorySearched();
                _directories.FoundVideoFile += (sender, e) => OnFoundVideoFile();
                _directories.FillingCustomSeriesTitles += (sender, e) =>
                {
                    this._startedEpisodeTitleSearch = true;

                    this.Dispatcher.Invoke(() =>
                    {
                        lblStatus.Text = SettingsManager.UseTVMazeAPI ? "Fetching TV Maze API Series Titles..." : "Loading Custom Series Titles...";
                    });
                };

                _directories.UpdateStatus += (sender, e) =>
                {
                    UpdateStatusBar();
                };

                _directories.InitializeAndPopulateVideoData(SettingsManager.DirectoriesManager.SourceDirectories,
                    SettingsManager.SearchSubFolders, SettingsManager.RecursiveSearchSubFolders);

                FinishedSearch();
            }
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.IsEnabled = true;
                    lstFiles.ItemsSource = Array.Empty<string>();
                    btnSort.IsEnabled = false;
                    lblStatus.Text = "No sources set.";
                    lblDirectoriesSearched.Text = "";
                    lblEpisodesFound.Text = "Episodes: --";
                    lblSeriesFound.Text = "Series: --";

                    var result = MessageBox.Show("There are no sources set. Without sources Magic Episode Sort doesn't know which directories to search.\r\n\r\nWould you like to set up sources now?",
                        "No Sources", MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        new EditSources().ShowDialog();
                    }
                });
            }
        }

        private void OnDirectorySearched()
        {
            Dispatcher.Invoke(() =>
            {
                lblDirectoriesSearched.Text = "Directories Searched: " + _directories.TotalDirectories;
                UpdateStatusBar();
            });
        }

        private void OnFoundVideoFile()
        {
            UpdateStatusBar();
        }

        private void FinishedSearch()
        {
            _stopwatch.Stop();
            if (string.IsNullOrEmpty(SettingsManager.OutputDirectory))
            {
                Dispatcher.Invoke(() =>
                {
                    btnSort.IsEnabled = false;
                    lblStatus.Text = "No output directory set.";
                    var result = MessageBox.Show("No output directory set! Without an output directory Magic Episode Sort won't know where to sort the files.\r\n\r\nWould you like to set it up now?",
                        "No Output Directory",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        OpenPreferences();
                    }
                });
            }
            else
            {
                if (_directories.VideoFiles.Count > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        btnSort.IsEnabled = true;
                        lblStatus.Text = "";
                    });
                }
                else
                {
                    UpdateNoEpisodesFound();
                }
            }

            Dispatcher.Invoke(() =>
            {
                IsEnabled = true;
                progressBar.IsIndeterminate = false;
                lstFiles.ItemsSource = _directories.VideoFiles;

                var newTitles = SettingsManager.CustomSeriesTitleManager.GetNewSeriesTitles(_directories.VideoFiles);
                if (newTitles.Count > 0 && SettingsManager.AskForNewSeriesNames)
                    new EditCustomSeriesTitles(_directories.VideoFiles).ShowDialog();
            });

            UpdateStatusBar(true);
        }
        #endregion

        #region *** Sort ***
        private void StartSort()
        {
            Dispatcher.Invoke(() =>
            {
                IsEnabled = false;
                progressBar.IsIndeterminate = true;
                lblStatus.Text = "Sorting...";
            });

            new Thread(() =>
            {
                try
                {
                    var targetree = new Targetree();
                    var targetreeResult = targetree.BuildDirectoryTreeInTarget(_directories.VideoFiles, SettingsManager.OutputDirectory);

                    if (targetreeResult)
                    {
                        var episodeMover = new EpisodeMover();
                        episodeMover.MoveEpisodeFiles(_directories.VideoFiles);
                    }


                    FinishedSort();
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        btnSort.IsEnabled = false;
                    });

                    StartSearch();

                    MessageBox.Show(
                        $"An unexpected error has occured while sorting:{Environment.NewLine}{Environment.NewLine}{ex.Message}", "Error While Sorting", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }).Start();
        }

        private void FinishedSort()
        {
            Dispatcher.Invoke(() =>
            {
                btnSort.IsEnabled = false;
                lblStatus.Text = "Sort Complete, Refreshing...";
            });

            SettingsManager.SaveSettings();

            Thread.Sleep(200);

            if (_directories.VideoFiles.Any(p => p.MoveError != EpisodeMover.MoveErrors.None))
            {
                Dispatcher.Invoke(() =>
                {
                    new SortFailed(_directories.VideoFiles.Where(p => p.MoveError != EpisodeMover.MoveErrors.None).ToList()).ShowDialog();
                });
            }

            if (SettingsManager.OpenOutputDirectoryAfterSort)
                Process.Start("explorer.exe", SettingsManager.OutputDirectory);

            StartSearch();
        }
        #endregion

        private void ctxIgnoreEpisode_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedEpisode();
            if (selected == null) return;

            _directories.VideoFiles.Remove(selected);
            RefreshEpisodeList();

            MessageBox.Show("Episode file skipped until next refresh", "Skip Episode", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ctxIgnoreSeries_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedEpisode();
            if (selected == null) return;
            
            _directories.VideoFiles.RemoveAll(p => p.SeriesTitle.CustomTitle == selected.SeriesTitle.CustomTitle || p.SeriesTitle.OriginalTitle == selected.SeriesTitle.OriginalTitle);
            RefreshEpisodeList();

            MessageBox.Show($"Entire series '{selected.SeriesTitle.CustomTitle}' skipped until next refresh", "Skip Series", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ctxIgnoreDirectory_Click(object sender, RoutedEventArgs e)
        {
            var selected = GetSelectedEpisode();
            if (selected == null) return;
            
            SettingsManager.DirectoriesManager.AddSkipDirectory(selected.SourceDirectory);
            RefreshEpisodeList();

            MessageBox.Show(
                $"Directory '{selected.SourceDirectory}' added to 'Skip Directory' list. Go to Edit > Skip Directories to edit skipped directories.", "Skip Directory", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
