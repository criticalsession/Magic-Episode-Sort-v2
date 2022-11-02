using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    public class EpisodeMover
    {
        public void MoveEpisodeFiles(List<VideoFile> episodes)
        {
            foreach (VideoFile episode in episodes)
            {
                File.Move(episode.SourcePath, episode.TargetPath);
            }
        }
    }
}
