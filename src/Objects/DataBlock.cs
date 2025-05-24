using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using ModConfigMenu.Services;

namespace ModConfigMenu.Objects
{
    public class DataBlock
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
        public List<string> Comments = new List<string>();

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

        public DataBlock()
        {

        }

        public void ResetDefault()
        {
            var defaultValue = GetDefault();
            if (defaultValue == null)
            {
                UnityEngine.Debug.LogWarning($"Could not reset default for {Key}");
                return;
            }
            this.Value = defaultValue;
        }

        #region Debug

        public void Debug()
        {
            string msg = "";

            msg += $"DataBlock Debug:\n";
            msg += $"Header: {Header}\n";
            msg += $"Comments:\n";

            foreach (var singleComment in Comments)
            {
                msg += $"{singleComment}.\n";
            }
            //msg += "Properties:\n";
            //foreach (var singleProp in Properties)
            //{
            //    msg += $"{singleProp.Key} - {singleProp.Value}.\n";
            //}
            msg += $"Value: {Value} as {Value.GetType()}\n";
            UnityEngine.Debug.Log(msg);
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
#if DEBUG
            UnityEngine.Debug.Log($"Setting unstored value {value} as {value.GetType()}");
#endif
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

        [CanBeNull]
        public object GetDefault()
        {
            Type valueType = Value.GetType();
            string defaultComment = GetComment("default");
            object defaultValue = ConvertHelper.ConvertValue(defaultComment);
#if DEBUG
            UnityEngine.Debug.Log($"Looking for default.\n{Value} is {Value?.GetType()}\nDefault: {defaultValue} is {defaultValue?.GetType()}");
#endif
            if (defaultValue?.GetType() == valueType)
            {
                return defaultValue;
                //return Convert.ChangeType(defaultValue, valueType);
            }

            UnityEngine.Debug.Log($"DataBlock {Key} does not have default value as {valueType}");
            return null;
        }

        public float GetMax()
        {
            bool validResult = float.TryParse(GetComment("max"), out var result);
            if (validResult)
            {
                return result;
            }
            else
            {
                UnityEngine.Debug.Log($"DataBlock {Key} does not have \"max\" value as float");
                return 1f;
            }
        }

        public float GetMin()
        {
            bool validResult = float.TryParse(GetComment("min"), out var result);
            if (validResult)
            {
                return result;
            }
            else
            {
                UnityEngine.Debug.Log($"DataBlock {Key} does not have \"min\" value as float");
                return 1f;
            }
        }

        public string GetTypeProp()
        {
            string validResult = GetComment("type");
            if (!string.IsNullOrEmpty(validResult))
            {
                return validResult;
            }
            else
            {
                UnityEngine.Debug.Log($"DataBlock {Key} does not have \"{validResult}\".");
                return string.Empty;
            }
        }

        public string GetLabel()
        {
            string validResult = GetComment("label");
            if (!string.IsNullOrEmpty(validResult))
            {
                return validResult;
            }
            else
            {
                UnityEngine.Debug.Log($"DataBlock {Key} does not have \"{validResult}\".");
                return string.Empty;
            }
        }

        public string GetDescription()
        {
            string validResult = GetComment("tooltip");
            //UnityEngine.Debug.Log($"GETTING {validResult} AS DESCRIPTION FOR {Key}");
            if (!string.IsNullOrEmpty(validResult))
            {
                return validResult;
            }
            else
            {
                UnityEngine.Debug.Log($"DataBlock {Key} does not have \"description\".");
                return string.Empty;
            }
        }

        public void AddComment(string comment)
        {
            if (string.IsNullOrEmpty(comment)) return;

            Comments.Add(comment);
        }

        public string GetComment(string start)
        {
            return Comments.Find(x => x.StartsWith(start, StringComparison.CurrentCultureIgnoreCase))?.Replace(start, string.Empty).Trim() ?? string.Empty;
        }

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

        [CanBeNull]
        //public string GetProperty(string key)
        //{
        //    Properties.TryGetValue(key, out string value);
        //    return value;
        //}

        public string GetComment(int index)
        {
            return Comments.Count < index ? string.Empty : Comments[index];
        }

        public List<string> GetComments()
        {
            return Comments;
        }

        #endregion
    }
}
