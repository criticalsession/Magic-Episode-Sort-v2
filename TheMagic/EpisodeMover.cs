namespace TheMagic; 

public class EpisodeMover
{
    public enum MoveErrors
    {
        None = 0,
        FileDoesNotExist,
        FileAlreadyExists,
        CouldNotDeleteDirectory
    }

    public void MoveEpisodeFiles(List<VideoFile> episodes)
    {
        foreach (var episode in episodes)
        {

            if (!File.Exists(episode.SourcePath)) episode.MoveError = MoveErrors.FileDoesNotExist;
            else
            {
                if (!File.Exists(episode.TargetPath))
                {
                    File.Move(episode.SourcePath, episode.TargetPath);
                    episode.MoveError = MoveErrors.None;
                }
                else
                    episode.MoveError = MoveErrors.FileAlreadyExists;
            }
        }

        DeleteParentDirectories(episodes);
    }

    private void DeleteParentDirectories(List<VideoFile> episodes) {
        if (episodes == null || !SettingsManager.DeleteParentFolder) {
            return;
        }

        foreach (var episode in episodes.Where(p => p.MoveError == MoveErrors.None)) {
            if (!string.IsNullOrEmpty(episode.ParentDirectory) &&
                !string.IsNullOrEmpty(episode.ParentDirectoryName) &&
                string.Equals(episode.ParentDirectoryName, Path.GetFileNameWithoutExtension(episode.SourcePath), StringComparison.OrdinalIgnoreCase)) {
                try {
                    Directory.Delete(episode.ParentDirectory, true);
                } catch {
                    episode.MoveError = MoveErrors.CouldNotDeleteDirectory;
                }
            }
        }
    }
}
