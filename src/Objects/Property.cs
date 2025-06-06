using ModConfigMenu.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModConfigMenu
{
    public struct Property
    {
        public readonly string Key;
        public readonly object Value;

        public Property(string key, object value)
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