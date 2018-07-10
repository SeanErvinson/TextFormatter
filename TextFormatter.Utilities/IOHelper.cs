using System.Text;
using System.Threading.Tasks;
using System.IO;
using log4net;
using System;

namespace TextFormatter.Utilities
{
    public static class IOHelper
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(IOHelper));

        public static async Task SaveFileAsync(string fileLocation, string content)
        {
            using (var fileStream = new StreamWriter(fileLocation, false, Encoding.Unicode))
            {
                try
                {
                    await fileStream.WriteAsync(content);                
                }
                catch (Exception ex)
                {
                    var file = new FileInfo(fileLocation);
                    _logger.Error($"{file.Name} - {file.Length}", ex);
                    throw;
                }
            }
        }

        public static async Task<string> LoadFileAsync(string fileLocation)
        {
            var stringBuilder = new StringBuilder();
            using (var fileStream = new StreamReader(fileLocation))
            {
                string line;
                try
                {
                    while ((line = await fileStream.ReadLineAsync()) != null)
                        stringBuilder.AppendLine(line);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    var file = new FileInfo(fileLocation);
                    _logger.Error($"{file.Name} - {file.Length}", ex);
                    throw;
                }
            }
            return stringBuilder.ToString();
        }
    }
}
