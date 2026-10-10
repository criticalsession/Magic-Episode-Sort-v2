namespace TheMagic; 

public class SeriesTitle
{
    public string OriginalTitle { get; set; }
    public string CustomTitle { get; set; }
    public bool IsNew { get; set; }
    public int Id { get; set; }

    public bool TitleChanged => OriginalTitle.ToLower() != CustomTitle.ToLower();

    public SeriesTitle()
    {
        OriginalTitle = string.Empty;
        CustomTitle = string.Empty;
        IsNew = false;
        Id = 0;
    }

    public override string ToString() => !TitleChanged ? CustomTitle : $"{CustomTitle} (⇐ {OriginalTitle})";
}
