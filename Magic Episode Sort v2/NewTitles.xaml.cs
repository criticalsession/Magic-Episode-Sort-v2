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
    /// Interaction logic for NewTitles.xaml
    /// </summary>
    public partial class NewTitles : Window
    {
        List<SeriesTitle> newSeriesTitles = new List<SeriesTitle>();
        SeriesTitle? selectedTitle;

        public NewTitles(List<VideoFile> videoFiles)
        {
            InitializeComponent();
            newSeriesTitles = SettingsManager.CustomSeriesTitleManager.GetNewSeriesTitles(videoFiles);

            lstNewTitles.ItemsSource = newSeriesTitles;
        }

        private void lstNewTitles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveCustomTitle();
            selectedTitle = lstNewTitles.SelectedItem as SeriesTitle;

            if (selectedTitle != null)
            {
                txtCustomTitle.IsEnabled = true;
                btnCustomTitle.IsEnabled = true;

                txtCustomTitle.Text = selectedTitle.CustomTitle;
            } 
            else
            {
                txtCustomTitle.IsEnabled = false;
                btnCustomTitle.IsEnabled = false;
            }
        }

        private void btnCustomTitle_Click(object sender, RoutedEventArgs e)
        {
            lstNewTitles.SelectedItem = null;
        }

        private void SaveCustomTitle()
        {
            if (selectedTitle != null)
            {
                SettingsManager.CustomSeriesTitleManager.ReplaceOrAddCustomSeriesTitle(selectedTitle.OriginalTitle, txtCustomTitle.Text);
                foreach (SeriesTitle newSeriesTitle in newSeriesTitles.Where(p => p.OriginalTitle.ToLower() == selectedTitle.OriginalTitle.ToLower()))
                    newSeriesTitle.CustomTitle = txtCustomTitle.Text;

                lstNewTitles.Items.Refresh();
            }

            txtCustomTitle.IsEnabled = false;
            btnCustomTitle.IsEnabled = false;
            txtCustomTitle.Text = "";
        }
    }
}
