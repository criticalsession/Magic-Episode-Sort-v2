using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    public class SettingsManager
    {
        internal static Settings settings;

        public static bool AskForNewSeriesNames
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return settings.askForNewSeriesNames;
            }
            set => settings.askForNewSeriesNames = value;
        }

        public static bool SearchSubFolders
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return settings.searchSubFolders;
            }
            set => settings.searchSubFolders = value;
        }

        public static bool RecursiveSearchSubFolders
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return settings.recursiveSearchSubFolders;
            }
            set => settings.recursiveSearchSubFolders = value;
        }

        public static string TargetDirectory
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return String.IsNullOrEmpty(settings.targetDirectory) ? "" : settings.targetDirectory;
            }
            set => settings.targetDirectory = value;
        }

        private static SourceDirectoriesManager? sourceDirectoriesManager;
        public static List<string> SourceDirectories
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return sourceDirectoriesManager == null ? new List<string>() 
                    : sourceDirectoriesManager.SourceDirectories;
            }
        }

        private static List<string>? extensions = null;
        public static List<string> Extensions
        {
            get
            {
                if (extensions == null)
                {
                    extensions = new List<string>();
                    extensions.Add(".avi");
                    extensions.Add(".mkv");
                    extensions.Add(".mp4");
                }

                return extensions;
            }
        }

        private static List<string>? regexes = null;
        public static List<string> Regexes
        {
            get
            {
                if (regexes == null)
                {
                    regexes = new List<string>();
                    regexes.Add("[sS][0-9]+");
                    regexes.Add("[sS][0-9]+[eE][0-9]+-*[eE]*[0-9]*");
                    regexes.Add("[0-9]+[xX][0-9]+");
                }

                return regexes;
            }
        }

        private static bool settingsLoaded = false;
        private static string appDirectory { get => System.IO.Directory.GetCurrentDirectory(); }
        private static string settingsPath { get => Path.Combine(appDirectory, "mes.settings"); }
        public static bool SettingsFileExists { get => File.Exists(settingsPath); }
        public static bool SettingsChanged { get; set; } = true;

        public static void LoadSettingsIfNotLoaded()
        {
            if (!settingsLoaded)
            {
                settings = new Settings();
                sourceDirectoriesManager = new SourceDirectoriesManager();

                if (!SettingsFileExists)
                {
                    settings.askForNewSeriesNames = true;
                    settings.searchSubFolders = true;
                    settings.recursiveSearchSubFolders = true;
                    settings.targetDirectory = Path.Combine(appDirectory, "Sorted Episodes");

                    sourceDirectoriesManager.AddDirectory(appDirectory);

                    settingsLoaded = true;

                    SaveSettings();
                }
                else
                {
                    // TODO: refactor to read from json file

                    //string[] settingsRead = File.ReadAllLines(settingsPath);

                    //foreach (string setting in settingsRead)
                    //{
                    //    if (setting.StartsWith("askfornewseriesnames=")) askForNewSeriesNames = ReadSettingsValue(setting, "askfornewseriesnames") == "yes" ? true : false;
                    //    if (setting.StartsWith("searchsubfolders=")) searchSubFolders = ReadSettingsValue(setting, "searchsubfolders") == "yes" ? true : false;
                    //    if (setting.StartsWith("recursivesearchsubfolders=")) recursiveSearchSubFolders = ReadSettingsValue(setting, "recursivesearchsubfolders") == "yes" ? true : false;
                    //    if (setting.StartsWith("targetdirectory=")) targetDirectory = ReadSettingsValue(setting, "targetdirectory");

                    //    if (setting.StartsWith("sources="))
                    //    {
                    //        string sources = ReadSettingsValue(setting, "sources");
                    //        if (!String.IsNullOrEmpty(sources))
                    //        {
                    //            if (!sources.Contains(";")) sourceDirectoriesManager.AddDirectory(sources);
                    //            else
                    //            {
                    //                foreach (string source in sources.Split(';'))
                    //                {
                    //                    sourceDirectoriesManager.AddDirectory(source);
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    settingsLoaded = true;
                }
            }
        }

        public static void AddSourceDirectory(string dir)
        {
            LoadSettingsIfNotLoaded();
            if (sourceDirectoriesManager != null && sourceDirectoriesManager.AddDirectory(dir))
                SaveSettings();
        }

        public static void RemoveSourceDirectory(string dir)
        {
            LoadSettingsIfNotLoaded();
            if (sourceDirectoriesManager != null && sourceDirectoriesManager.RemoveDirectory(dir))
                SaveSettings();
        }

        public static void SaveSettings()
        {
            LoadSettingsIfNotLoaded();

            // TODO: refactor to read from json file

            //StringBuilder settingsFile = new StringBuilder();
            //settingsFile.AppendLine("askfornewseriesnames=" + (askForNewSeriesNames ? "yes" : "no"));
            //settingsFile.AppendLine("searchsubfolders=" + (searchSubFolders ? "yes" : "no"));
            //settingsFile.AppendLine("recursivesearchsubfolders=" + (recursiveSearchSubFolders? "yes" : "no"));
            //settingsFile.AppendLine("targetdirectory=" + targetDirectory);
            //settingsFile.AppendLine("sources=" + sourceDirectoriesManager.GetCSVDirectories());

            //File.WriteAllText(settingsPath, settingsFile.ToString());

            SettingsChanged = true;
        }

    }
}
