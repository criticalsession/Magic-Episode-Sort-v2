using System.Threading;
using System.Windows;
using TheMagic;

namespace Magic_Episode_Sort_v2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void RetrieveData()
        {
            StartSearch();
            FinishedSearch();
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            new About().ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //TODO: if first time
            new FirstTime().ShowDialog();
            //TODO: load preferences window

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
                lblStatus.Text = "Search Complete";
                btnSort.IsEnabled = true;
                progressBar.IsIndeterminate = false;
            });
        }
    }
}
