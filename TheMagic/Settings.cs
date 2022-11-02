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
        [JsonProperty]
        internal bool askForNewSeriesNames = true;
        [JsonProperty]
        internal bool searchSubFolders = true;
        [JsonProperty]
        internal bool recursiveSearchSubFolders = true;
        [JsonProperty] 
        internal string outputDirectory = "";
        [JsonProperty] 
        internal string sources = "";
        [JsonProperty]
        internal bool openOutputDirectoryAfterSort = false;
    }
}
