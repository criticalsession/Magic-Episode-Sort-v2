using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheMagic
{
    public class VideoFile
    {
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public string OriginalSeriesName { get; set; }
        public string CustomSeriesName { get; set; }
        public string FileName { get; set; }
        public int SeasonNumber { get; set; }
        
        private bool IsVideoFileExtension
        {
            get
            {
                return Settings.Extensions.Contains(Path.GetExtension(SourcePath).ToLower());
            }
        }

        private bool RegexMatches
        {
            get
            {
                foreach (string regex in Settings.Regexes)
                {
                    if (Regex.Match(FileName, regex).Success) return true;
                }

                return false;
            }
        }

        public bool IsVideoFile 
        { 
            get
            {
                return IsVideoFileExtension && RegexMatches;
            }
        }

        public string SeasonDirName
        {
            get
            {
                return "Season " + SeasonNumber.ToString().PadLeft(2, '0');
            }
        }

        public VideoFile(string path)
        {
            SourcePath = path;
            FileName = Path.GetFileName(path);
            TargetPath = path; // todo
            OriginalSeriesName = GetSeriesNameFromFileName();
            CustomSeriesName = OriginalSeriesName; // todo
            SeasonNumber = GetSeasonNumberFromFileName().GetValueOrDefault(0);
        }

        private string GetSeriesNameFromFileName()
        {
            string seriesName = String.Empty;
            foreach (string regex in Settings.Regexes)
            {
                Match match = Regex.Match(FileName, regex);
                if (match.Success)
                {
                    TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                    seriesName = FileName.Substring(0, match.Index).Replace(".", " ").Trim();
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

        private int? GetSeasonNumberFromFileName()
        {
            foreach (string regex in Settings.Regexes)
            {
                Match match = Regex.Match(FileName, regex);
                if (match.Success)
                {
                    string matched = match.Value.ToLower();
                    if (regex.Contains("e")) //SDDEDD
                    {
                        matched = matched.Replace("s", "");
                        matched = matched.Substring(0, matched.IndexOf("e"));
                        return int.Parse(matched);
                    } 
                    else if (regex.Contains("x")) //DDXDD
                    {
                        matched = matched.Substring(0, matched.IndexOf("x"));
                        return int.Parse(matched);
                    }
                }
            }

            return null;
        }

        public override string ToString()
        {
            return String.Format("{0} > {1} > {2}", CustomSeriesName, SeasonDirName, FileName);
        }
    }
}
