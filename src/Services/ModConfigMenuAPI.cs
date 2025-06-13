using System;
using System.Collections.Generic;
using ModConfigMenu.Objects;

namespace ModConfigMenu
{
    public static class ModConfigMenuAPI
    {      
        public delegate bool ConfigStoredDelegate(Dictionary<string, object> currentConfig, out string feedbackMessage);

        /// <summary>
        /// Register a mod to allow MCM to configure it via UI.
        /// </summary>
        /// <param name="modName">The label for your mod. Used to display a beautified name for your mod in the UI.</param>
        /// <param name="configFilePath">Path with extension to the config file of your mod.</param>
        /// <param name="OnConfigSaved">This function will be executed every time your mod config is saved in-game. It will additionally include all properties in a Dictionary</param>
        public static void RegisterModConfig(string modName, string configFilePath, ConfigStoredDelegate OnConfigSaved)
        {
            ModConfigManager.LoadModData(modName, configFilePath, OnConfigSaved);
        }

        /// <summary>
        /// Register a mod to allow MCM to configure it via UI.
        /// </summary>
        /// <param name="modName">The label for your mod. Used to display a beautified name for your mod in the UI.</param>
        /// <param name="configData">Your mod data, passing a list of ConfigValues. A configValue is one configurable parameter from your mod.</param>
        /// <param name="OnConfigSaved">This delegate will be executed every time your mod config is saved in-game. It will include all properties in your modconfig, indexed by the same name you configured in configData.</param>
        public static void RegisterModConfig(string modName, List<ConfigValue> configData, ConfigStoredDelegate OnConfigSaved)
        {
            ModConfigManager.LoadModData(modName, configData, OnConfigSaved);
        }

        /// <summary>
        /// Returns the filename structure for your given config filename.
        /// </summary>
        /// <param name="fileName">Your file name with extension.</param>
        /// <returns>The name of the file controlled by MCM with .ini extension</returns>
        public static string GetNameForConfigFile(string fileName)
        {
            return FileHandler.GetModControlledFilename(fileName);
        }
    }
}