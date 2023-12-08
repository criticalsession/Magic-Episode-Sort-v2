using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Stopwatch stopwatch = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();

            if (SettingsManager.FirstTime)
            {
                FirstTime firsttime = new FirstTime();
                firsttime.ShowDialog();

                if (!firsttime.migrationComplete)
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
            if (SettingsManager.SettingsChanged)
            {
                new Thread(() => StartSearch()).Start();
                SettingsManager.SettingsChanged = false;
            }
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            if (directories.SearchComplete)
            {
                new Thread(() => StartSort()).Start();
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            new Thread(() => StartSearch()).Start();
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
                lblEpisodesFound.Text = "Episodes: " + directories.cVideoFiles.Count.ToString()
                    + String.Format(" (⌚ {0})", stopwatch.ElapsedMilliseconds > 1000 ? (stopwatch.ElapsedMilliseconds / 1000.0).ToString("N1") + "s" : stopwatch.ElapsedMilliseconds + "ms");
                lblSeriesFound.Text = "Series: " + directories.DistinctSeriesTitles.Count.ToString();
            });
        }

        private void UpdateNoEpisodesFound()
        {
            this.Dispatcher.Invoke(() =>
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
            System.Diagnostics.Process.Start(new ProcessStartInfo("https://github.com/criticalsession/Magic-Episode-Sort-v2") { UseShellExecute = true });
        }
        #endregion

        #region *** Search ***
        private void StartSearch()
        {
            stopwatch.Reset();
            stopwatch.Start();

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

                directories = new Directree();
                directories.DirectorySearched += (sender, e) => OnDirectorySearched();
                directories.FoundVideoFile += (sender, e) => OnFoundVideoFile();
                directories.FillingCustomSeriesTitles += (sender, e) =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (SettingsManager.UseTVMazeAPI)
                        {
                            lblStatus.Text = "Fetching TV Maze API Series Titles...";
                        }
                        else
                        {
                            lblStatus.Text = "Loading Custom Series Titles...";
                        }
                    });
                };

                directories.UpdateStatus += (sender, e) =>
                {
                    UpdateStatusBar();
                };

                directories.InitializeAndPopulateVideoData(SettingsManager.DirectoriesManager.SourceDirectories, 
                    SettingsManager.SearchSubFolders, SettingsManager.RecursiveSearchSubFolders);

                FinishedSearch();
            } 
            else
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.IsEnabled = true;
                    lstFiles.ItemsSource = new string[0];
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
            stopwatch.Stop();
            if (String.IsNullOrEmpty(SettingsManager.OutputDirectory))
            {
                this.Dispatcher.Invoke(() =>
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
                if (directories.VideoFiles.Count > 0)
                {
                    this.Dispatcher.Invoke(() =>
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

            this.Dispatcher.Invoke(() =>
            {
                this.IsEnabled = true;
                progressBar.IsIndeterminate = false;
                lstFiles.ItemsSource = directories.VideoFiles;

                List<SeriesTitle> newTitles = SettingsManager.CustomSeriesTitleManager.GetNewSeriesTitles(directories.VideoFiles);
                if (newTitles.Count > 0 && SettingsManager.AskForNewSeriesNames)
                    new EditCustomSeriesTitles(directories.VideoFiles).ShowDialog();
            });

            UpdateStatusBar();
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
                try
                {
                    Targetree targetree = new Targetree();
                    bool targetreeResult = targetree.BuildDirectoryTreeInTarget(directories.VideoFiles, SettingsManager.OutputDirectory);

                    if (targetreeResult)
                    {
                        EpisodeMover epmover = new EpisodeMover();
                        epmover.MoveEpisodeFiles(directories.VideoFiles);
                    }


                    FinishedSort();
                } 
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        btnSort.IsEnabled = false;
                    });

                    StartSearch();

                    MessageBox.Show("An unexpected error has occured while sorting:" + Environment.NewLine + Environment.NewLine + ex.Message, "Error While Sorting", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }).Start();
        }

        private void FinishedSort()
        {
            this.Dispatcher.Invoke(() =>
            {
                btnSort.IsEnabled = false;
                lblStatus.Text = "Sort Complete, Refreshing...";
            });

            SettingsManager.SaveSettings();

            Thread.Sleep(200);

            if (directories.VideoFiles.Any(p => p.MoveError != EpisodeMover.MoveErrors.None))
            {
                this.Dispatcher.Invoke(() =>
                {
                    new SortFailed(directories.VideoFiles.Where(p => p.MoveError != EpisodeMover.MoveErrors.None).ToList()).ShowDialog();
                });
            }

            if (SettingsManager.OpenOutputDirectoryAfterSort)
                Process.Start("explorer.exe", SettingsManager.OutputDirectory); 

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

                MessageBox.Show("Episode file skipped until next refresh", "Skip Episode", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ctxIgnoreSeries_Click(object sender, RoutedEventArgs e)
        {
            VideoFile? selected = GetSelectedEpisode();
            if (selected != null)
            {
                directories.VideoFiles.RemoveAll(p => p.SeriesTitle.CustomTitle == selected.SeriesTitle.CustomTitle || p.SeriesTitle.OriginalTitle == selected.SeriesTitle.OriginalTitle);
                RefreshEpisodeList();

                MessageBox.Show("Entire series '" + selected.SeriesTitle.CustomTitle + "' skipped until next refresh", "Skip Series", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ctxIgnoreDirectory_Click(object sender, RoutedEventArgs e)
        {
            VideoFile? selected = GetSelectedEpisode();
            if (selected != null)
            {
                SettingsManager.DirectoriesManager.AddSkipDirectory(selected.SourceDirectory);
                RefreshEpisodeList();

                MessageBox.Show("Directory '" + selected.SourceDirectory + "' added to 'Skip Directory' list. Go to Edit > Skip Directories to edit skipped directories.", "Skip Directory", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
