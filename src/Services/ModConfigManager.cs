using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModConfigMenu.Objects;
using UnityEngine;
using static ModConfigMenu.ModConfigMenuAPI;

namespace ModConfigMenu
{
    internal static class ModConfigManager
    {
        private static Dictionary<string, ModConfig> allModsConfigData = new Dictionary<string, ModConfig>();

        /// <summary>
        /// This corresponds to the list of mods that have a mod config available.
        /// </summary>
        public static List<string> ModsList => allModsConfigData.Keys.ToList();

        /// <summary>
        /// Old, deprecated function but kept as retrocompatibility
        /// </summary>
        /// <param name="modName"></param>
        /// <param name="filePath"></param>
        /// <param name="ModOnConfigSaved"></param>
        [Obsolete]
        public static void LoadModData(string modName, string filePath, Action<Dictionary<string, object>> ModOnConfigSaved)
        {
            Logger.SetContext(modName);
            Logger.LogDebug($"Config file: {filePath}");

            if (string.IsNullOrEmpty(filePath)) return;

            if (!File.Exists(filePath))
            {
                Logger.LogError($"Error when trying to load mod {modName}. File at {filePath} does not exist.");
                return;
            }

            ModConfig modConfigData = new ModConfig(modName, filePath, ModOnConfigSaved);
            modConfigData.Parse();

            allModsConfigData.Add(modName, modConfigData);
            Logger.ClearContext();
        }


        /// <summary>
        /// This function handles .ini to load and store values.
        /// </summary>
        /// <param name="modName"></param>
        /// <param name="filePath"></param>
        /// <param name="ModOnConfigSaved"></param>
        public static void LoadModData(string modName, string filePath, ConfigStoredDelegate ModOnConfigSaved)
        {
            Logger.SetContext(modName);
            Logger.LogDebug($"Config file: {filePath}");

            if (string.IsNullOrEmpty(filePath)) return;

            if (!File.Exists(filePath))
            {
                Logger.LogError($"Error when trying to load mod {modName}. File at {filePath} does not exist.");
                return;
            }

            ModConfig modConfigData = new ModConfig(modName, filePath, ModOnConfigSaved);
            modConfigData.Parse();
            allModsConfigData.Add(modName, modConfigData);
            Logger.ClearContext();
            Logger.Flush();
        }

        /// <summary>
        /// This one loads from a list of configs. Does not use any .ini
        /// </summary>
        /// <param name="modName"></param>
        /// <param name="configData"></param>
        /// <param name="OnConfigSaved"></param>
        /// <returns></returns>
        public static bool LoadModData(string modName, List<ConfigValue> configData, ConfigStoredDelegate OnConfigSaved)
        {
            Logger.SetContext(modName);
            Logger.LogDebug($"Mod is being registered with data-block list.");
            if (configData == null || configData.Count <= 0)
            {
                Logger.LogError($"ERROR: ConfigData for Mod \"{modName}\" is null or empty.");
                return false;
            }

            ModConfig modConfigData = new ModConfig(modName, configData, OnConfigSaved);
            allModsConfigData.Add(modName, modConfigData);
            Logger.ClearContext();
            Logger.Flush();
            return true;
        }

        public static ModConfig GetModConfig(string modName)
        {
            allModsConfigData.TryGetValue(modName, out ModConfig data);
            return data;
        }
    }
}
