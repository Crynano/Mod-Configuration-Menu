using System;
using System.Globalization;
using UnityEngine;

namespace ModConfigMenu.Services
{
    internal class ConvertHelper
    {
        public static object ConvertValue(string value, Type implicitConversion = null)
        {
            // First check if there is an implicit conversion in properties
            if (implicitConversion != null)
            {
                try
                {
#if DEBUG
                    UnityEngine.Debug.Log($"Chosen dynamic type for {value} is: {implicitConversion}");
#endif
                    var returnVar = Convert.ChangeType(value, implicitConversion);
                    if (returnVar != null) return returnVar;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error on Explicit Type Conversion. Could not convert {value} to {implicitConversion}");
                }
            }

            // Try to convert to int, double, or leave as string
            if (int.TryParse(value, out int intValue))
            {
#if DEBUG
                Debug.Log($"Converting {value} to int.");
#endif
                return intValue;
            }

            if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatValue))
            //if (float.TryParse(value, out float floatValue))
            {
#if DEBUG
                Debug.Log($"Converting {value} to float.");
#endif
                return floatValue;
            }

            if (bool.TryParse(value, out bool boolValue))
            {
#if DEBUG
                Debug.Log($"Converting {value} to bool.");
#endif
                return boolValue;
            }

            if (ColorUtility.TryParseHtmlString(value.Replace("\"", string.Empty), out Color colorParsed))
            {
#if DEBUG
                Debug.Log($"Converting {value} to color.");
#endif
                return colorParsed;
            }

#if DEBUG
            Debug.Log($"Converting {value} to string.");
#endif
            return value as string;
        }
    }
}
