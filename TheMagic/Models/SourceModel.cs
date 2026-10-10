namespace TheMagic.Models; 

public class SourceModel
{
    public int id { get; set; }
    public string source { get; set; }

    internal void Fill(SourceDirectory dir)
    {
        id = dir.Id;
        source = dir.SourcePath;
    }
}
