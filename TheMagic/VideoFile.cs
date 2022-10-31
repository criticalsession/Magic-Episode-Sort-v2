using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic
{
    public class VideoFile
    {
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }
        public string OriginalSeriesName { get; set; }
        public string CustomSeriesName { get; set; }
        public string FileName { get; set; }
        public int SeasonNumber { get; set; }

        public VideoFile(string path)
        {
            SourcePath = path;
            FileName = Path.GetFileName(path);
            TargetPath = path; // todo
            OriginalSeriesName = ""; // todo
            CustomSeriesName = ""; // todo
            SeasonNumber = 1; // todo
        }

        public override string ToString()
        {
            return FileName;
        }
    }
}
