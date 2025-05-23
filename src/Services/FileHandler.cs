using System.IO;

namespace ModConfigMenu
{
    internal static class FileHandler
    {
        public static void WriteToFile(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        public static string GetModControlledFilename(string fileName)
        {
            return fileName.Replace(".ini", $"{Plugin.MCM_CONTROLLED_SUFFIX}.ini");
        }
    }
}
    