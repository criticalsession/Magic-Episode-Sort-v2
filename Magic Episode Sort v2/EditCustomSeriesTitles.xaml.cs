using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TheMagic;

namespace Magic_Episode_Sort_v2
{
    public partial class EditCustomSeriesTitles : Window
    {
        private readonly List<SeriesTitle> _seriesTitles;
        private List<SeriesTitle> _filteredSeriesTitles;
        private SeriesTitle? _selectedTitle;

        private readonly bool _newTitles;

        public EditCustomSeriesTitles(List<VideoFile> videoFiles)
        {
            InitializeComponent();

            Title = "Magic Episode Sort > New Series Titles Found";
            _newTitles = true;

            _seriesTitles = SettingsManager.CustomSeriesTitleManager.GetNewSeriesTitles(videoFiles);
            _filteredSeriesTitles = _seriesTitles;

            lstNewTitles.ItemsSource = _filteredSeriesTitles;
            chkGroupCustomTitles.Visibility = Visibility.Hidden;
        }

        public EditCustomSeriesTitles()
        {
            InitializeComponent();

            Title = "Magic Episode Sort > Edit Custom Series Titles";
            _newTitles = false;

            _seriesTitles = SettingsManager.CustomSeriesTitleManager.GetAllCustomSeriesTitles();
            _filteredSeriesTitles = _seriesTitles;

            RefreshTitlesList();

            firstRow.Height = new GridLength(0);
        }

        private void lstNewTitles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveCustomTitle();
            _selectedTitle = lstNewTitles.SelectedItem as SeriesTitle;

            if (_selectedTitle != null)
            {
                txtCustomTitle.IsEnabled = true;
                btnCustomTitle.IsEnabled = true;

                txtCustomTitle.Text = _selectedTitle.CustomTitle;
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
            if (_selectedTitle != null)
            {
                var originalTitlesToUpdate = new List<string> { _selectedTitle.OriginalTitle };

                if (!_newTitles && chkGroupCustomTitles.IsChecked.GetValueOrDefault(false))
                    originalTitlesToUpdate.AddRange(
                        _seriesTitles.Where(p => p.CustomTitle.Equals(_selectedTitle.CustomTitle, System.StringComparison.CurrentCultureIgnoreCase))
                        .Select(p => p.OriginalTitle));

                foreach (string original in originalTitlesToUpdate)
                {
                    SettingsManager.CustomSeriesTitleManager.UpdateCustomSeriesTitle(original, txtCustomTitle.Text);
                    foreach (var newSeriesTitle in _seriesTitles.Where(p => p.OriginalTitle.Equals(original, System.StringComparison.CurrentCultureIgnoreCase)))
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
            var hideUpdatedTitles = chkHideUpdated.IsChecked.GetValueOrDefault(false);
            var groupCustomTitles = !_newTitles && chkGroupCustomTitles.IsChecked.GetValueOrDefault(false);

            _filteredSeriesTitles = !hideUpdatedTitles ? _seriesTitles : _seriesTitles.Where(p => !p.TitleChanged).ToList();

            if (groupCustomTitles)
                _filteredSeriesTitles = _filteredSeriesTitles.DistinctBy(p => p.CustomTitle).ToList();

            if (!_newTitles)
                _filteredSeriesTitles = _filteredSeriesTitles.OrderBy(p => p.CustomTitle).ToList();

            lstNewTitles.ItemsSource = _filteredSeriesTitles;
            lstNewTitles.Items.Refresh();
        }

        private void txtCustomTitle_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    btnCustomTitle_Click(sender, null);
                    break;
                case Key.Escape:
                {
                    var textInfo = new CultureInfo("en-US", false).TextInfo;
                    txtCustomTitle.Text = textInfo.ToTitleCase(_selectedTitle?.OriginalTitle);
                    break;
                }
            }
        }
    }
}
