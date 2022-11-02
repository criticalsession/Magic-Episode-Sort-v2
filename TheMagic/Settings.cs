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
        internal bool askForNewSeriesNames;
        [JsonProperty]
        internal bool searchSubFolders;
        [JsonProperty]
        internal bool recursiveSearchSubFolders;
        [JsonProperty] 
        internal string targetDirectory = "";
        [JsonProperty] 
        internal string sources = "";
    }
}
