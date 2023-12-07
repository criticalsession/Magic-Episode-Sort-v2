using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic.Models
{
    public class SettingsModel
    {
        public bool askNew { get; set; }
        public bool searchSub { get; set; }
        public bool recursive { get; set; }
        public bool useTvMaze { get; set; }
        public string outputDirectory { get; set; }
        public bool openOutput { get; set; }
        public bool renameFilenames { get; set; }
        public bool deleteParent { get; set; }

        internal void Fill(Settings settings)
        {
            askNew = settings.askForNewSeriesNames;
            openOutput = settings.openOutputDirectoryAfterSort;
            outputDirectory = settings.outputDirectory;
            recursive = settings.recursiveSearchSubFolders;
            searchSub = settings.searchSubFolders;
            useTvMaze = settings.useTVMazeApi;
            renameFilenames = settings.renameFilenames;
            deleteParent = settings.deleteParentFolder;
        }
    }
}
