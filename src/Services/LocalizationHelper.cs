using MGSC;
using UnityEngine;

namespace ModConfigMenu.Services
{
    public class LocalizationHelper
    {
        public static void AddKeyToAllDictionaries(string key, string value)
        {
            foreach (var singleDictionary in Singleton<Localization>.Instance.db)
            {
                var currentDict = singleDictionary.Value;
                if (currentDict != null && !currentDict.ContainsKey(key))
                {
                    currentDict.Add(key, value);
                    //Logger.LogDebug($"Adding {key} with {value} in {singleDictionary.Key}");
                }
            }
        }
    }
}
