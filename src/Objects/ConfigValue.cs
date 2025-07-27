using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using MGSC;
using ModConfigMenu.Services;
using System.Xml.Linq;

namespace ModConfigMenu.Objects
{
    /// <summary>
    /// Conserves a config value
    /// </summary>
    public class ConfigValue
    {
        /// <summary>
        /// The name of the property
        /// </summary>
        public string Key = string.Empty;

        /// <summary>
        /// Value stored with the type set.
        /// </summary>
        public object Value;

        /// <summary>
        /// The value that needs to be saved to Value property.
        /// Can be discarded or applied from outside.
        /// </summary>
        public object UnstoredValue;

        /// <summary>
        /// Storage for all comments in string format. Must be parsed before used.
        /// This contains full comments with no parsing.
        /// </summary>
        //public List<string> Comments = new List<string>();
        
        /// <summary>
        /// Storage for all the comments converted to properties, which facilitates the search for data.
        /// Currently used properties are: min, max, default, type, tooltip, label.
        /// </summary>
        public List<MetaData> Properties = new List<MetaData>();

        /// <summary>
        /// Properties are two-split values that contain desired values.
        /// Such as min, max, default and others.
        /// Differentiates from comments, as this contains special types as values.
        /// </summary>
        //public Dictionary<string, string> Properties = new Dictionary<string, string>();

        /// <summary>
        /// What category this data belongs to.
        /// Headers are defined between square brackets. []
        /// </summary>
        public string Header = string.Empty;

        public Action OnValueChanged;

        public ConfigValue()
        {
            
        }

        public ConfigValue(string key, object value, List<MetaData> properties, string header)
        {
            this.Key = key;
            this.Value = value;
            this.Properties = new List<MetaData>();
            this.Properties.AddRange(properties);
            this.Header = header;
        }
        
        /// <summary>
        /// A constructor for strings
        /// </summary>
        /// <param name="key">The key which to identify the configuration.</param>
        /// <param name="value">The starting value of the configuration.</param>
        /// <param name="header">Category in UI where to place value under.</param>
        public ConfigValue(string key, object value, string header)
        {
            this.Key = key;
            this.Value = value;
            this.Header = header;
            this.Properties = new List<MetaData>();
        }

        /// <summary>
        /// A constructor for booleans or colors.
        /// </summary>
        /// <param name="key">The key which to identify the configuration.</param>
        /// <param name="value">The starting value of the configuration.</param>
        /// <param name="header">Category in UI where to place value under.</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="tooltip">Text that appears when value is hovered. Used to clarify config's functionality</param>
        /// <param name="label">Alternative name for the variable.</param>
        public ConfigValue(string key, object value, string header, object defaultValue, string tooltip, string label)
        {
            this.Key = key;
            this.Value = value;
            this.Properties = new List<MetaData>();
            this.Properties.Add(new MetaData("default", defaultValue));
            this.Properties.Add(new MetaData("label", label));
            this.Properties.Add(new MetaData("tooltip", tooltip));
            this.Header = header;
        }

        /// <summary>
        /// A constructor for int variables that has a min-max range asociated.
        /// </summary>
        /// <param name="key">The key which to identify the configuration.</param>
        /// <param name="value">The starting value of the configuration.</param>
        /// <param name="header">Category in UI where to place value under.</param>
        /// <param name="min">Minimum value for a range. Only used if it's a numerical value.</param>
        /// <param name="max">Maximum value for a range. Only used if it's a numerical value.</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="tooltip">Text that appears when value is hovered. Used to clarify config's functionality</param>
        /// <param name="label">Alternative name for the variable.</param>
        public ConfigValue(string key, object value, string header, object defaultValue, string tooltip, string label, float min, float max)
        {
            this.Key = key;
            this.Value = value;
            this.Properties = new List<MetaData>();
            this.Properties.Add(new MetaData("default", defaultValue));
            this.Properties.Add(new MetaData("label", label));
            this.Properties.Add(new MetaData("tooltip", tooltip));
            this.Properties.Add(new MetaData("min", min));
            this.Properties.Add(new MetaData("max", max));
            this.Header = header;
        }

        /// <summary>
        /// A constructor for dropdowns
        /// </summary>
        /// <param name="key">The key which to identify the configuration.</param>
        /// <param name="value">The starting value of the configuration.</param>
        /// <param name="header">Category in UI where to place value under.</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="tooltip">Text that appears when value is hovered. Used to clarify config's functionality</param>
        /// <param name="label">Alternative name for the variable.</param>
        /// <param name="orderedDropdownOptions">Ordered dropdown options. Only used if the property is a dropdown.</param>
        public ConfigValue(string key, object value, string header, object defaultValue, string tooltip, string label, List<string> orderedDropdownOptions)
        {
            this.Key = key;
            this.Value = value;
            this.Properties = new List<MetaData>();
            this.Properties.Add(new MetaData("default", defaultValue));
            this.Properties.Add(new MetaData("label", label));
            this.Properties.Add(new MetaData("tooltip", tooltip));
            if (orderedDropdownOptions != null)
            {
                for (int i = 0; i < orderedDropdownOptions.Count; i++)
                {
                    this.Properties.Add(new MetaData(i.ToString(), orderedDropdownOptions[i]));
                }
            }
            this.Header = header;
        }

        /// <summary>
        /// A constructor with all optional-parameters set.
        /// </summary>
        /// <param name="key">The key which to identify the configuration.</param>
        /// <param name="value">The starting value of the configuration.</param>
        /// <param name="header">Category in UI where to place value under.</param>
        /// <param name="min">Minimum value for a range. Only used if it's a numerical value.</param>
        /// <param name="max">Maximum value for a range. Only used if it's a numerical value.</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="tooltip">Text that appears when value is hovered. Used to clarify config's functionality</param>
        /// <param name="label">Alternative name for the variable.</param>
        /// <param name="orderedDropdownOptions">Ordered dropdown options. Only used if the property is a dropdown.</param>
        public ConfigValue(string key, object value, string header, object defaultValue, string tooltip, string label, List<string> orderedDropdownOptions, Type typeForce = null, float min = 1.0f, float max = 1.0f)
        {
            this.Key = key;
            this.Value = value;
            this.Properties = new List<MetaData>();
            this.Properties.Add(new MetaData("default", defaultValue));
            this.Properties.Add(new MetaData("min", min));
            this.Properties.Add(new MetaData("max", max));
            this.Properties.Add(new MetaData("label", label));
            this.Properties.Add(new MetaData("tooltip", tooltip));
            if (typeForce != null)
                this.Properties.Add(new MetaData("type", typeForce));
            if (orderedDropdownOptions != null)
            {
                for (int i = 0; i < orderedDropdownOptions.Count; i++)
                {
                    this.Properties.Add(new MetaData(i.ToString(), orderedDropdownOptions[i]));
                }
            }
            this.Header = header;
        }

        public void ResetDefault()
        {
            var defaultValue = GetDefault();
            if (defaultValue == null)
            {
                Logger.LogWarning($"Could not reset default for {Key}");
                return;
            }
            this.Value = defaultValue;
        }

        #region Debug

        public void PrintDebug()
        {
            string msg = "";

            msg += $"DataBlock Debug:\n";
            msg += $"Header: {Header}\n";
            msg += $"Properties:\n";

            foreach (var singleComment in Properties)
            {
                msg += $"{singleComment.GetPrintable()}\n";
            }

            msg += $"Value: {Value} as {Value.GetType()}\n";
            Logger.LogDebug(msg);
        }

        #endregion

        #region Values

        public void Save()
        {
            if (UnstoredValue != null)
            {
                Value = UnstoredValue;
                ClearUnstored();
            }
        }

        public void ClearUnstored()
        {
            UnstoredValue = null;
        }

        public void SetUnstoredValue<T>(T value)
        {
            if (value == null) return;
            Logger.LogDebug($"Setting unstored value {value} as {value.GetType()}");
            this.UnstoredValue = value;
            OnValueChanged?.Invoke();
        }

        private void SetValue<T>(T value)
        {
            if (value == null) return;
            this.Value = value;
            OnValueChanged?.Invoke();
        }

        public object GetValue()
        {
            return Value;
        }

        public object GetUnstoredValue()
        {
            return UnstoredValue;
        }

        #endregion

        #region Comments

        public object GetDefault()
        {
            var defaultValue = GetPropertyValue("default");
            if (defaultValue == null) return null;

            Type valueType = Value.GetType();
            //Logger.LogDebug($"Looking for default.\n{Value} is {Value?.GetType()}\nDefault: {defaultValue} is {defaultValue?.GetType()}");
            if (defaultValue.GetType() == valueType)
            {
                return defaultValue;
                //return Convert.ChangeType(defaultValue, valueType);
            }

            Logger.LogDebug($"DataBlock {Key} does not have default value as {valueType}");
            return null;
        }

        public float GetMax()
        {
            var validResult = GetPropertyValue("max");
            if (validResult is float floatValue)
            {
                return floatValue;
            }
            else if( validResult is int intResult)
            {
                return (float)intResult;
            }
            else
            {
                Logger.LogDebug($"DataBlock {Key} does not have \"max\" value as float nor int");
                return 1f;
            }
        }

        public float GetMin()
        {
            var validResult = GetPropertyValue("min");
            if (validResult is float floatValue)
            {
                return floatValue;
            }
            else if (validResult is int intResult)
            {
                return (float)intResult;
            }
            else
            {
                Logger.LogDebug($"DataBlock {Key} does not have \"min\" value as float nor int");
                return 1f;
            }
        }

        public string GetTypeProp()
        {
            var validResult = GetPropertyValue("type");
            if (validResult is string typeProp && !string.IsNullOrEmpty(typeProp))
            {
                return typeProp;
            }
            else
            {
                return string.Empty;
            }
        }

        public List<string> GetDropdownOptions()
        {
            List<string> returnVal = new List<string>();
            foreach (var comment in Properties)
            {
                if (int.TryParse(comment.Key, out int result))
                {
                    Logger.LogDebug($"Dropdown option for {comment.Key} is {comment.Value}");
                    returnVal.Add(comment.Value as string);
                }
            }
            return returnVal;
        }

        public string GetLabel()
        {
            var validResult = GetPropertyValue("label");
            if (validResult is string labelProp && !string.IsNullOrEmpty(labelProp))
            {
                return labelProp;
            }
            else
            {
                Logger.LogDebug($"DataBlock {Key} does not have a label.");
                return string.Empty;
            }
        }

        public string GetTooltip()
        {
            var validResult = GetPropertyValue("tooltip");
            if (validResult is string tooltip && !string.IsNullOrEmpty(tooltip))
            {
                return tooltip;
            }
            else
            {
                Logger.LogDebug($"DataBlock {Key} does not have \"description\".");
                return string.Empty;
            }
        }

        //public void AddComment(string comment)
        //{
        //    if (string.IsNullOrEmpty(comment)) return;

        //    Comments.Add(comment);
        //}
        
        public void AddProperty(string untrimmedLine)
        {
            if (string.IsNullOrEmpty(untrimmedLine))
            {
                return;
            }
            // Create key and value.
            var trimmedValues = untrimmedLine.Split(new[] { ' ' }, 2);
            string key = trimmedValues[0].Replace("#", string.Empty);
            var value = ConvertHelper.ConvertValue(trimmedValues[1]);
            MetaData newProp = new MetaData(key, value);
            Properties.Add(newProp);
        }

        //public string GetComment(string start)
        //{
        //    return Comments.Find(x => x.StartsWith(start, StringComparison.CurrentCultureIgnoreCase))?.Replace(start, string.Empty).Trim() ?? string.Empty;
        //}

        public MetaData? GetProperty(string name)
        {
            if (Properties.Exists(x => x.Key == name))
            {
                return Properties.Find(x => x.Key == name);
            }
            else
            {
                return null;
            }
        }

        public object GetPropertyValue(string name)
        {
            return GetProperty(name)?.Value ?? null;
        }

        //public T GetPropertyValue<T>(string name)
        //{
        //    return GetProperty(name).Value as T;
        //}

        //public void AddProperty(string key, string value)
        //{
        //    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value)) return;

        //    if (Properties.ContainsKey(key)) return;

        //    Properties.Add(key, value);
        //}

        //public void AddProperty(KeyValuePair<string, string> entry)
        //{
        //    AddProperty(entry.Key, entry.Value);
        //}

        //[CanBeNull]
        //public string GetProperty(string key)
        //{
        //    Properties.TryGetValue(key, out string value);
        //    return value;
        //}

        //public string GetComment(int index)
        //{
        //    return Comments.Count < index ? string.Empty : Comments[index];
        //}

        //public List<string> GetComments()
        //{
        //    return Comments;
        //}

        #endregion
    }
}
