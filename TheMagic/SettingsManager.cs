namespace TheMagic; 

public class SettingsManager
{
    internal static Settings settings;
    private static DirectoriesManager? sourceDirectoriesManager = null;
    private static CustomSeriesTitleManager? customSeriesTitleManager = null;
    private static bool settingsLoaded = false;
    private static bool tablesChecked = false;

    internal static string settingsFolder => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Magic Episode Sort");

    public static bool SettingsChanged { get; set; } = true;

    public static bool AskForNewSeriesNames
    {
        get
        {
            LoadSettings();
            return settings.askForNewSeriesNames;
        }
        set => settings.askForNewSeriesNames = value;
    }

    public static bool SearchSubFolders
    {
        get
        {
            LoadSettings();
            return settings.searchSubFolders;
        }
        set => settings.searchSubFolders = value;
    }

    public static bool RecursiveSearchSubFolders
    {
        get
        {
            LoadSettings();
            return settings.searchSubFolders && settings.recursiveSearchSubFolders;
        }
        set => settings.recursiveSearchSubFolders = value;
    }

    public static bool UseTVMazeAPI
    {
        get
        {
            LoadSettings();
            return settings.useTVMazeApi;
        }
        set => settings.useTVMazeApi = value;
    }

    public static string OutputDirectory
    {
        get
        {
            LoadSettings();
            return string.IsNullOrEmpty(settings.outputDirectory) ? "" : settings.outputDirectory;
        }
        set => settings.outputDirectory = value;
    }

    public static bool OpenOutputDirectoryAfterSort
    {
        get
        {
            LoadSettings();
            return settings.openOutputDirectoryAfterSort;
        }
        set => settings.openOutputDirectoryAfterSort = value;
    }

    public static bool RenameFilenames
    {
        get
        {
            LoadSettings();
            return settings.renameFilenames;
        }
        set => settings.renameFilenames = value;
    }

    public static bool DeleteParentFolder
    {
        get
        {
            LoadSettings();
            return settings.deleteParentFolder;
        }
        set => settings.deleteParentFolder = value;
    }

    public static CustomSeriesTitleManager CustomSeriesTitleManager
    {
        get
        {
            customSeriesTitleManager ??= new CustomSeriesTitleManager();
            return customSeriesTitleManager;
        }
    }

    public static DirectoriesManager DirectoriesManager
    {
        get
        {
            sourceDirectoriesManager ??= new DirectoriesManager();
            return sourceDirectoriesManager;
        }
    }

    public static bool FirstTime
    {
        get
        {
            if (!Directory.Exists(settingsFolder)) Directory.CreateDirectory(settingsFolder);
            if (!File.Exists(Path.Combine(settingsFolder, "MESDB.db")))
            {
                MESDBHandler.BuildDB();
                return true;
            }

            return false;
        }
    }

    public static void RunDBUpdates()
    {
        MESDBHandler.RunDBUpdates();
    }

    public static List<string> Extensions => [".avi", ".mkv", ".mp4"];

    public static List<string> Regexes => ["[sS][0-9]+[eE][0-9]+-*[eE]*[0-9]*", "[0-9]+[xX][0-9]+"];

    public static void LoadSettings()
    {
        if (!settingsLoaded)
        {
            settings = MESDBHandler.LoadSettings();
            settingsLoaded = true;

            if (!tablesChecked)
            {
                MESDBHandler.CheckTablesAreAllSetup();
                tablesChecked = true;
            }
        }
    }

    public static void SaveSettings(bool isNew = false)
    {
        MESDBHandler.SaveSettings(settings, isNew);
        SettingsChanged = true;
    }
}