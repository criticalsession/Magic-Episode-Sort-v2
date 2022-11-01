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
    public class Settings
    {
        private static bool askForNewSeriesNames;
        public static bool AskForNewSeriesNames
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return askForNewSeriesNames;
            }
            set => askForNewSeriesNames = value;
        }

        private static bool searchSubFolders;
        public static bool SearchSubFolders
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return searchSubFolders;
            }
            set => searchSubFolders = value;
        }

        private static bool recursiveSearchSubFolders;
        public static bool RecursiveSearchSubFolders
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return recursiveSearchSubFolders;
            }
            set => recursiveSearchSubFolders = value;
        }

        private static string targetDirectory = "";
        public static string TargetDirectory
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return String.IsNullOrEmpty(targetDirectory) ? "" : targetDirectory;
            }
            set => targetDirectory = value;
        }

        private static SourceDirectoriesManager sourceDirectoriesManager;
        public static List<string> SourceDirectories
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return sourceDirectoriesManager.SourceDirectories;
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
                sourceDirectoriesManager = new SourceDirectoriesManager();

                if (!SettingsFileExists)
                {
                    askForNewSeriesNames = true;
                    searchSubFolders = true;
                    recursiveSearchSubFolders = true;
                    targetDirectory = Path.Combine(appDirectory, "Sorted_Episodes");

                    sourceDirectoriesManager.AddDirectory(appDirectory);

                    settingsLoaded = true;

                    SaveSettings();
                }
                else
                {
                    string[] settingsRead = File.ReadAllLines(settingsPath);

                    foreach (string setting in settingsRead)
                    {
                        if (setting.StartsWith("askfornewseriesnames=")) askForNewSeriesNames = ReadSettingsValue(setting, "askfornewseriesnames") == "yes" ? true : false;
                        if (setting.StartsWith("searchsubfolders=")) searchSubFolders = ReadSettingsValue(setting, "searchsubfolders") == "yes" ? true : false;
                        if (setting.StartsWith("recursivesearchsubfolders=")) recursiveSearchSubFolders = ReadSettingsValue(setting, "recursivesearchsubfolders") == "yes" ? true : false;
                        if (setting.StartsWith("targetdirectory=")) targetDirectory = ReadSettingsValue(setting, "targetdirectory");

                        if (setting.StartsWith("sources="))
                        {
                            string sources = ReadSettingsValue(setting, "sources");
                            if (!String.IsNullOrEmpty(sources))
                            {
                                if (!sources.Contains(";")) sourceDirectoriesManager.AddDirectory(sources);
                                else
                                {
                                    foreach (string source in sources.Split(';'))
                                    {
                                        sourceDirectoriesManager.AddDirectory(source);
                                    }
                                }
                            }
                        }
                    }

                    settingsLoaded = true;
                }
            }
        }

        private static string ReadSettingsValue(string settingsLine, string settingsName)
        {
            return settingsLine.Replace(settingsName.Contains("=") ? settingsName : settingsName + "=", "");
        }

        public static void AddSourceDirectory(string dir)
        {
            LoadSettingsIfNotLoaded();
            if (sourceDirectoriesManager.AddDirectory(dir))
                SaveSettings();
        }

        public static void RemoveDirectory(string dir)
        {
            LoadSettingsIfNotLoaded();
            if (sourceDirectoriesManager.RemoveDirectory(dir))
                SaveSettings();
        }

        public static void SaveSettings()
        {
            LoadSettingsIfNotLoaded();

            StringBuilder settingsFile = new StringBuilder();
            settingsFile.AppendLine("askfornewseriesnames=" + (askForNewSeriesNames ? "yes" : "no"));
            settingsFile.AppendLine("searchsubfolders=" + (searchSubFolders ? "yes" : "no"));
            settingsFile.AppendLine("recursivesearchsubfolders=" + (recursiveSearchSubFolders? "yes" : "no"));
            settingsFile.AppendLine("targetdirectory=" + targetDirectory);
            settingsFile.AppendLine("sources=" + sourceDirectoriesManager.GetCSVDirectories());

            File.WriteAllText(settingsPath, settingsFile.ToString());

            SettingsChanged = true;
        }

    }
}
