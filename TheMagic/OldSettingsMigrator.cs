using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    public class OldSettingsMigrator
    {
        public enum MigrationResults
        {
            NothingToMigrate,
            Successful,
            Error
        }

        public static (MigrationResults, int) Migrate()
        {
            string settingsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Magic Episode Sort");
            if (!Directory.Exists(settingsFolder)) return (MigrationResults.NothingToMigrate, 0);

            string settingsPath = Path.Combine(settingsFolder, "mes.settings");
            if (!File.Exists(settingsPath)) return (MigrationResults.NothingToMigrate, 0);

            v1.Settings? deserializedSettings = JsonConvert.DeserializeObject<v1.Settings>(File.ReadAllText(settingsPath));
            if (deserializedSettings == null) return (MigrationResults.Error, 0);

            int totalMigrated = 0;

            try
            {
                MigrateSettings(deserializedSettings);
                totalMigrated = MigrateSeriesTitles(deserializedSettings);
                MigrateSources(deserializedSettings);
            }
            catch
            {
                return (MigrationResults.Error, totalMigrated);
            }

            return (MigrationResults.Successful, totalMigrated);
        }

        private static void MigrateSources(v1.Settings deserializedSettings)
        {
            foreach (string d in deserializedSettings.sources.Split(";"))
            {
                SettingsManager.SourceDirectoriesManager.AddDirectory(d);
            }
        }

        private static int MigrateSeriesTitles(v1.Settings? deserializedSettings)
        {
            int count = 0;
            foreach (v1.SeriesTitle st in deserializedSettings.customSeriesTitles)
            {
                SettingsManager.CustomSeriesTitleManager.AddCustomSeriesTitle(st.OriginalTitle, st.CustomTitle, true);
                count++;
            }

            return count;
        }

        private static void MigrateSettings(v1.Settings? deserializedSettings)
        {
            Settings v2Settings = new Settings()
            {
                askForNewSeriesNames = deserializedSettings.askForNewSeriesNames,
                openOutputDirectoryAfterSort = deserializedSettings.openOutputDirectoryAfterSort,
                outputDirectory = deserializedSettings.outputDirectory,
                recursiveSearchSubFolders = deserializedSettings.recursiveSearchSubFolders,
                searchSubFolders = deserializedSettings.searchSubFolders,
                useTVMazeApi = deserializedSettings.useTVMazeApi
            };

            SettingsManager.settings = v2Settings;
            SettingsManager.SaveSettings(true);
        }
    }
}
