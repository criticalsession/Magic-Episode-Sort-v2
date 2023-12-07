using System;
using System.Collections.Generic;
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
using TheMagic;

namespace Magic_Episode_Sort_v2
{
    /// <summary>
    /// Interaction logic for SortFailed.xaml
    /// </summary>
    public partial class SortFailed : Window
    {
        List<VideoFile> failedVideoFiles = new List<VideoFile>();

        public SortFailed(List<VideoFile> failedVideoFiles)
        {
            InitializeComponent();
            this.failedVideoFiles = failedVideoFiles;
        }

        private void btnClosePopup_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            lstAlreadyExist.ItemsSource = failedVideoFiles.Where(p => p.MoveError == EpisodeMover.MoveErrors.FileAlreadyExists).Select(p => p.SourcePath);
            lstNotFound.ItemsSource = failedVideoFiles.Where(p => p.MoveError == EpisodeMover.MoveErrors.FileDoesNotExist).Select(p => p.SourcePath);

            if (failedVideoFiles.Any(p => p.MoveError == EpisodeMover.MoveErrors.CouldNotDeleteDirectory))
            {
                this.Height = 510;
                txtDeleteError.Visibility = Visibility.Visible;
                lstDeleteError.Visibility = Visibility.Visible;
                lstDeleteError.ItemsSource = failedVideoFiles.Where(p => p.MoveError == EpisodeMover.MoveErrors.CouldNotDeleteDirectory).Select(p => p.SourcePath);
            }
            else
            {
                this.Height = 375;
                txtDeleteError.Visibility = Visibility.Collapsed;
                lstDeleteError.Visibility = Visibility.Collapsed;
            }
        }
    }
}
