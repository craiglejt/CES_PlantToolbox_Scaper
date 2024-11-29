using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CES_WebCrawler
{
    internal static class Helpers
    {

        public static string CleanupString(string str)
        {
            str = str.Replace("&lt;", "<");

            return str.Trim();
        }

    }
}
