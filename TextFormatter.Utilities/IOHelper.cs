using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextFormatter.Utilities
{
    public static class IOHelper
    {
        public static async Task SaveFileAsync(string fileLocation, string content)
        {
            using (var fileStream = new StreamWriter(fileLocation, false, Encoding.Unicode))
            {
                await fileStream.WriteAsync(content);
            }
        }

        public static async Task<string> LoadFileAsync(string fileLocation)
        {
            var stringBuilder = new StringBuilder();
            using (var fileStream = new StreamReader(fileLocation))
            {
                string line;
                while ((line = await fileStream.ReadLineAsync()) != null)
                    stringBuilder.AppendLine(line);
            }
            return stringBuilder.ToString();
        }
    }
}
