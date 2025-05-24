using ModConfigMenu.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ModConfigMenu.Services;
using UnityEngine;

namespace ModConfigMenu
{
    internal class ConfigData
    {
        public readonly string ModName;

        private readonly string filePath;

        private string McmFilepath => FileHandler.GetModControlledFilename(filePath);

        private readonly Dictionary<string, List<DataBlock>> data;
        public bool IsDirty { get; private set; } = false;

        public Action OnConfigChanged;

        public Action<Dictionary<string, object>> OnConfigSaved;

        public ConfigData(string modName, string filePath, string fileExtension = ".ini")
        {
            if (!filePath.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException($"File must have a {fileExtension} extension");
            }

            this.ModName = modName;
            this.filePath = filePath;
            this.data = new Dictionary<string, List<DataBlock>>();
        }

        public void Parse()
        {
            string selectedFilePath = string.Empty;

            if (File.Exists(McmFilepath))
            {
                selectedFilePath = McmFilepath;
            }
            else if (File.Exists(filePath))
            {
                selectedFilePath = filePath;
            }
            else
            {
                throw new FileNotFoundException($"The specified config file at \"{selectedFilePath}\" does not exist.");
            }

            // If a _mcm config file exists in the source, use that.

            string[] lines = File.ReadAllLines(selectedFilePath);
            string currentSection = null;

            DataBlock currentBlock = new DataBlock();

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();

                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith(";"))
                {
                    // Skip empty lines or comments
                    continue;
                }

                if (trimmedLine.StartsWith("##"))
                {
                    currentBlock.AddComment(trimmedLine);
                }
                else if (trimmedLine.StartsWith("#"))
                {
                    currentBlock.AddComment(trimmedLine.Remove(0, 1));
                }

                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    // Section header
                    currentSection = trimmedLine.Trim('[', ']');
                    if (!data.ContainsKey(currentSection))
                    {
                        data[currentSection] = new List<DataBlock>();
                    }
                }
                else if (currentSection != null && trimmedLine.Contains('='))
                {
                    // Key-value pair
                    string[] keyValue = trimmedLine.Split(new[] { '=' }, 2);
                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();

                    // Convert and assign
                    // This could be a good step to check for forced Type-castings
                    var convertedValue = ConvertValue(value);
                    currentBlock.Value = convertedValue;
                    currentBlock.Key = key;

                    // Set
                    currentBlock.OnValueChanged += DataBlockChanged;

                    // Store
                    data[currentSection].Add(currentBlock);

                    // Reset
                    currentBlock = new DataBlock();
                }
            }
        }

        private object ConvertValue(string value)
        {
            return ConvertHelper.ConvertValue(value);
        }

        public void Debug()
        {
            foreach (var headerKeyValue in data)
            {
                UnityEngine.Debug.Log($"Debugging {headerKeyValue.Key}");
                foreach (var variableList in headerKeyValue.Value)
                {
                    //UnityEngine.Debug.Log($"{variableList.Key} has {variableList.Value} of {variableList.Value.GetType()}");
                    variableList.Debug();
                }
            }
        }

        public string GetPrintableFile()
        {
            // Here we can return a new text file that can be printed to disk
            string fileText = "";
            DateTime startTime = DateTime.Now;

            foreach (var headerData in data)
            {
                fileText += $"[{headerData.Key}]\n";
                foreach (var singleDataBlock in headerData.Value)
                {
                    // Print Comments first.
                    foreach (var singleComment in singleDataBlock.Comments)
                    {
                        fileText += $"#{singleComment}\n";
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
                        correctedValue = ColorHelper.GetIniColor(color);

                    fileText += $"{singleDataBlock.Key} = {correctedValue}\n\n";
                }
            }

            UnityEngine.Debug.Log($"{(DateTime.Now - startTime).TotalSeconds:0.0000}");
            return fileText;
        }

        public void ResetAllToDefault()
        {
            GetAllDataBlocks().ForEach(x => x.ResetDefault());
        }

        internal void DataBlockChanged()
        {
#if DEBUG
            UnityEngine.Debug.Log($"A ConfigData has changed!");
#endif
            //FileHandler.WriteToFile(filePath.Replace(".ini", "_test.ini"), GetPrintableFile());
            // Set Dirty
            IsDirty = true;
            OnConfigChanged?.Invoke();
        }

        internal void Save()
        {
#if DEBUG
            UnityEngine.Debug.Log($"ConfigData has been SAVED!");
#endif
            // First Save all Blocks
            SaveDataBlocks();
            string finalFilePath = McmFilepath;
#if DEBUG
            finalFilePath.Replace(".ini", "_debug.ini");
#endif
            FileHandler.WriteToFile(finalFilePath, GetPrintableFile());
            IsDirty = false;
            OnConfigSaved?.Invoke(GetAllValues());
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
            foreach (var singleDataBlock in GetAllDataBlocks())
            {
                singleDataBlock.Save();
            }
        }

        private void DiscardDataBlocks()
        {
            foreach (var singleDataBlock in GetAllDataBlocks())
            {
                singleDataBlock.ClearUnstored();
            }
        }

        private Dictionary<string, object> GetAllValues()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (var singleDataBlock in GetAllDataBlocks())
            {
                result.Add(singleDataBlock.Key, singleDataBlock.Value);
            }
            return result;
        }

        private List<DataBlock> GetAllDataBlocks()
        {
            List<DataBlock> result = new List<DataBlock>();
            foreach (var VARIABLE in data)
            {
                result.AddRange(VARIABLE.Value);
            }
            return result;
        }

        public Dictionary<string, List<DataBlock>> GetData()
        {
            return data;
        }
    }
}