using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheMagic
{
    public class SeriesTitle
    {
        public string OriginalTitle { get; set; }
        public string CustomTitle { get; set; }
        public bool IsNew { get; set; }
        public int Id { get; set; }

        public bool TitleChanged
        {
            get
            {
                return this.OriginalTitle.ToLower() != this.CustomTitle.ToLower();
            }
        }

        public SeriesTitle()
        {
            OriginalTitle = String.Empty;
            CustomTitle = String.Empty;
            IsNew = false;
        }

        public override string ToString()
        {
            return !TitleChanged ? CustomTitle : String.Format("{0} (⇐ {1})", CustomTitle, OriginalTitle);
        }
    }
}
