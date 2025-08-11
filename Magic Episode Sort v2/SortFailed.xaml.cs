using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TheMagic;

namespace Magic_Episode_Sort_v2;

/// <summary>
/// Interaction logic for SortFailed.xaml
/// </summary>
public partial class SortFailed : Window
{
    private readonly List<VideoFile> _failedVideoFiles;

    public SortFailed(List<VideoFile> failedVideoFiles)
    {
        InitializeComponent();
        this._failedVideoFiles = failedVideoFiles;
    }

    private void btnClosePopup_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void Window_Activated(object sender, EventArgs e)
    {
        lstAlreadyExist.ItemsSource = _failedVideoFiles.Where(p => p.MoveError == EpisodeMover.MoveErrors.FileAlreadyExists).Select(p => p.SourcePath);
        lstNotFound.ItemsSource = _failedVideoFiles.Where(p => p.MoveError == EpisodeMover.MoveErrors.FileDoesNotExist).Select(p => p.SourcePath);

        if (_failedVideoFiles.Any(p => p.MoveError == EpisodeMover.MoveErrors.CouldNotDeleteDirectory))
        {
            Height = 510;
            txtDeleteError.Visibility = Visibility.Visible;
            lstDeleteError.Visibility = Visibility.Visible;
            lstDeleteError.ItemsSource = _failedVideoFiles.Where(p => p.MoveError == EpisodeMover.MoveErrors.CouldNotDeleteDirectory).Select(p => p.SourcePath);
        }
        else
        {
            Height = 375;
            txtDeleteError.Visibility = Visibility.Collapsed;
            lstDeleteError.Visibility = Visibility.Collapsed;
        }
    }
}