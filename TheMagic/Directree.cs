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
        private const string ErrorResponse = "error";
        private static readonly StringComparer IgnoreCaseComparer = StringComparer.OrdinalIgnoreCase;

        public HashSet<string> Directories { get; } = new();
        public HashSet<string> SkipDirectories { get; private set; } = new(IgnoreCaseComparer);
        public List<VideoFile> VideoFiles { get; private set; } = new();

        public bool SearchComplete { get; private set; }

        public event EventHandler? DirectorySearched;
        public event EventHandler? FoundVideoFile;
        public event EventHandler? FillingCustomSeriesTitles;

        public List<string> DistinctSeriesTitles
            => VideoFiles.Select(p => p.SeriesTitle.CustomTitle).Distinct().ToList();

        public void InitializeAndPopulateVideoData(List<SourceDirectory> sourceDirectories, bool searchSubDirectories, bool recursive)
        {
            SkipDirectories = new HashSet<string>(SettingsManager.DirectoriesManager.SkipDirectoryPaths, IgnoreCaseComparer);
            SearchDirectories(sourceDirectories, searchSubDirectories, recursive);

            BuildVideoFiles();
            FillCustomSeriesTitles();

            VideoFiles = VideoFiles.OrderBy(p => p.SeriesTitle.CustomTitle).ThenBy(p => p.SeasonNumber).ToList();

            SearchComplete = true;
        }

        private bool ShouldSkipDirectory(string directory)
            => Directories.Contains(directory) || SkipDirectories.Contains(directory) || IsOutputDirectory(directory);

        private static bool IsOutputDirectory(string directory)
            => IgnoreCaseComparer.Equals(directory, SettingsManager.OutputDirectory);

        private void SearchDirectories(List<SourceDirectory> sourceDirectories, bool searchSubDirectories, bool recursive)
        {
            foreach (var sourceDirectory in sourceDirectories)
            {
                var directory = sourceDirectory.SourcePath;

                if (ShouldSkipDirectory(directory))
                    continue;

                Directories.Add(directory);
                DirectorySearched?.Invoke(this, EventArgs.Empty);

                if (searchSubDirectories)
                    AddSubDirectories(directory, recursive);
            }
        }

        private void AddSubDirectories(string path, bool recursive)
        {
            foreach (var subDirectory in Directory.GetDirectories(path))
            {
                if (ShouldSkipDirectory(subDirectory))
                    continue;

                Directories.Add(subDirectory);
                DirectorySearched?.Invoke(this, EventArgs.Empty);

                if (recursive)
                    AddSubDirectories(subDirectory, recursive);
            }
        }

        private void BuildVideoFiles()
        {
            foreach (var filePath in GetFilePathsInAllDirectories())
            {
                var videoFile = new VideoFile(filePath);
                if (videoFile.IsValidVideoFile)
                {
                    VideoFiles.Add(videoFile);
                    FoundVideoFile?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private IEnumerable<string> GetFilePathsInAllDirectories()
            => Directories.SelectMany(directory => Directory.GetFiles(directory));


        private void FillCustomSeriesTitles()
        {
            FillingCustomSeriesTitles?.Invoke(this, EventArgs.Empty);
            var tvMazeApi = SettingsManager.UseTVMazeAPI ? new TVMazeAPI() : null;

            foreach (var videoFile in VideoFiles)
            {
                var originalTitle = videoFile.SeriesTitle.OriginalTitle;
                var storedTitle = SettingsManager.CustomSeriesTitleManager.GetCustomSeriesTitle(originalTitle);

                if (storedTitle != null)
                {
                    videoFile.SetCustomTitle(storedTitle.CustomTitle, false);
                }
                else
                {
                    var titleToUse = originalTitle;

                    if (tvMazeApi != null)
                    {
                        var apiResponse = GetSeriesTitleFromTVMaze(tvMazeApi, originalTitle);

                        if (ReceivedValidResponse(apiResponse))
                        {
                            titleToUse = apiResponse;
                        }
                    }

                    videoFile.SetCustomTitle(titleToUse, true);
                }
            }
        }

        private static string? GetSeriesTitleFromTVMaze(TVMazeAPI tvMazeApi, string originalTitle)
        {
            string? apiResponse = null;
            int errorCount = 0;

            while (errorCount <= 2)
            {
                apiResponse = tvMazeApi.GetSeriesTitle(originalTitle);
                if (apiResponse != ErrorResponse)
                    break;

                errorCount++;
                Thread.Sleep(200);
            }

            return apiResponse;
        }

        private static bool ReceivedValidResponse(string? apiResponse)
            => !string.IsNullOrEmpty(apiResponse) && apiResponse != ErrorResponse;
    }
}
