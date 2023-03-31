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
        public SeriesTitle SeriesTitle { get; set; }
        public string FileName { get; set; }
        public int SeasonNumber { get; set; }
        
        private bool IsVideoFileExtension
        {
            get
            {
                return SettingsManager.Extensions.Contains(Path.GetExtension(SourcePath).ToLower());
            }
        }

        private bool RegexMatches
        {
            get
            {
                foreach (string regex in SettingsManager.Regexes)
                {
                    if (Regex.Match(FileName, regex).Success) return true;
                }

                return false;
            }
        }

        public bool IsValidVideoFile 
        { 
            get
            {
                return IsVideoFileExtension && RegexMatches && !String.IsNullOrEmpty(SeriesTitle.OriginalTitle);
            }
        }

        public string SeasonDirName
        {
            get
            {
                return "Season " + SeasonNumber.ToString().PadLeft(2, '0');
            }
        }

        public EpisodeMover.MoveErrors MoveError { get; internal set; } = EpisodeMover.MoveErrors.None;

        public VideoFile(string path)
        {
            SourcePath = path;
            FileName = Path.GetFileName(path);
            TargetPath = String.Empty;
            SeriesTitle = new SeriesTitleExtractor().Extract(FileName);
            SeasonNumber = GetSeasonNumberFromFileName().GetValueOrDefault(0);
        }

        private int? GetSeasonNumberFromFileName()
        {
            foreach (string regex in SettingsManager.Regexes)
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
            return String.Format("{0} > {1} > {2}", SeriesTitle.CustomTitle, SeasonDirName, FileName);
        }

        public void SetCustomTitle(string? customTitle, bool isNew)
        {
            this.SeriesTitle.CustomTitle = customTitle == null ? this.SeriesTitle.OriginalTitle : customTitle;
            this.SeriesTitle.IsNew = isNew;

            if (isNew) 
                SettingsManager.CustomSeriesTitleManager.AddCustomSeriesTitle(this.SeriesTitle.OriginalTitle, this.SeriesTitle.CustomTitle, true);
        }
    }
}
