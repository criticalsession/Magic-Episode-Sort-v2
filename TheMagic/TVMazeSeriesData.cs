using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    public class TVMazeSeriesData
    {
        public string OriginalTitle { get; set; } = "";
        public string? SeriesTitle { get; set; }
        public int? SeriesId { get; set; }
        public TVMazeAPI.EpisodeListApiModel[]? EpisodeData { get; set; }
    }
}
