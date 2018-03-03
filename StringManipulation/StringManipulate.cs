using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StringManipulation
{
    public class StringManipulate
    {
        /// <summary>
        /// Replace a specific word with the given pattern
        /// </summary>
        /// <param name="content">The text to be replaced</param>
        /// <param name="patternKey">The regular expression pattern to match</param>
        /// <param name="replacement">The replacement string</param>
        /// <param name="caseSensitive">Case sensitivity</param>
        /// <returns></returns>
        public static string Replace(string content, string patternKey, string replacement, bool caseSensitive = false)
        {
            if (string.IsNullOrEmpty(content))
                return null;
            string pattern = $"({patternKey})";
            replacement = replacement ?? string.Empty;
            if (caseSensitive)
                return Regex.Replace(content, pattern, replacement);
            else
                return Regex.Replace(content, pattern, replacement, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Converts text into an Array-friendly format
        /// </summary>
        /// <param name="content">The text to format</param>
        /// <param name="type">The desired output type</param>
        /// <returns></returns>
        public static string ArrayFormat(string content, ArrayType type)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            string modifiedWords = String.Empty;
            string pattern = @"\w+";

            var words = Regex.Matches(content, pattern).Cast<Match>().Select(word => word.Value).ToArray();

            if (type == ArrayType.String)
                words = words.Select(word => String.Format("\"{0}\"", word)).ToArray();                

            modifiedWords = String.Join(",", words);
            
            return modifiedWords;
        }
    }
}
