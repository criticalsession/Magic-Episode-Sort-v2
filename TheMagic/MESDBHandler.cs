using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheMagic.Models;

namespace TheMagic
{
    internal class MESDBHandler
    {
        #region Connection String
        private static string connectionString = @"Data Source=" + Path.Combine(SettingsManager.settingsFolder, "MESDB.db") + ";";
        private static SqliteConnection GetConnection()
        {
            return new SqliteConnection(connectionString);
        }
        #endregion

        #region Build DB
        public static void BuildDB()
        {
            if (!Directory.Exists(SettingsManager.settingsFolder)) { Directory.CreateDirectory(SettingsManager.settingsFolder); }
            using (SqliteConnection conn = GetConnection())
            {
                conn.Execute(@"CREATE TABLE ""SeriesTitles"" (
	                ""id""	INTEGER NOT NULL UNIQUE,
	                ""original""	TEXT NOT NULL,
	                ""custom""	TEXT NOT NULL,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                )");

                conn.Execute(@"CREATE TABLE ""Settings"" (
	                ""askNew""	INTEGER NOT NULL DEFAULT 1,
	                ""searchSub""	INTEGER NOT NULL DEFAULT 1,
	                ""recursive""	INTEGER NOT NULL DEFAULT 1,
                    ""renameFilenames""	INTEGER NOT NULL DEFAULT 0,
	                ""useTvMaze""	INTEGER NOT NULL DEFAULT 0,
	                ""outputDirectory""	TEXT NOT NULL,
	                ""openOutput""	INTEGER NOT NULL DEFAULT 0
                )");

                conn.Execute(@"CREATE TABLE ""Sources"" (
	                ""id""	INTEGER NOT NULL UNIQUE,
	                ""source""	TEXT NOT NULL,
	                PRIMARY KEY(""id"" AUTOINCREMENT)
                )");
            }

            CreateSkipDirectoriesTable();
        }

        public static void RunDBUpdates()
        {
            // TODO: add versioning
            using (SqliteConnection conn = GetConnection())
            {
                if (!ColExists("Settings", "renameFilenames"))
                    conn.Execute("ALTER TABLE Settings ADD COLUMN renameFilenames INTEGER NOT NULL DEFAULT 0");

                if (!ColExists("Settings", "deleteParent"))
                    conn.Execute("ALTER TABLE Settings ADD COLUMN deleteParent INTEGER NOT NULL DEFAULT 0");
            }
        }

        private static bool ColExists(string table, string column)
        {
            using (SqliteConnection conn = GetConnection())
            {
                int rowCount = conn.ExecuteScalar<int>("SELECT count(*) FROM pragma_table_info(@TableName) WHERE name=@ColumnName;", new { TableName = table, ColumnName = column });
                if (rowCount > 0) return true;
            }

            return false;
        }

        private static void CreateSkipDirectoriesTable()
        {
            using (SqliteConnection conn = GetConnection())
            {
                conn.Execute(@"CREATE TABLE ""SkipDirectories"" (
                    ""id"" INTEGER NOT NULL UNIQUE,
                    ""dir"" TEXT NOT NULL,
                    PRIMARY KEY(""id"" AUTOINCREMENT)
                )");
            }
        }

        public static void CheckTablesAreAllSetup()
        {
            if (!TableExists("SkipDirectories"))
                CreateSkipDirectoriesTable();
        }

        private static bool TableExists(string tableName)
        {
            bool found = false;
            using (SqliteConnection conn = GetConnection())
            {
                int rowCount = conn.ExecuteScalar<int>("SELECT count(*) FROM sqlite_master WHERE type='table' AND name=@TableName;", new { TableName = tableName });
                if (rowCount > 0) found = true;
            }

            return found;
        }
        #endregion

        #region Settings
        internal static Settings LoadSettings()
        {
            Settings result = new Settings();

            using (SqliteConnection conn = GetConnection())
            {
                IEnumerable<SettingsModel> output = conn.Query<SettingsModel>("select * from Settings", new DynamicParameters());
                if (output.Count() > 0)
                {
                    SettingsModel loaded = output.First();
                    result = new Settings()
                    {
                        askForNewSeriesNames = loaded.askNew,
                        openOutputDirectoryAfterSort = loaded.openOutput,
                        outputDirectory = loaded.outputDirectory,
                        recursiveSearchSubFolders = loaded.recursive,
                        searchSubFolders = loaded.searchSub,
                        useTVMazeApi = loaded.useTvMaze,
                        renameFilenames = loaded.renameFilenames,
                        deleteParentFolder = loaded.deleteParent
                    };

                    CheckOutputDirectoryExists(result);
                }
                else
                {
                    SaveSettings(result, true);
                }
            }

            return result;
        }

        private static void CheckOutputDirectoryExists(Settings settings)
        {
            if (!String.IsNullOrEmpty(settings.outputDirectory) && !Directory.Exists(settings.outputDirectory))
            {
                settings.outputDirectory = "";
                SaveSettings(settings);
            }
        }

        internal static void SaveSettings(Settings toSave, bool isNew = false)
        {
            using (SqliteConnection conn = GetConnection())
            {
                SettingsModel model = new SettingsModel();
                model.Fill(toSave);

                if (isNew)
                {
                    conn.Execute("insert into Settings(askNew, openOutput, outputDirectory, recursive, searchSub, useTvMaze, renameFilenames, deleteParent) values " +
                        "(@askNew, @openOutput, @outputDirectory, @recursive, @searchSub, @useTvMaze, @renameFilenames, @deleteParent)", model);
                }
                else
                {
                    conn.Execute("update Settings set " +
                        "askNew = @askNew," +
                        "openOutput = @openOutput," +
                        "outputDirectory = @outputDirectory," +
                        "recursive = @recursive," +
                        "searchSub = @searchSub," +
                        "renameFilenames = @renameFilenames," +
                        "useTvMaze = @useTvMaze," +
                        "deleteParent = @deleteParent", model);
                }
            }
        }
        #endregion

        #region Custom Titles
        internal static List<SeriesTitle> LoadCustomTitles()
        {
            List<SeriesTitle> result = new List<SeriesTitle>();

            using (SqliteConnection conn = GetConnection())
            {
                IEnumerable<SeriesTitleModel> output = conn.Query<SeriesTitleModel>("select * from SeriesTitles", new DynamicParameters());
                foreach (var o in output)
                {
                    result.Add(new SeriesTitle()
                    {
                        CustomTitle = o.custom,
                        OriginalTitle = o.original,
                        Id = o.id
                    });
                }
            }

            return result;
        }

        internal static void SaveCustomTitle(SeriesTitle title)
        {
            SeriesTitleModel model = new SeriesTitleModel();
            model.Fill(title);

            using (SqliteConnection conn = GetConnection())
            {
                if (title.IsNew)
                {
                    conn.Execute("insert into SeriesTitles(original, custom) values " +
                        "(@original, @custom)", model);
                }
                else
                {
                    conn.Execute("update SeriesTitles set " +
                        "custom = @custom " +
                        "where original = @original", model);
                }
            }
        }

        internal static void DeleteCustomTitle(string originalTitle)
        {
            using (SqliteConnection conn = GetConnection())
            {
                conn.Execute("delete from SeriesTitles where original = @original", new
                {
                    original = originalTitle
                });
            }
        }
        #endregion

        #region Sources
        internal static List<SourceDirectory> LoadSourceDirectories()
        {
            List<SourceDirectory> result = new List<SourceDirectory>();

            using (SqliteConnection conn = GetConnection())
            {
                IEnumerable<SourceModel> output = conn.Query<SourceModel>("select * from Sources", new DynamicParameters());
                foreach (var o in output)
                {
                    result.Add(new SourceDirectory()
                    {
                        Id = o.id,
                        SourcePath = o.source
                    });
                }
            }

            return result;
        }

        internal static void AddSourceDirectory(string dir)
        {
            AddSourceDirectory(new SourceDirectory() { SourcePath = dir });
        }

        internal static void AddSourceDirectory(SourceDirectory dir)
        {
            SourceModel model = new SourceModel();
            model.Fill(dir);

            using (SqliteConnection conn = GetConnection())
            {
                conn.Execute("insert into Sources(source) values " +
                    "(@source)", model);
            }
        }

        internal static void DeleteSourceDirectory(string dir)
        {
            using (SqliteConnection conn = GetConnection())
            {
                conn.Execute("delete from Sources where source = @source", new { source = dir });
            }
        }
        #endregion

        #region Skip Directories
        internal static List<SkipDirectory> LoadSkipDirectories()
        {
            List<SkipDirectory> result = new List<SkipDirectory>();

            using (SqliteConnection conn = GetConnection())
            {
                IEnumerable<SkipDirectoryModel> output = conn.Query<SkipDirectoryModel>("select * from SkipDirectories", new DynamicParameters());
                foreach (var o in output)
                {
                    result.Add(new SkipDirectory()
                    {
                        Id = o.id,
                        Directory = o.dir
                    });
                }
            }

            return result;
        }

        internal static void DeleteSkipDirectory(string dir)
        {
            using (SqliteConnection conn = GetConnection())
            {
                conn.Execute("delete from SkipDirectories where dir = @dir", new { dir = dir });
            }
        }

        internal static void AddSkipDirectory(string dir)
        {
            using (SqliteConnection conn = GetConnection())
            {
                conn.Execute("insert into SkipDirectories(dir) values " +
                    "(@dir)", new { dir = dir });
            }
        }
        #endregion
    }
}
