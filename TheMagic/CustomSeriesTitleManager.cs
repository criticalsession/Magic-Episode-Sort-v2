using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    public class CustomSeriesTitleManager
    {
        internal List<SeriesTitle> CustomSeriesTitles
        {
            get
            {
                return MESDBHandler.LoadCustomTitles();
            }
        }

        public void AddCustomSeriesTitle(string originalTitle, string customTitle, bool isNew)
        {
            MESDBHandler.SaveCustomTitle(new SeriesTitle()
            {
                CustomTitle = customTitle,
                OriginalTitle = originalTitle,
                IsNew = isNew
            });

            SettingsManager.SettingsChanged = true;
        }

        public void UpdateCustomSeriesTitle(string originalTitle, string customTitle)
        {
            AddCustomSeriesTitle(originalTitle, customTitle, false);
        }

        public void RemoveCustomSeriesTitle(string originalTitle)
        {
            MESDBHandler.DeleteCustomTitle(originalTitle);
            SettingsManager.SettingsChanged = true;
        }

        public List<SeriesTitle> GetAllCustomSeriesTitles()
        {
            return CustomSeriesTitles;
        }

        public SeriesTitle? GetCustomSeriesTitle(string originalTitle)
        {
            return CustomSeriesTitles.FirstOrDefault(p => p.OriginalTitle.ToLower() == originalTitle.ToLower());
        }

        public List<SeriesTitle> GetNewSeriesTitles(List<VideoFile> videoFiles)
        {
            return videoFiles.Where(p => p.SeriesTitle.IsNew).Select(p => p.SeriesTitle).Distinct().ToList();
        }
    }
}
