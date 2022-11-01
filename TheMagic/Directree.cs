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

        public void Build(List<string> sourceDirectories, bool searchSubDirectories, bool recursive)
        {
            foreach (string directory in sourceDirectories)
            {
                if (directory.ToLower() != Settings.TargetDirectory.ToLower() && !Directories.Contains(directory))
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

            SearchComplete = true;
        }

        private void AddDirsInPath(string path, bool recursive)
        {
            foreach (string subDirectory in Directory.GetDirectories(path).ToList())
            {
                if (subDirectory.ToLower() != Settings.TargetDirectory.ToLower())
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

            VideoFiles.RemoveAll(p => String.IsNullOrEmpty(p.OriginalSeriesName));
            VideoFiles = VideoFiles.OrderBy(p => p.CustomSeriesName).ThenBy(p => p.SeasonNumber).ToList();
        }
    }
}
