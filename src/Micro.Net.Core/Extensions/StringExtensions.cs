using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Micro.Net.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Replace(this string text, Dictionary<string, string> replacements)
        {
            return Regex.Replace(text,
                $"({String.Join("|", replacements.Keys)})",
                delegate (Match m) { return replacements[m.Value]; }
            );
        }

        public static string Replace(this string text, params (string value,string replacement)[] replacements)
        {
            return text.Replace(replacements.ToDictionary(x => x.value, y => y.replacement));
        }
    }
}
