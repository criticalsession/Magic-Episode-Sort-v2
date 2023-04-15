using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    public class DirectoriesManager
    {
        public List<SourceDirectory> SourceDirectories
        {
            get
            {
                List<SourceDirectory> directories = MESDBHandler.LoadSourceDirectories();
                directories = CheckDirectoriesExist(directories);

                return directories;
            }
        }

        public List<SkipDirectory> SkipDirectories
        {
            get
            {
                List<SkipDirectory> directories = MESDBHandler.LoadSkipDirectories();
                return directories;
            }
        }

        private List<SourceDirectory> CheckDirectoriesExist(List<SourceDirectory> directories)
        {
            List<SourceDirectory> checkedDirectories = new List<SourceDirectory>();
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
            List<SourceDirectory> directories = MESDBHandler.LoadSourceDirectories();
            if (!directories.Any(p => p.SourcePath == dir))
            {
                MESDBHandler.AddSourceDirectory(dir);
                SettingsManager.SettingsChanged = true;

                return true;
            }
            
            
            return false;
        }

        public void RemoveSourceDirectory(SourceDirectory dir)
        {
            RemoveSourceDirectory(dir.SourcePath);
        }

        public bool RemoveSourceDirectory(string? dir)
        {
            if (!String.IsNullOrEmpty(dir))
            {
                List<SourceDirectory> directories = MESDBHandler.LoadSourceDirectories();
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
            List<SkipDirectory> directories = MESDBHandler.LoadSkipDirectories();
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
            if (!String.IsNullOrEmpty(dir))
            {
                List<SkipDirectory> directories = MESDBHandler.LoadSkipDirectories();
                if (directories.Any(p => p.Directory == dir))
                {
                    MESDBHandler.DeleteSkipDirectory(dir);
                    SettingsManager.SettingsChanged= true;

                    return true;
                }
            }

            return false;
        }

        public List<string> SourceDirectoryPaths
        {
            get
            {
                return SourceDirectories.Select(p => p.SourcePath).ToList();
            }
        }

        public List<string> SkipDirectoryPaths
        {
            get
            {
                return SkipDirectories.Select(p => p.Directory).ToList();
            }
        }
    }
}
