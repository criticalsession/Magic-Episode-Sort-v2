using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    internal class SourceDirectoriesManager
    {
        public List<string> SourceDirectories
        {
            get
            {
                List<string> directories = new List<string>();
                foreach (string d in SettingsManager.settings.sources.Split(";"))
                {
                    if (!String.IsNullOrWhiteSpace(d) && !directories.Contains(d))
                    {
                        directories.Add(d);
                    }
                }

                return directories;
            }
        }

        internal bool AddDirectory(string dir)
        {
            List<string> directories = SourceDirectories;
            if (!directories.Contains(dir))
            {
                directories.Add(dir);
                SettingsManager.settings.sources = String.Join(';', directories);

                return true;
            }
            else return false;
        }

        internal bool RemoveDirectory(string dir)
        {
            List<string> directories = SourceDirectories;
            if (directories.Contains(dir))
            {
                directories.Remove(dir);
                SettingsManager.settings.sources = String.Join(';', directories);

                return true;
            }
            else return false;
        }
    }
}
