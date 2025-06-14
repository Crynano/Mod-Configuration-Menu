﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ModConfigMenu
{
    internal static class ModConfigManager
    {
        private static Dictionary<string, ConfigData> allModsConfigData = new Dictionary<string, ConfigData>();

        /// <summary>
        /// This corresponds to the list of mods that have a mod config available.
        /// </summary>
        public static List<string> ModsList => allModsConfigData.Keys.ToList();

        //public static void LoadAllModData()
        //{
        //    var fileList = Directory.GetDirectories(Plugin.AllModsConfigPath);
        //    foreach (var folderPath in fileList)
        //    {
        //        DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
        //        FileInfo[] file = directoryInfo.GetFiles("*.ini");
        //        if (file.Length <= 0) continue;
        //        ConfigData modConfigData = new ConfigData("DEFAULT", file[0].FullName);
        //        modConfigData.Parse();
        //        modConfigData.Debug();
        //        allModsConfigData.Add(directoryInfo.Name, modConfigData);
        //    }
        //}

        /// <summary>
        /// Loads into memory a new mod config.
        /// </summary>
        /// <param name="modName">Should be the same as the config folder name</param>
        /// <param name="modOnConfigSaved">Code that will be Invoked when any parameter from the config is changed.</param>
        public static void LoadModData(string modName, Action<Dictionary<string, object>> modOnConfigSaved)
        {
            var finalPath = Plugin.AllModsConfigPath;
            var fileName = Directory.GetDirectories(finalPath).FirstOrDefault(x => x.EndsWith(modName));

            Debug.Log($"Filename for {modName} is {fileName}");

            if (string.IsNullOrEmpty(fileName)) return;

            DirectoryInfo directoryInfo = new DirectoryInfo(fileName);
            FileInfo[] file = directoryInfo.GetFiles("*.ini");
            if (file.Length <= 0)
            {
                Debug.Log($"No config found for mod: \"{modName}\"");
                return;
            }
            ConfigData modConfigData = new ConfigData(modName, file[0].FullName);
            modConfigData.Parse();
#if DEBUG
            modConfigData.Debug();
#endif
            allModsConfigData.Add(modName, modConfigData);
            modConfigData.OnConfigSaved += modOnConfigSaved;
        }

        public static void LoadModData(string modName, string filePath, Action<Dictionary<string, object>> modOnConfigSaved)
        {
            Debug.Log($"Filename for {modName} is {filePath}");

            if (string.IsNullOrEmpty(filePath)) return;

            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"Error when trying to load mod {modName}. File at {filePath} does not exist.");
                return;
            }

            ConfigData modConfigData = new ConfigData(modName, filePath);
            modConfigData.Parse();
#if DEBUG
            modConfigData.Debug();
#endif
            allModsConfigData.Add(modName, modConfigData);
            modConfigData.OnConfigSaved += modOnConfigSaved;
        }

        public static ConfigData GetModConfig(string modName)
        {
            allModsConfigData.TryGetValue(modName, out ConfigData data);
            return data;
        }
    }
}
