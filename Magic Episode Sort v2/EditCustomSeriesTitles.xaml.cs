using System;
using System.Collections.Generic;
using System.Globalization;
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
    public partial class EditCustomSeriesTitles : Window
    {
        List<SeriesTitle> seriesTitles = new List<SeriesTitle>();
        List<SeriesTitle> filteredSeriesTitles = new List<SeriesTitle>();
        SeriesTitle? selectedTitle;

        bool newTitles = false;

        public EditCustomSeriesTitles(List<VideoFile> videoFiles)
        {
            InitializeComponent();

            this.Title = "Magic Episode Sort > New Series Titles Found";
            newTitles = true;

            seriesTitles = SettingsManager.CustomSeriesTitleManager.GetNewSeriesTitles(videoFiles);
            filteredSeriesTitles = seriesTitles;

            lstNewTitles.ItemsSource = filteredSeriesTitles;
            chkGroupCustomTitles.Visibility = Visibility.Hidden;
        }

        public EditCustomSeriesTitles()
        {
            InitializeComponent();

            this.Title = "Magic Episode Sort > Edit Custom Series Titles";
            newTitles = false;

            seriesTitles = SettingsManager.CustomSeriesTitleManager.GetAllCustomSeriesTitles();
            filteredSeriesTitles = seriesTitles;

            RefreshTitlesList();

            firstRow.Height = new GridLength(0);
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
                List<string> originalTitlesToUpdate = new List<string>();
                originalTitlesToUpdate.Add(selectedTitle.OriginalTitle);

                if (!newTitles && chkGroupCustomTitles.IsChecked.GetValueOrDefault(false))
                    originalTitlesToUpdate.AddRange(
                        seriesTitles.Where(p => p.CustomTitle.ToLower() == selectedTitle.CustomTitle.ToLower())
                        .Select(p => p.OriginalTitle));

                foreach (string original in originalTitlesToUpdate)
                {
                    SettingsManager.CustomSeriesTitleManager.UpdateCustomSeriesTitle(original, txtCustomTitle.Text);
                    foreach (SeriesTitle newSeriesTitle in seriesTitles.Where(p => p.OriginalTitle.ToLower() == original.ToLower()))
                        newSeriesTitle.CustomTitle = txtCustomTitle.Text;
                }

                RefreshTitlesList();
            }

            txtCustomTitle.IsEnabled = false;
            btnCustomTitle.IsEnabled = false;
            txtCustomTitle.Text = "";
        }

        private void chkHideUpdated_Click(object sender, RoutedEventArgs e)
        {
            RefreshTitlesList();
        }

        private void RefreshTitlesList()
        {
            bool hideUpdatedTitles = chkHideUpdated.IsChecked.GetValueOrDefault(false);
            bool groupCustomTitles = !newTitles && chkGroupCustomTitles.IsChecked.GetValueOrDefault(false);

            if (!hideUpdatedTitles)
                filteredSeriesTitles = seriesTitles;
            else
                filteredSeriesTitles = seriesTitles.Where(p => !p.TitleChanged).ToList();

            if (groupCustomTitles)
                filteredSeriesTitles = filteredSeriesTitles.DistinctBy(p => p.CustomTitle).ToList();

            if (!newTitles)
                filteredSeriesTitles = filteredSeriesTitles.OrderBy(p => p.CustomTitle).ToList();

            lstNewTitles.ItemsSource = filteredSeriesTitles;
            lstNewTitles.Items.Refresh();
        }

        private void txtCustomTitle_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnCustomTitle_Click(sender, null);
            } 
            else if (e.Key == Key.Escape)
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                txtCustomTitle.Text = textInfo.ToTitleCase(selectedTitle.OriginalTitle);
            }
        }
    }
}
