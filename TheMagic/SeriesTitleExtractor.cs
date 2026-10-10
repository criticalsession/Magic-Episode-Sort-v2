using System.Globalization;
using System.Text.RegularExpressions;

namespace TheMagic; 

public class SeriesTitleExtractor
{
    public SeriesTitle Extract(string videoFileName)
    {
        var originalTitle = GetSeriesTitleFromFileName(videoFileName);
        return new SeriesTitle()
        {
            OriginalTitle = originalTitle,
            CustomTitle = originalTitle,
            IsNew = false
        };
    }

    private string GetSeriesTitleFromFileName(string fileName)
    {
        var seriesName = string.Empty;
        foreach (var regex in SettingsManager.Regexes)
        {
            var match = Regex.Match(fileName, regex);
            if (match.Success)
            {
                var textInfo = new CultureInfo("en-US", false).TextInfo;

                seriesName = fileName.Substring(0, match.Index).Replace(".", " ").Trim();
                seriesName = textInfo.ToTitleCase(seriesName.ToLower());

                if (seriesName.EndsWith(" -")) seriesName = seriesName.Substring(0, seriesName.LastIndexOf("-") - 1);
                if (seriesName.EndsWith("-")) seriesName = seriesName.Substring(0, seriesName.Length - 1);
                if (seriesName.EndsWith(".")) seriesName = seriesName.Substring(0, seriesName.Length - 1);
                seriesName = Utils.Sanitize(seriesName);

                break;
            }
        }

        return seriesName;
    }
}
