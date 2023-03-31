using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMagic.Models
{
    public class SeriesTitleModel
    {
        public int id { get; set; }
        public string original { get; set; }
        public string custom { get; set; }

        internal void Fill(SeriesTitle title)
        {
            id = title.Id;
            original = title.OriginalTitle;
            custom = title.CustomTitle;
        }
    }
}
