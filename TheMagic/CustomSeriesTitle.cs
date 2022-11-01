using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheMagic
{
    public class SeriesTitle
    {
        public string OriginalTitle { get; set; }
        public string CustomTitle { get; set; }
        public bool IsNew { get; set; }

        public SeriesTitle(string videoFileName)
        {
            OriginalTitle = GetSeriesTitleFromFileName(videoFileName);
            CustomTitle = OriginalTitle;
            IsNew = true;
        }

        private string GetSeriesTitleFromFileName(string fileName)
        {
            string seriesName = String.Empty;
            foreach (string regex in Settings.Regexes)
            {
                Match match = Regex.Match(fileName, regex);
                if (match.Success)
                {
                    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                    seriesName = fileName.Substring(0, match.Index).Replace(".", " ").Trim();
                    seriesName = textInfo.ToTitleCase(seriesName.ToLower());

                    if (seriesName.EndsWith(" -")) seriesName = seriesName.Substring(0, seriesName.LastIndexOf("-") - 1);
                    if (seriesName.EndsWith("-")) seriesName = seriesName.Substring(0, seriesName.Length - 1);
                    if (seriesName.EndsWith(".")) seriesName = seriesName.Substring(0, seriesName.Length - 1);
                    seriesName = seriesName.Replace(":", "");
                    seriesName = Regex.Replace(seriesName, @"\s+", " "); // replace all extra spaces

                    break;
                }
            }

            return seriesName;
        }

    }
}
