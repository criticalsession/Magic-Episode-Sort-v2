using System;
using System.Collections.Generic;
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
            FileAlreadyExists
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
        }
    }
}
