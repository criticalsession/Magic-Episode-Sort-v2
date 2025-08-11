namespace TheMagic; 

public class DirectoriesManager
{
    public List<SourceDirectory> SourceDirectories => CheckDirectoriesExist(MESDBHandler.LoadSourceDirectories());

    public List<SkipDirectory> SkipDirectories => MESDBHandler.LoadSkipDirectories();

    public void RemoveSourceDirectory(SourceDirectory dir) => RemoveSourceDirectory(dir.SourcePath);

    public List<string> SourceDirectoryPaths => SourceDirectories.Select(p => p.SourcePath).ToList();

    public List<string> SkipDirectoryPaths => SkipDirectories.Select(p => p.Directory).ToList();

    private List<SourceDirectory> CheckDirectoriesExist(List<SourceDirectory> directories)
    {
        var checkedDirectories = new List<SourceDirectory>();
        foreach (SourceDirectory dir in directories)
        {
            if (!Directory.Exists(dir.SourcePath))
            {
                RemoveSourceDirectory(dir);
            }
            else
            {
                checkedDirectories.Add(dir);
            }
        }

        return checkedDirectories;
    }

    public bool AddSourceDirectory(string dir)
    {
        var directories = MESDBHandler.LoadSourceDirectories();
        if (!directories.Any(p => p.SourcePath == dir))
        {
            MESDBHandler.AddSourceDirectory(dir);
            SettingsManager.SettingsChanged = true;

            return true;
        }
        
        return false;
    }

    public bool RemoveSourceDirectory(string? dir)
    {
        if (!string.IsNullOrEmpty(dir))
        {
            var directories = MESDBHandler.LoadSourceDirectories();
            if (directories.Any(p => p.SourcePath == dir))
            {
                MESDBHandler.DeleteSourceDirectory(dir);
                SettingsManager.SettingsChanged = true;

                return true;
            }
        }

        return false;
    }

    public bool AddSkipDirectory(string dir)
    {
        var directories = MESDBHandler.LoadSkipDirectories();
        if (!directories.Any(p => p.Directory == dir))
        {
            MESDBHandler.AddSkipDirectory(dir);
            SettingsManager.SettingsChanged = true;

            return true;
        }
        
        return false;
    }

    public bool RemoveSkipDirectory(string? dir)
    {
        if (!string.IsNullOrEmpty(dir))
        {
            var directories = MESDBHandler.LoadSkipDirectories();
            if (directories.Any(p => p.Directory == dir))
            {
                MESDBHandler.DeleteSkipDirectory(dir);
                SettingsManager.SettingsChanged= true;

                return true;
            }
        }

        return false;
    }
}
