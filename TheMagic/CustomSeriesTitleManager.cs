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
                return SettingsManager.settings.customSeriesTitles;
            }
        }

        public bool AddCustomSeriesTitle(string originalTitle, string customTitle)
        {
            if (!HasCustomSeriesTitle(originalTitle))
            {
                SettingsManager.settings.customSeriesTitles.Add(new SeriesTitle()
                {
                    CustomTitle = customTitle,
                    OriginalTitle = originalTitle.ToLower()
                });

                SettingsManager.SaveSettings();

                return true;
            }

            return false;
        }

        public void ReplaceOrAddCustomSeriesTitle(string originalTitle, string customTitle)
        {
            RemoveCustomSeriesTitle(originalTitle);
            AddCustomSeriesTitle(originalTitle, customTitle);
        }

        public bool RemoveCustomSeriesTitle(string originalTitle)
        {
            if (HasCustomSeriesTitle(originalTitle))
            {
                SettingsManager.settings.customSeriesTitles.RemoveAll(p => p.OriginalTitle == originalTitle.ToLower());
                SettingsManager.SaveSettings();

                return true;
            }

            return false;
        }

        public List<SeriesTitle> GetAllCustomSeriesTitles()
        {
            return CustomSeriesTitles;
        }

        public SeriesTitle? GetCustomSeriesTitle(string originalTitle)
        {
            return CustomSeriesTitles.FirstOrDefault(p => p.OriginalTitle == originalTitle.ToLower());
        }

        public List<SeriesTitle> GetNewSeriesTitles(List<VideoFile> videoFiles)
        {
            return videoFiles.Where(p => p.SeriesTitle.IsNew).Select(p => p.SeriesTitle).Distinct().ToList();
        }

        public bool HasCustomSeriesTitle(string originalTitle)
        {
            return CustomSeriesTitles.Any(p => p.OriginalTitle == originalTitle.ToLower());
        }
    }
}
