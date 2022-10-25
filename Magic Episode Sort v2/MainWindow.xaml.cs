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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            RetrieveData();
        }

        void RetrieveData()
        {
            lblDirectoriesSearched.Text = "Directories Searched: --";
            lblSeasonsFound.Text = "Seasons: --";
            lblSeriesFound.Text = "Series: --";
            lblEpisodesFound.Text = "Episodes: --";
            lblNewSeriesFound.Text = "New Series Found: --";
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            new About().ShowDialog();
        }
    }
}
