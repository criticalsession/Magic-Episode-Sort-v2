using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    public class Directree
    {
        public List<string> Directories { get; set; } = new List<string>();
        public List<VideoFile> VideoFiles { get; set; } = new List<VideoFile>();

        public bool SearchComplete = false;

        public event EventHandler? DirectorySearched;
        public event EventHandler? FoundVideoFile;
        public event EventHandler? FillingCustomSeriesTitles;

        public List<string> DistingSeriesTitles
        {
            get
            {
                return VideoFiles.Select(p => p.SeriesTitle.CustomTitle).Distinct().ToList();
            }
        }

        public void Build(List<string> sourceDirectories, bool searchSubDirectories, bool recursive)
        {
            foreach (string directory in sourceDirectories)
            {
                if (directory.ToLower() != SettingsManager.OutputDirectory.ToLower() && !Directories.Contains(directory))
                {
                    Directories.Add(directory);
                    DirectorySearched?.Invoke(this, EventArgs.Empty);

                    if (searchSubDirectories)
                    {
                        AddDirsInPath(directory, recursive);
                    }
                }
            }

            BuildVideoFiles();
            FillCustomSeriesTitles();

            SearchComplete = true;
        }

        private void AddDirsInPath(string path, bool recursive)
        {
            foreach (string subDirectory in Directory.GetDirectories(path).ToList())
            {
                if (subDirectory.ToLower() != SettingsManager.OutputDirectory.ToLower())
                {
                    if (!Directories.Contains(subDirectory))
                    {
                        Directories.Add(subDirectory);
                        DirectorySearched?.Invoke(this, EventArgs.Empty);
                    }

                    if (recursive)
                    {
                        AddDirsInPath(subDirectory, recursive);
                    }
                }
            }
        }

        private void BuildVideoFiles()
        {
            foreach (string directory in Directories)
            {
                foreach (string filePath in Directory.GetFiles(directory))
                {
                    VideoFile videoFile = new VideoFile(filePath);
                    if (videoFile.IsVideoFile)
                    {
                        VideoFiles.Add(videoFile);
                        FoundVideoFile?.Invoke(this, EventArgs.Empty);
                    }
                }
            }

            VideoFiles.RemoveAll(p => String.IsNullOrEmpty(p.SeriesTitle.OriginalTitle));
            VideoFiles = VideoFiles.OrderBy(p => p.SeriesTitle.CustomTitle).ThenBy(p => p.SeasonNumber).ToList();
        }        
        
        private void FillCustomSeriesTitles()
        {
            FillingCustomSeriesTitles?.Invoke(this, EventArgs.Empty);

            foreach (VideoFile videoFile in VideoFiles)
            {
                string title = videoFile.SeriesTitle.OriginalTitle;

                if (SettingsManager.CustomSeriesTitleManager.HasCustomSeriesTitle(title))
                {
                    SeriesTitle? custom = SettingsManager.CustomSeriesTitleManager.GetCustomSeriesTitle(title);
                    videoFile.SeriesTitle.CustomTitle = custom != null ? custom.CustomTitle : title;
                    videoFile.SeriesTitle.IsNew = false;
                }
                else
                {
                    SettingsManager.CustomSeriesTitleManager.AddCustomSeriesTitle(title, title);
                    videoFile.SeriesTitle.CustomTitle = title;
                    videoFile.SeriesTitle.IsNew = true;
                }
            }
        }
    }
}
