using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    internal class Settings
    {
        internal bool askForNewSeriesNames = true;
        internal bool searchSubFolders = true;
        internal bool recursiveSearchSubFolders = true;
        internal bool useTVMazeApi = false;
        internal bool openOutputDirectoryAfterSort = false;
        internal string outputDirectory = "";
        internal bool renameFilenames = true;
    }
}
