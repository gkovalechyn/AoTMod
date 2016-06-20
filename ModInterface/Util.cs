using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AoTModAPI {
    public class Util {
        private static Regex colorRegex = new Regex("(\\[(([0-F]{6})|(\\-))\\])", RegexOptions.IgnoreCase);
        public static string stripColorCodes(string src) {
            if (!string.IsNullOrEmpty(src)) {
                return colorRegex.Replace(src, "");
            }
            return src;
        }
    }
}
