using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
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
            foreach (VideoFile episode in episodes)
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

        private void DeleteParentDirectories(List<VideoFile> episodes)
        {
            if (episodes != null && SettingsManager.DeleteParentFolder)
            {
                foreach (VideoFile episode in episodes.Where(p => p.MoveError == MoveErrors.None))
                {
                    if (!String.IsNullOrEmpty(episode.ParentDirectory) && 
                        !String.IsNullOrEmpty(episode.ParentDirectoryName) && 
                        episode.ParentDirectoryName == Path.GetFileNameWithoutExtension(episode.SourcePath))
                    {
                        try
                        {
                            Directory.Delete(episode.ParentDirectory, true);
                        }
                        catch
                        {
                            episode.MoveError = MoveErrors.CouldNotDeleteDirectory;
                        }
                    }
                }
            }
        }
    }
}
