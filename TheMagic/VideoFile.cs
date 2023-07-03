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
        public string Extension { get; set; }
        public string? EpisodeName { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }

        public string NewFileName
        {
            get
            {
                if (SeasonNumber > 0 && EpisodeNumber > 0 && !String.IsNullOrEmpty(EpisodeName))
                    return Utils.Sanitize(String.Format("{0} - S{1}E{2} - {3}{4}", SeriesTitle.CustomTitle, SeasonNumber.ToString().PadLeft(2, '0'), EpisodeNumber.ToString().PadLeft(2, '0'), EpisodeName, Extension));
                else
                    return FileName;
            }
        }

        public string SourceDirectory
        {
            get
            {
                if (String.IsNullOrEmpty(SourcePath)) return "";
                else
                {
                    string? directory = Path.GetDirectoryName(SourcePath);
                    if (String.IsNullOrEmpty(directory)) return "";
                    else return directory;
                }
            }
        }

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
            EpisodeNumber = GetEpisodeNumberFromFileName().GetValueOrDefault(0);
            EpisodeName = null;
            Extension = Path.GetExtension(SourcePath).ToLower();
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

        private int? GetEpisodeNumberFromFileName()
        {
            foreach (string regex in SettingsManager.Regexes)
            {
                Match match = Regex.Match(FileName, regex);
                if (match.Success)
                {
                    string matched = match.Value.ToLower();
                    if (regex.Contains("e")) //SDDEDD
                    {
                        matched = matched.Substring(matched.IndexOf("e") + 1);

                        // if the matched result contains "e" or "-", then it's a double episode
                        // get the first episode number
                        if (matched.Contains("e") || matched.Contains("-"))
                        {
                            matched = matched.Substring(0, matched.IndexOf(matched.Contains("e") ? "e" : "-"));
                        }

                        return int.Parse(matched);
                    }
                    else if (regex.Contains("x")) //DDXDD
                    {
                        matched = matched.Substring(matched.IndexOf("x") + 1);
                        return int.Parse(matched);
                    }
                }
            }

            return null;
        }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(EpisodeName))
            {
                return String.Format("{0} > S{1}{2} > {3}", SeriesTitle.CustomTitle, SeasonNumber.ToString().PadLeft(2, '0'), EpisodeNumber > 0 ? "E" + EpisodeNumber : "", FileName);
            }
            else
            {
                return String.Format("{0} > S{1}E{2} > \"{3}\"", SeriesTitle.CustomTitle, SeasonNumber.ToString().PadLeft(2, '0'), EpisodeNumber.ToString().PadLeft(2, '0'), EpisodeName);
            }
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
