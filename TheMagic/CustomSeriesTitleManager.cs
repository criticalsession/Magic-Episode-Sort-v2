namespace TheMagic; 

public class CustomSeriesTitleManager
{
    internal List<SeriesTitle> CustomSeriesTitles => MESDBHandler.LoadCustomTitles();

    public void AddCustomSeriesTitle(string originalTitle, string customTitle, bool isNew)
    {
        MESDBHandler.SaveCustomTitle(new SeriesTitle()
        {
            CustomTitle = customTitle,
            OriginalTitle = originalTitle,
            IsNew = isNew
        });

        SettingsManager.SettingsChanged = true;
    }

    public void UpdateCustomSeriesTitle(string originalTitle, string customTitle)
    {
        AddCustomSeriesTitle(originalTitle, customTitle, false);
    }

    public void RemoveCustomSeriesTitle(string originalTitle)
    {
        MESDBHandler.DeleteCustomTitle(originalTitle);
        SettingsManager.SettingsChanged = true;
    }

    public List<SeriesTitle> GetAllCustomSeriesTitles() => CustomSeriesTitles;

    public SeriesTitle? GetCustomSeriesTitle(string originalTitle) => CustomSeriesTitles.FirstOrDefault(p => string.Equals(p.OriginalTitle, originalTitle, StringComparison.OrdinalIgnoreCase));

    public List<SeriesTitle> GetNewSeriesTitles(List<VideoFile> videoFiles) => videoFiles.Where(p => p.SeriesTitle.IsNew).Select(p => p.SeriesTitle).Distinct().ToList();
}
