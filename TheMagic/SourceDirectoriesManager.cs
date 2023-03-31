using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    public class SourceDirectoriesManager
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

        private List<SourceDirectory> CheckDirectoriesExist(List<SourceDirectory> directories)
        {
            List<SourceDirectory> checkedDirectories = new List<SourceDirectory>();
            foreach (SourceDirectory dir in directories)
            {
                if (!Directory.Exists(dir.SourcePath))
                {
                    RemoveDirectory(dir);
                }
                else
                {
                    checkedDirectories.Add(dir);
                }
            }

            return checkedDirectories;
        }

        public bool AddDirectory(string dir)
        {
            List<SourceDirectory> directories = MESDBHandler.LoadSourceDirectories();
            if (!directories.Any(p => p.SourcePath == dir))
            {
                MESDBHandler.AddSourceDirectory(dir);
                SettingsManager.SettingsChanged = true;

                return true;
            }
            else return false;
        }

        public void RemoveDirectory(SourceDirectory dir)
        {
            RemoveDirectory(dir.SourcePath);
        }

        public bool RemoveDirectory(string? dir)
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

        public List<string> SourceDirectoryPaths
        {
            get
            {
                return SourceDirectories.Select(p => p.SourcePath).ToList();
            }
        }
    }
}
