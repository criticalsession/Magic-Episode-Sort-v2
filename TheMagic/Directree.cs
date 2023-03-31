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

        public void Build(List<SourceDirectory> sourceDirectories, bool searchSubDirectories, bool recursive)
        {
            foreach (SourceDirectory sourceDirectory in sourceDirectories)
            {
                string directory = sourceDirectory.SourcePath;
                if (!SameAsOutputDirectory(directory) && !AlreadyChecked(directory))
                {
                    Directories.Add(directory);
                    DirectorySearched?.Invoke(this, EventArgs.Empty);

                    if (searchSubDirectories) AddDirsInPath(directory, recursive);
                }
            }

            BuildVideoFiles();
            FillCustomSeriesTitles();

            VideoFiles = VideoFiles.OrderBy(p => p.SeriesTitle.CustomTitle).ThenBy(p => p.SeasonNumber).ToList();

            SearchComplete = true;
        }

        private bool AlreadyChecked(string directory)
        {
            return Directories.Contains(directory);
        }

        private static bool SameAsOutputDirectory(string directory)
        {
            return directory.ToLower() == SettingsManager.OutputDirectory.ToLower();
        }

        private void AddDirsInPath(string path, bool recursive)
        {
            foreach (string subDirectory in Directory.GetDirectories(path))
            {
                if (!SameAsOutputDirectory(subDirectory))
                {
                    if (!AlreadyChecked(subDirectory))
                    {
                        Directories.Add(subDirectory);
                        DirectorySearched?.Invoke(this, EventArgs.Empty);
                    }

                    if (recursive) AddDirsInPath(subDirectory, recursive);
                }
            }
        }

        private void BuildVideoFiles()
        {
            foreach (string filePath in GetFilePathsInAllDirectories())
            {
                VideoFile videoFile = new VideoFile(filePath);
                if (videoFile.IsValidVideoFile)
                {
                    VideoFiles.Add(videoFile);
                    FoundVideoFile?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private IEnumerable<string> GetFilePathsInAllDirectories()
        {
            List<string> filePaths = new List<string>();

            foreach (string directory in Directories)
                filePaths.AddRange(Directory.GetFiles(directory));

            return filePaths;
        }

        private void FillCustomSeriesTitles()
        {
            FillingCustomSeriesTitles?.Invoke(this, EventArgs.Empty);
            TVMazeAPI? tvMazeApi = null;

            foreach (VideoFile videoFile in VideoFiles)
            {
                string originalTitle = videoFile.SeriesTitle.OriginalTitle;

                SeriesTitle? storedTitle = SettingsManager.CustomSeriesTitleManager.GetCustomSeriesTitle(originalTitle);

                if (storedTitle != null)
                {
                    videoFile.SetCustomTitle(storedTitle.CustomTitle, false);
                }
                else
                {
                    if (SettingsManager.UseTVMazeAPI)
                    {
                        if (tvMazeApi == null) tvMazeApi = new TVMazeAPI();
                        string? apiResponse = GetSeriesTitleFromTVMaze(tvMazeApi, originalTitle);

                        if (ReceivedValidResponse(apiResponse)) videoFile.SetCustomTitle(apiResponse, true);
                        else videoFile.SetCustomTitle(originalTitle, true);
                    }
                    else
                    {
                        videoFile.SetCustomTitle(originalTitle, true);
                    }
                }
            }
        }

        private static string? GetSeriesTitleFromTVMaze(TVMazeAPI tvMazeApi, string originalTitle)
        {
            string? apiResponse = null;
            int errorCount = 0;

            do
            {
                if (errorCount > 2) break;

                apiResponse = tvMazeApi.GetSeriesTitle(originalTitle);
                if (apiResponse == "error") // wait a bit then try again
                {
                    errorCount++;
                    Thread.Sleep(200);
                }
                else break;
            } while (true);

            return apiResponse;
        }

        private static bool ReceivedValidResponse(string? apiResponse)
        {
            return !String.IsNullOrEmpty(apiResponse) && apiResponse != "error";
        }
    }
}
