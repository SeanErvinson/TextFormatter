using log4net;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextFormatter.Utilities
{
    public static class StringHelper
    {
        //private static readonly ILog _logger = LogManager.GetLogger(typeof(IOHelper));

        public static int AffectedCharacter { get; private set; }

        /// <summary>
        /// Converts text into an Array-friendly format
        /// </summary>
        /// <param name="content">The text to format</param>
        /// <param name="type">The desired output type</param>
        /// <returns>Array friendly format</returns>
        public static async Task<string> ArrayStructureAsync(string content, ArrayFormat type)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            const string pattern = @"\w+";

            var matches = Regex.Matches(content, pattern);

            AffectedCharacter = matches.Count;

            string[] words;
            try
            {
                words = await Task.Run(() => matches.Cast<Match>().Select(word => word.Value).ToArray());
            }
            catch (Exception ex)
            {
                //_logger.Error($"Content size = {content.Length}", ex);
                throw;
            }
            if (type == ArrayFormat.String)
                words = await Task.Run(() => words.Select(word => $"\"{word}\"").ToArray());
            else if (type == ArrayFormat.Char)
                words = await Task.Run(() => words.Select(word => $"\'{word}\'").ToArray());
            var modifiedWords = await Task.Run(() => String.Join(",", words));


            return modifiedWords;
        }

        /// <summary>
        /// Insert a character/word to either the start or the end of every word
        /// </summary>
        /// <param name="content">String to be modified</param>
        /// <param name="value">Value to be inserted</param>
        /// <param name="position">Position to where the value is be inserted</param>
        /// <returns></returns>
        public static async Task<string> InsertAsync(string content, string value, InsertPosition position)
        {
            if (string.IsNullOrEmpty(content))
                return null;
            var sb = new StringBuilder();
            const string pattern = @"\r\n?|\n";

            var words = Regex.Split(content, pattern);
            await Task.Run(() =>
            {
                foreach (var word in words)
                {
                    int index;
                    index = position == InsertPosition.Start ? 0 : word.Length;

                    try
                    {
                        sb.AppendLine(word.Insert(index, value));
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        //_logger.Error($"Content length - {content.Length}", ex);
                        throw;
                    }
                }
            });
            return sb.ToString();
        }

        /// <summary>
        /// Replace a specific word with the given pattern
        /// </summary>
        /// <param name="content">The text to be replaced</param>
        /// <param name="patternKey">The regular expression pattern to match</param>
        /// <param name="replacement">The replacement string</param>
        /// <param name="caseSensitive">Case sensitivity</param>
        /// <returns>Replaced/Removed content</returns>
        public static async Task<string> ReplaceAsync(string content, string patternKey, string replacement, bool caseSensitive = false, bool escapedCharacter = true)
        {
            if (string.IsNullOrEmpty(content))
                return null;

            // Handles the escape character for user input
            var pattern = $"({Regex.Escape(patternKey)})";
            if (!escapedCharacter)
                pattern = $"({patternKey})";

            replacement = replacement ?? string.Empty;
            try
            {
                AffectedCharacter = Regex.Matches(content, pattern).Count;
                if (caseSensitive)
                    return await Task.Run(() => Regex.Replace(content, pattern, replacement));
                return await Task.Run(() => Regex.Replace(content, pattern, replacement, RegexOptions.IgnoreCase));
            }
            catch (ArgumentException ex)
            {
                //_logger.Error($"Error occured with \"{pattern}\" pattern ", ex);
                throw;
            }
        }
    }

    public enum InsertPosition
    {
        Start,
        End
    }

    public enum ArrayFormat
    {
        String,
        Integer,
        Char
    }

    [Flags]
    public enum LetterCase
    {
        Upper = 1 << 0,
        Lower = 1 << 1
    }
}
