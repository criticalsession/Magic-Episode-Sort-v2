using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    public class SettingsManager
    {
        internal static Settings settings;
        private static SourceDirectoriesManager? sourceDirectoriesManager = null;
        private static CustomSeriesTitleManager? customSeriesTitleManager = null;
        private static bool? firstTime = null;
        private static bool settingsLoaded = false;
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
                return String.IsNullOrEmpty(settings.outputDirectory) ? "" : settings.outputDirectory;
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

        public static CustomSeriesTitleManager CustomSeriesTitleManager
        {
            get
            {
                if (customSeriesTitleManager == null) customSeriesTitleManager = new CustomSeriesTitleManager();
                return customSeriesTitleManager;
            }
        }

        public static SourceDirectoriesManager SourceDirectoriesManager
        {
            get
            {
                if (sourceDirectoriesManager == null) sourceDirectoriesManager = new SourceDirectoriesManager();
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

        public static List<string> Extensions
        {
            get
            {
                List<string> extensions = new List<string>();
                extensions.Add(".avi");
                extensions.Add(".mkv");
                extensions.Add(".mp4");

                return extensions;
            }
        }

        public static List<string> Regexes
        {
            get
            {
                List<string> regexes = new List<string>();
                regexes.Add("[sS][0-9]+");
                regexes.Add("[sS][0-9]+[eE][0-9]+-*[eE]*[0-9]*");
                regexes.Add("[0-9]+[xX][0-9]+");

                return regexes;
            }
        }

        public static void LoadSettings()
        {
            if (!settingsLoaded)
            {
                settings = MESDBHandler.LoadSettings();
                settingsLoaded = true;
            }
        }

        public static void SaveSettings(bool isNew = false)
        {
            MESDBHandler.SaveSettings(settings, isNew);
            SettingsChanged = true;
        }
    }
}
