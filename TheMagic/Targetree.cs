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
        public void BuildDirectoryTreeInTarget(List<VideoFile> episodes, string outputDirectory)
        {
            if (!String.IsNullOrEmpty(outputDirectory))
            {
                if (!Directory.Exists(outputDirectory))
                    Directory.CreateDirectory(outputDirectory);

                List<string> donePaths = new List<string>();

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
