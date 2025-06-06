using UnityEngine;

namespace ModConfigMenu.Services
{
    internal static class ColorHelper
    {
        public static string GetIniColor(Color color)
        {
            return "#" + ColorUtility.ToHtmlStringRGBA(color).Replace("\"", string.Empty);
        }
    }
}
