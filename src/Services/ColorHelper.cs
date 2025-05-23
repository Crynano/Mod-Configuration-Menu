using UnityEngine;

namespace ModConfigMenu.Services
{
    internal static class ColorHelper
    {
        public static string GetIniColor(Color color)
        {
            return "#" + ColorUtility.ToHtmlStringRGB(color).Replace("\"", string.Empty);
        }
    }
}
