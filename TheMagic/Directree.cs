using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    public class Directree
    {
        public List<string> Directories { get; set; }
        public event EventHandler DirectorySearched;

        public Directree()
        {
            Directories = new List<string>();
        }

        public void Build(List<string> sourceDirectories, bool searchSubDirectories, bool recursive)
        {
            foreach (var directory in sourceDirectories)
            {
                if (!Directories.Contains(directory))
                {
                    Directories.Add(directory);
                    DirectorySearched?.Invoke(this, EventArgs.Empty);

                    if (searchSubDirectories)
                    {
                        AddDirsInPath(directory, recursive);
                    }
                }
            }
        }

        private void AddDirsInPath(string path, bool recursive)
        {
            foreach (string subDirectory in Directory.GetDirectories(path).ToList())
            {
                if (!Directories.Contains(subDirectory))
                {
                    Directories.Add(subDirectory);
                    DirectorySearched?.Invoke(this, EventArgs.Empty);
                }

                if (recursive)
                {
                    AddDirsInPath(subDirectory, recursive);
                }
            }
        }
    }
}
