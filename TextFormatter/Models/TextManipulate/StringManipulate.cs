using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextFormatter.Models.TextManipulate
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
        /// <returns>Replaced/Removed content</returns>
        public static async Task<string> Replace(string content, string patternKey, string replacement, bool caseSensitive = false)
        {
            if (string.IsNullOrEmpty(content))
                return null;
            string pattern = $"({patternKey})";
            replacement = replacement ?? string.Empty;
            if (caseSensitive)
                return await Task.Run(() => Regex.Replace(content, pattern, replacement));
            else
                return await Task.Run(() => Regex.Replace(content, pattern, replacement, RegexOptions.IgnoreCase));
        }

        /// <summary>
        /// Converts text into an Array-friendly format
        /// </summary>
        /// <param name="content">The text to format</param>
        /// <param name="type">The desired output type</param>
        /// <returns>Array friendly format</returns>
        public static async Task<string> ArrayFormat(string content, ArrayFormat type)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            string modifiedWords = String.Empty;
            string pattern = @"\w+";

            var words = await Task.Run(() => Regex.Matches(content, pattern).Cast<Match>().Select(word => word.Value).ToArray());

            if (type == TextManipulate.ArrayFormat.String)
                words = await Task.Run(() => words.Select(word => string.Format("\"{0}\"", word)).ToArray());                

            modifiedWords = await Task.Run(() => String.Join(",", words));
            
            return modifiedWords;
        }
    }
}
