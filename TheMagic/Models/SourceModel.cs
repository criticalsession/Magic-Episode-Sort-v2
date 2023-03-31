using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic.Models
{
    public class SourceModel
    {
        public int id { get; set; }
        public string source { get; set; }

        internal void Fill(SourceDirectory dir)
        {
            id = dir.Id;
            source = dir.SourcePath;
        }
    }
}
