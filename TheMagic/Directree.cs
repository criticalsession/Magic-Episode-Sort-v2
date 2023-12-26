using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public System.Collections.Concurrent.ConcurrentBag<string> cDirectories { get; } = new();
        public HashSet<string> Directories { get; } = new();
        public HashSet<string> SkipDirectories { get; private set; } = new(IgnoreCaseComparer);
        public System.Collections.Concurrent.ConcurrentBag<VideoFile> cVideoFiles { get; private set; } = new();
        public List<VideoFile> VideoFiles { get; private set; } = new();

        public bool SearchComplete { get; private set; }

        public event EventHandler? DirectorySearched;
        public event EventHandler? FoundVideoFile;
        public event EventHandler? FillingCustomSeriesTitles;
        public event EventHandler? UpdateStatus;

        public List<string> DistinctSeriesTitles
            => VideoFiles.Count > 0 ? VideoFiles.Select(p => p.SeriesTitle.CustomTitle).Distinct().ToList() : cVideoFiles.Select(p => p.SeriesTitle.CustomTitle).Distinct().ToList();

        public int TotalDirectories = 0;
        public int TotalVideoFiles = 0;

        public void InitializeAndPopulateVideoData(List<SourceDirectory> sourceDirectories, bool searchSubDirectories, bool recursive)
        {
            SkipDirectories = new HashSet<string>(SettingsManager.DirectoriesManager.SkipDirectoryPaths, IgnoreCaseComparer);
            SearchDirectories(sourceDirectories, searchSubDirectories, recursive);

            foreach (var d in cDirectories)
                Directories.Add(d);

            BuildVideoFiles();
            FillCustomSeriesTitles();

            foreach (var v in cVideoFiles)
                VideoFiles.Add(v);

            VideoFiles = VideoFiles.OrderBy(p => p.SeriesTitle.CustomTitle).ThenBy(p => p.SeasonNumber).ThenBy(p => p.EpisodeNumber).ToList();

            SearchComplete = true;
        }

        private bool ShouldSkipDirectory(string directory)
            => Directories.Contains(directory) || SkipDirectories.Contains(directory) || IsOutputDirectory(directory);

        private static bool IsOutputDirectory(string directory)
            => IgnoreCaseComparer.Equals(directory, SettingsManager.OutputDirectory);

        private void SearchDirectories(List<SourceDirectory> sourceDirectories, bool searchSubDirectories, bool recursive)
        {
            Parallel.ForEach(sourceDirectories, sourceDirectory =>
            {
                var directory = sourceDirectory.SourcePath;

                if (!ShouldSkipDirectory(directory))
                {
                    cDirectories.Add(directory);
                    Interlocked.Add(ref TotalDirectories, 1);
                    DirectorySearched?.Invoke(this, EventArgs.Empty);

                    if (searchSubDirectories)
                        AddSubDirectories(directory, recursive);
                }
            });
        }

        private void AddSubDirectories(string path, bool recursive)
        {
            Parallel.ForEach(Directory.GetDirectories(path), subDirectory =>
            {
                if (!ShouldSkipDirectory(subDirectory))
                {
                    cDirectories.Add(subDirectory);
                    Interlocked.Add(ref TotalDirectories, 1);
                    DirectorySearched?.Invoke(this, EventArgs.Empty);

                    if (recursive)
                        AddSubDirectories(subDirectory, recursive);
                }
            });
        }

        private void BuildVideoFiles()
        {
            Parallel.ForEach(GetFilePathsInAllDirectories(), filePath =>
            {
                var videoFile = new VideoFile(filePath);
                if (videoFile.IsValidVideoFile)
                {
                    cVideoFiles.Add(videoFile);
                    Interlocked.Add(ref TotalVideoFiles, 1);

                    FoundVideoFile?.Invoke(this, EventArgs.Empty);
                }
            });
        }

        private IEnumerable<string> GetFilePathsInAllDirectories()
            => Directories.SelectMany(directory => Directory.GetFiles(directory));


        private void FillCustomSeriesTitles()
        {
            FillingCustomSeriesTitles?.Invoke(this, EventArgs.Empty);
            var tvMazeApi = SettingsManager.UseTVMazeAPI ? new TVMazeAPI() : null;

            List<TVMazeSeriesData> seriesData = new List<TVMazeSeriesData>();
            foreach (var videoFile in cVideoFiles)
            {
                UpdateStatus?.Invoke(this, EventArgs.Empty);
                string titleToSearch = SettingsManager.CustomSeriesTitleManager.GetCustomSeriesTitle(videoFile.SeriesTitle.OriginalTitle)?.CustomTitle ?? videoFile.SeriesTitle.OriginalTitle;

                TVMazeSeriesData? data = seriesData.FirstOrDefault(p => p.OriginalTitle == titleToSearch);
                if (data == null)
                {
                    data = new TVMazeSeriesData()
                    {
                        OriginalTitle = titleToSearch,
                        SeriesTitle = null,
                        SeriesId = null,
                        EpisodeData = null
                    };

                    if (tvMazeApi != null)
                    {
                        var apiResponse = GetTitleAndIdFromTVMazeApi(tvMazeApi, titleToSearch);
                        data.SeriesTitle = apiResponse.Item1;
                        data.SeriesId = apiResponse.Item2;

                        if (data.SeriesId.HasValue && SettingsManager.RenameFilenames) data.EpisodeData = tvMazeApi.GetEpisodeList(data.SeriesId.Value);
                    }

                    seriesData.Add(data);
                }

                SetVideoFileCustomTitle(videoFile, data.SeriesTitle);
                SetVideoFileEpisodeName(videoFile, data.EpisodeData);
            }
        }

        private static void SetVideoFileCustomTitle(VideoFile videoFile, string? tvMazeTitle)
        {
            string originalTitle = videoFile.SeriesTitle.OriginalTitle;
            SeriesTitle? storedTitle = SettingsManager.CustomSeriesTitleManager.GetCustomSeriesTitle(originalTitle);

            if (storedTitle != null) videoFile.SetCustomTitle(storedTitle.CustomTitle, false);
            else videoFile.SetCustomTitle(String.IsNullOrEmpty(tvMazeTitle) ? originalTitle : tvMazeTitle, true);
        }

        private static (string?, int?) GetTitleAndIdFromTVMazeApi(TVMazeAPI? tvMazeApi, string originalTitle)
        {
            if (tvMazeApi != null)
            {
                var apiResponse = GetSeriesDetailsFromApi(tvMazeApi, originalTitle);
                if (ReceivedValidResponse(apiResponse.Item1)) return apiResponse;
            }

            return (null, null);
        }

        private static void SetVideoFileEpisodeName(VideoFile videoFile, TVMazeAPI.EpisodeListApiModel[]? episodeData)
        {
            if (episodeData == null) return;

            TVMazeAPI.EpisodeListApiModel? thisEpisode = episodeData.FirstOrDefault(p => p.season == videoFile.SeasonNumber && p.number == videoFile.EpisodeNumber);

            if (thisEpisode == null) return;

            videoFile.EpisodeName = thisEpisode.name;
        }

        private static (string?, int?) GetSeriesDetailsFromApi(TVMazeAPI tvMazeApi, string originalTitle)
        {
            (string?, int?) apiResponse = (null, null);
            int errorCount = 0;

            while (errorCount <= 2)
            {
                apiResponse = tvMazeApi.GetSeriesTitle(originalTitle);
                if (apiResponse.Item1 != ErrorResponse)
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
