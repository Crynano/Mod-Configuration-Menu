using System;
using System.Collections.Generic;

namespace ModConfigMenu
{
    public static class ModConfigMenuAPI
    {
        /// <summary>
        /// Register a mod to allow MCM to configure it via UI.
        /// This method assumes you have a folder with the same name as the mod with your config inside.
        /// </summary>
        /// <param name="modName">The label for your mod. Used to display a beautified name for your mod in the UI.</param>
        /// <param name="onConfigSaved">This function will be executed every time your mod config is saved in-game.</param>
        //public static void RegisterModConfig(string modName, Action onConfigSaved)
        //{
        //    ModConfigManager.LoadModData(modName, onConfigSaved);
        //}

        /// <summary>
        /// Register a mod to allow MCM to configure it via UI.
        /// </summary>
        /// <param name="modName">The label for your mod. Used to display a beautified name for your mod in the UI.</param>
        /// <param name="configFilePath">Path with extension to the config file of your mod.</param>
        /// <param name="onConfigSaved">This function will be executed every time your mod config is saved in-game. It will additionally include all properties in a Dictionary</param>
        public static void RegisterModConfig(string modName, string configFilePath, Action<Dictionary<string, object>> onConfigSaved)
        {
            ModConfigManager.LoadModData(modName, configFilePath, onConfigSaved);
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