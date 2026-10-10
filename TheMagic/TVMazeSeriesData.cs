namespace TheMagic; 

public class TVMazeSeriesData
{
    public string OriginalTitle { get; set; } = "";
    public string? SeriesTitle { get; set; }
    public int? SeriesId { get; set; }
    public TVMazeAPI.EpisodeListApiModel[]? EpisodeData { get; set; }
}
