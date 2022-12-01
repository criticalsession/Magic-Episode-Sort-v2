using Newtonsoft.Json;
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
                return settings.searchSubFolders && settings.recursiveSearchSubFolders;
            }
            set => settings.recursiveSearchSubFolders = value;
        }

        public static bool UseTVMazeAPI
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return settings.useTVMazeApi;
            }
            set => settings.useTVMazeApi = value;
        }

        public static string OutputDirectory
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return String.IsNullOrEmpty(settings.outputDirectory) ? "" : settings.outputDirectory;
            }
            set => settings.outputDirectory = value;
        }

        public static bool OpenOutputDirectoryAfterSort
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return settings.openOutputDirectoryAfterSort;
            }
            set => settings.openOutputDirectoryAfterSort = value;
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

        private static CustomSeriesTitleManager customSeriesTitleManager = new CustomSeriesTitleManager();
        public static CustomSeriesTitleManager CustomSeriesTitleManager
        {
            get
            {
                LoadSettingsIfNotLoaded();
                return customSeriesTitleManager;
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
        private static string settingsFolder { get => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Magic Episode Sort"); }
        private static string settingsPath { get => Path.Combine(settingsFolder, "mes.settings"); }
        public static bool SettingsFileExists { get => File.Exists(settingsPath); }
        public static bool SettingsChanged { get; set; } = true;

        public static void LoadSettingsIfNotLoaded()
        {
            if (!settingsLoaded)
            {
                if (!Directory.Exists(settingsFolder)) Directory.CreateDirectory(settingsFolder);

                settings = new Settings();
                sourceDirectoriesManager = new SourceDirectoriesManager();

                if (!SettingsFileExists)
                {
                    settings.askForNewSeriesNames = true;
                    settings.searchSubFolders = true;
                    settings.recursiveSearchSubFolders = true;
                    settings.outputDirectory = "";
                    settings.openOutputDirectoryAfterSort = false;

                    settingsLoaded = true;

                    SaveSettings();
                }
                else
                {
                    Settings? deserializedSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsPath));
                    if (deserializedSettings != null)
                    {
                        settings = deserializedSettings;
                        settingsLoaded = true;

                        bool issuesFound = false;
                        if (!String.IsNullOrEmpty(OutputDirectory) && !Directory.Exists(OutputDirectory))
                        {
                            OutputDirectory = "";
                            issuesFound = true;
                        }

                        foreach (string sourceDirectory in SourceDirectories)
                        {
                            if (!String.IsNullOrEmpty(sourceDirectory) && !Directory.Exists(sourceDirectory))
                            {
                                RemoveSourceDirectory(sourceDirectory, false);
                                issuesFound = true;
                            }
                        }

                        if (issuesFound)
                            SaveSettings();
                    }
                    else
                    {
                        File.Delete(settingsPath);
                        LoadSettingsIfNotLoaded();
                    }
                }
            }
        }

        public static void AddSourceDirectory(string dir)
        {
            LoadSettingsIfNotLoaded();
            if (sourceDirectoriesManager != null && sourceDirectoriesManager.AddDirectory(dir))
                SaveSettings();
        }

        public static void RemoveSourceDirectory(string dir, bool saveAfter = true)
        {
            LoadSettingsIfNotLoaded();
            if (sourceDirectoriesManager != null && sourceDirectoriesManager.RemoveDirectory(dir) && saveAfter)
                SaveSettings();
        }

        public static void SaveSettings()
        {
            LoadSettingsIfNotLoaded();

            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));

            SettingsChanged = true;
        }

    }
}
