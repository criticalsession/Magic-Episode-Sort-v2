using System.Text.RegularExpressions;

namespace TheMagic {
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
        public string? ParentDirectory => Path.GetDirectoryName(SourcePath);
        public string? ParentDirectoryName => Path.GetFileName(ParentDirectory);
        private bool IsVideoFileExtension => SettingsManager.Extensions.Contains(Path.GetExtension(SourcePath).ToLower());
        public bool IsValidVideoFile => IsVideoFileExtension && RegexMatches && !string.IsNullOrEmpty(SeriesTitle.OriginalTitle);
        public string SeasonDirName => $"Season {SeasonNumber.ToString().PadLeft(2, '0')}";
        public EpisodeMover.MoveErrors MoveError { get; internal set; } = EpisodeMover.MoveErrors.None;


        public string NewFileName
        {
            get
            {
                if (SeasonNumber > 0 && EpisodeNumber > 0 && !String.IsNullOrEmpty(EpisodeName))
                    return Utils.Sanitize($"{SeriesTitle.CustomTitle} - S{SeasonNumber.ToString().PadLeft(2, '0')}E{EpisodeNumber.ToString().PadLeft(2, '0')} - {EpisodeName}{Extension}");
                else
                    return FileName;
            }
        }

        public string SourceDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(SourcePath)) return "";
                else
                {
                    string? directory = Path.GetDirectoryName(SourcePath);
                    if (string.IsNullOrEmpty(directory)) return "";
                    else return directory;
                }
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

        public VideoFile(string path)
        {
            SourcePath = path;
            FileName = Path.GetFileName(path);
            TargetPath = string.Empty;
            SeriesTitle = new SeriesTitleExtractor().Extract(FileName);
            SeasonNumber = GetSeasonNumberFromFileName().GetValueOrDefault(0);
            EpisodeNumber = GetEpisodeNumberFromFileName().GetValueOrDefault(0);
            EpisodeName = null;
            Extension = Path.GetExtension(SourcePath).ToLower();
        }

        private int? GetSeasonNumberFromFileName()
        {
            foreach (var regex in SettingsManager.Regexes)
            {
                var match = Regex.Match(FileName, regex);
                if (!match.Success) continue;

                var matched = match.Value.ToLower();
                if (regex.Contains('e')) //SDDEDD
                {
                    matched = matched.Replace("s", "");
                    matched = matched.Substring(0, matched.IndexOf("e"));
                    return int.Parse(matched);
                }
                else if (regex.Contains('x')) //DDXDD
                {
                    matched = matched[..matched.IndexOf("x")];
                    return int.Parse(matched);
                }
            }

            return null;
        }

        private int? GetEpisodeNumberFromFileName()
        {
            foreach (var regex in SettingsManager.Regexes)
            {
                var match = Regex.Match(FileName, regex);
                if (!match.Success) continue;

                var matched = match.Value.ToLower();
                if (regex.Contains('e')) //SDDEDD
                {
                    matched = matched[(matched.IndexOf("e") + 1)..];

                    // if the matched result contains "e" or "-", then it's a double episode
                    // get the first episode number
                    if (matched.Contains('e') || matched.Contains('-'))
                    {
                        var sep = matched.Contains('e') ? "e" : "-";
                        matched = matched[..matched.IndexOf(sep)].Replace("-", "");
                    }

                    return int.Parse(matched);
                }
                else if (regex.Contains('x')) //DDXDD
                {
                    matched = matched[(matched.IndexOf("x") + 1)..];
                    return int.Parse(matched);
                }
            }

            return null;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(EpisodeName))
            {
                return $"{SeriesTitle.CustomTitle} > S{SeasonNumber.ToString().PadLeft(2, '0')}{(EpisodeNumber > 0 ? "E" + EpisodeNumber : "")} > {FileName}";
            }
            else
            {
                return $"{SeriesTitle.CustomTitle} > S{SeasonNumber.ToString().PadLeft(2, '0')}E{EpisodeNumber.ToString().PadLeft(2, '0')} > \"{EpisodeName}\"";
            }
        }

        public void SetCustomTitle(string? customTitle, bool isNew)
        {
            SeriesTitle.CustomTitle = customTitle ?? SeriesTitle.OriginalTitle;
            SeriesTitle.IsNew = isNew;

            if (isNew)
                SettingsManager.CustomSeriesTitleManager.AddCustomSeriesTitle(SeriesTitle.OriginalTitle, SeriesTitle.CustomTitle, isNew);
        }
    }
}
