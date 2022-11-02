using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TheMagic
{
    public static class Utils
    {
        public static string Sanitize(string val)
        {
            val = val.Replace(":", "");
            val = Regex.Replace(val, @"\s+", " ");

            return val;
        }
    }
}
