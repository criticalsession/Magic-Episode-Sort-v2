using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheMagic
{
    public class Targetree
    {
        public bool BuildDirectoryTreeInTarget(List<VideoFile> episodes, string outputDirectory)
        {
            if (String.IsNullOrEmpty(outputDirectory)) return false;
            else
            {
                if (!Directory.Exists(outputDirectory))
                    Directory.CreateDirectory(outputDirectory);

                List<string> donePaths = new List<string>();

                if (episodes.Count == 0) return false;
                else
                {
                    foreach (VideoFile episode in episodes)
                    {
                        string seriesDirectoryPath = GetSeriesDirectoryPath(outputDirectory, episode);
                        if (!donePaths.Contains(seriesDirectoryPath) && !Directory.Exists(seriesDirectoryPath))
                        {
                            Directory.CreateDirectory(seriesDirectoryPath);
                            donePaths.Add(seriesDirectoryPath);
                        }

                        string fullDirectoryPath = GetFullDirectoryPath(outputDirectory, episode);
                        if (!donePaths.Contains(fullDirectoryPath) && !Directory.Exists(fullDirectoryPath))
                        {
                            Directory.CreateDirectory(fullDirectoryPath);
                            donePaths.Add(fullDirectoryPath);
                        }

                        episode.TargetPath = Path.Combine(fullDirectoryPath, episode.NewFileName);
                    }

                    return true;
                }
            }
        }

        private string GetSeriesDirectoryPath(string outputDirectory, VideoFile episode)
        {
            return Path.Combine(outputDirectory, Utils.Sanitize(episode.SeriesTitle.CustomTitle));
        }

        private string GetFullDirectoryPath(string outputDirectory, VideoFile episode)
        {
            return Path.Combine(GetSeriesDirectoryPath(outputDirectory, episode), Utils.Sanitize(episode.SeasonDirName));
        }
    }
}
