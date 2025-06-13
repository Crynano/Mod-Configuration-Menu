using ModConfigMenu.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModConfigMenu.Services;
using UnityEngine;
using static ModConfigMenu.ModConfigMenuAPI;
using MGSC;
using ModConfigMenu.Components;

namespace ModConfigMenu
{
    internal class ModConfig
    {
        public readonly string ModName;

        private readonly string filePath;

        private string McmFilepath => FileHandler.GetModControlledFilename(filePath);

        private List<ConfigValue> data;

        public bool IsDirty { get; private set; } = false;

        private bool _saveToFile = true;

        public Action OnConfigChanged;

        public ConfigStoredDelegate OnConfigSaved;

        public ModConfig(string modName, string filePath, ConfigStoredDelegate OnConfigSaved)
        {
            this.ModName = modName;
            this.filePath = filePath;
            this.OnConfigSaved = OnConfigSaved;
            this.data = new List<ConfigValue>();
        }

        public ModConfig(string modName, List<ConfigValue> userData, ConfigStoredDelegate OnConfigSaved, bool saveToFile = false)
        {
            // Directly load data. No files related.
            this.ModName = modName;
            this.data = userData;
            foreach (ConfigValue entryData in this.data)
            {
                entryData.OnValueChanged += DataBlockChanged;
            }
            this.OnConfigSaved = OnConfigSaved;
            this._saveToFile = saveToFile;
        }

        private void LoadUser()
        {
            // Import MCM config.
            // Use a new filepath
            // It is loaded inside the new constructor
            // If MCM config exists, then compare it to original.
            List<ConfigValue> mcmData = new List<ConfigValue>();

            if (File.Exists(McmFilepath))
            {
                UnityEngine.Debug.Log("Loading mcm data");
                mcmData = ParseFile(McmFilepath);
            }

            // Data stored is a mix of the two
            if (mcmData.Count > 0 )
            {
                if (data.Count > 0)
                    this.data = UpdateData(mcmData, data);
                else
                    this.data = mcmData;
            }
        }

        public void Parse()
        {
            // Import MCM config
            // Import Original config.
            // If MCM config exists, then compare it to original.
            List<ConfigValue> mcmData = new List<ConfigValue>();
            List<ConfigValue> originalData = new List<ConfigValue>();

            if (File.Exists(McmFilepath))
            {
                UnityEngine.Debug.Log("Loading mcm data");
                mcmData = ParseFile(McmFilepath);
            }

            if (File.Exists(filePath))
            {
                UnityEngine.Debug.Log("Loading filepath data");
                originalData = ParseFile(filePath);
            }

            // Data stored is a mix of the two
            if (mcmData.Count > 0 && originalData.Count > 0)
            {
                UnityEngine.Debug.Log("Trying to update mod data");
                this.data = UpdateData(mcmData, originalData);
            }
            else if (mcmData.Count > 0)
            {
                UnityEngine.Debug.Log("Error updating, only loading mcm data");
                this.data = mcmData;
            }
            else if (originalData.Count > 0)
            {
                UnityEngine.Debug.Log("Error updating, only loading original data");
                this.data = originalData;
            }
            else
            {
                UnityEngine.Debug.LogError("NO DATA HAS BEEN LOADED FOR A MOD????");
            }
        }

        private List<ConfigValue> ParseFile(string filepath)
        {
            List<ConfigValue> localData = new List<ConfigValue>();
            string[] lines = File.ReadAllLines(filepath);
            string currentSection = string.Empty;

            ConfigValue currentBlock = new ConfigValue();

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";"))
                {
                    // Skip empty lines or comments
                    continue;
                }

                if (trimmedLine.StartsWith("#"))
                {
                    currentBlock.AddProperty(trimmedLine);
                } 
                else if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    // Section header
                    currentSection = trimmedLine.Trim('[', ']');
                    //if (!localData.ContainsKey(currentSection))
                    //{
                    //    localData[currentSection] = new List<DataBlock>();
                    //}
                }
                else if (trimmedLine.Contains('='))
                {
                    // Key-value pair
                    string[] keyValue = trimmedLine.Split(new[] { '=' }, 2);
                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();

                    // Convert and assign 
                    // This could be a good step to check for forced Type-castings

                    // If there's a forced conversion, we should do that.
                    var convertingType = Type.GetType(currentBlock.GetTypeProp());
                    var convertedValue = ConvertValue(value, convertingType);
                    currentBlock.Value = convertedValue;
                    currentBlock.Key = key;
                    currentBlock.Header = currentSection;

                    // Set
                    currentBlock.OnValueChanged += DataBlockChanged;

                    // Store
                    //localData[currentSection].Add(currentBlock);
                    localData.Add(currentBlock);

                    // Reset
                    currentBlock = new ConfigValue();
                }
            }

            return localData;
        }

        private List<ConfigValue> UpdateData(List<ConfigValue> userData, List<ConfigValue> originalModData)
        {
            // For each value of the original, get the value in the userData. Otherwise, keep it default.
            // If a value is in the userdata, just ignore that.
            foreach (var dataBlock in originalModData)
            {
                // Find in userData-block
                var foundUserDatablock = userData.Find(x => x.Key.Equals(dataBlock.Key));
                // If the config already exists, copy the user value.
                if (foundUserDatablock != null)
                {
                    //Logger.LogDebug($"Found a user datablock with ID: {foundUserDatablock.Key} with value {foundUserDatablock.Value} against the original {dataBlock.Value}");
                    dataBlock.Value = foundUserDatablock.Value;
                }
            }

            return originalModData;
        }

        private object ConvertValue(string value, Type forcedConversion = null)
        {
            return ConvertHelper.ConvertValue(value, forcedConversion);
        }

        public void DebugObject()
        {
            foreach (var variableList in data)
            {
                variableList.PrintDebug();
            }
        }

        public string GetPrintableFile()
        {
            // Here we can return a new text file that can be printed to disk
            string fileText = "";
            DateTime startTime = DateTime.Now;
            string currentHeader = string.Empty;

            foreach (var singleDataBlock in data)
            {
                // Print Comments first.
                if (!singleDataBlock.Header.Equals(currentHeader))
                {
                    currentHeader = singleDataBlock.Header;
                    fileText += $"[{currentHeader}]\n";
                }

                foreach (var singleComment in singleDataBlock.Properties)
                {
                    fileText += $"#{singleComment.GetPrintable()}\n";
                }

                // Print Properties later. Even tho the description is a property.
                //foreach (var singleProp in singleDataBlock.Properties)
                //{
                //    fileText += $"#{singleProp.Key} {singleProp.Value}\n";
                //}

                // Maybe we should not the data RAW!
                // if its a color, treat it.
                object correctedValue = singleDataBlock.Value;
                if (singleDataBlock.Value is Color color)
                {
                    correctedValue = ColorHelper.GetIniColor(color);
                }

                fileText += $"{singleDataBlock.Key} = {correctedValue}\n\n";
            }


            UnityEngine.Debug.Log($"{(DateTime.Now - startTime).TotalSeconds:0.0000}");
            return fileText;
        }

        public void ResetAllToDefault()
        {
            foreach (var item in GetData())
            {
                item.ResetDefault();
            }
        }

        internal void DataBlockChanged()
        {
            IsDirty = true;
            OnConfigChanged?.Invoke();
        }

        internal bool Save(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                bool goodConfig = OnConfigSaved?.Invoke(GetAllValues(), out errorMessage) ?? false;
                // show a message if goodConfig is false
                if (!goodConfig)
                {
                    return false;
                }
                SaveDataBlocks();
                SaveToFile();
                Logger.LogInfo($"ModConfig for \"{this.ModName}\" has been saved.");
                IsDirty = false;
                return true;
            }
            catch (NullReferenceException e)
            {
                errorMessage += e.Message;
            }
            catch (Exception e)
            {
                errorMessage += e.Message;
            }
            return false;
        }

        internal void SaveToFile()
        {
            // If the user does not want a file to save to. Then ignore.
            if (!_saveToFile) return;

            string finalFilePath = McmFilepath;
//#if DEBUG
//            finalFilePath.Replace(".ini", "_debug.ini");
//#endif
            FileHandler.WriteToFile(finalFilePath, GetPrintableFile());
        }

        internal void Discard()
        {
#if DEBUG
            UnityEngine.Debug.Log($"ConfigData has been DISCARDED!");
#endif
            // First Save all Blocks
            DiscardDataBlocks();
            IsDirty = false;
        }

        private void SaveDataBlocks()
        {
            foreach (var singleDataBlock in GetData())
            {
                singleDataBlock.Save();
            }
        }

        private void DiscardDataBlocks()
        {
            foreach (var singleDataBlock in GetData())
            {
                singleDataBlock.ClearUnstored();
            }
        }

        private Dictionary<string, object> GetAllValues()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (var singleDataBlock in GetData())
            {
                result.Add(singleDataBlock.Key, singleDataBlock.Value);
            }
            return result;
        }

        public List<ConfigValue> GetData()
        {
            return data;
        }
    }
}