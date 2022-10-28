using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    internal class SourceDirectoriesManager
    {
        public List<string> SourceDirectories { get; set; } = new List<string>();

        internal bool AddDirectory(string dir)
        {
            if (!SourceDirectories.Contains(dir))
            {
                SourceDirectories.Add(dir);
                return true;
            }
            else return false;
        }

        internal bool RemoveDirectory(string dir)
        {
            if (SourceDirectories.Contains(dir))
            {
                SourceDirectories.Remove(dir);
                return true;
            }
            else return false;
        }

        internal string GetCSVDirectories()
        {
            if (SourceDirectories.Count > 0)
            {
                return String.Join(';', SourceDirectories);
            }
            else return "";
        }
    }
}
