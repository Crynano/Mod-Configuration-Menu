using ModConfigMenu.Services;
using UnityEngine;

namespace ModConfigMenu
{
    public class MetaData
    {
        public readonly string Key;
        public readonly object Value;

        public MetaData(string key, object value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string GetPrintable()
        {
            string returnVal = string.Empty;
            var correctedValue = Value;
            if (this.Value is Color color)
            {
                correctedValue = ColorHelper.GetIniColor(color);
            }
            returnVal = $"{this.Key} {correctedValue}";
            return returnVal;
        }
    }
}