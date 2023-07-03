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
            string[] invalidCharacters = new string[] { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            foreach (string c in invalidCharacters) val = val.Replace(c, "");

            val = Regex.Replace(val, @"\s+", " "); // trim extra spaces

            return val;
        }
    }
}
