using ModConfigMenu.Services;
using System;
using System.Collections.Generic;

namespace ModConfigMenu.Contracts
{
    public abstract class BaseConfig : IConfigValue
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public object UnstoredValue { get; set; }
        public List<MetaData> Properties { get; set; }
        public string Header { get; set; }
        public Action OnValueChanged { get; set; }

        #region Values

        public void Save()
        {
            if (UnstoredValue != null)
            {
                Value = UnstoredValue;
                ClearUnstored();
            }
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
            else if (validResult is int intResult)
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

        public List<object> GetDropdownOptions()
        {
            return GetProperty("dropdowns")?.Value as List<object> ?? new List<object>();
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

        private MetaData? GetProperty(string name)
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

        private object GetPropertyValue(string name)
        {
            return GetProperty(name)?.Value ?? null;
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

        #endregion
    }

}