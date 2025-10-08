using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using MGSC;
using ModConfigMenu.Services;
using ModConfigMenu.Contracts;
using System.Xml.Linq;

namespace ModConfigMenu.Objects
{
    /// <summary>
    /// Conserves a config value
    /// </summary>
    public class ConfigValue : BaseConfig
    {
        public ConfigValue() : base()
        {
            Key = string.Empty;
            Value = string.Empty;
            UnstoredValue = null;
            Properties = new List<MetaData>();
            Header = string.Empty;
        }

        public ConfigValue(string key, object value, List<MetaData> properties, string header) : base()
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
        public ConfigValue(string key, object value, string header) : base()
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
        public ConfigValue(string key, object value, string header, object defaultValue, string tooltip, string label) : base()
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
        public ConfigValue(string key, object value, string header, object defaultValue, string tooltip, string label, float min, float max) : base()
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
        public ConfigValue(string key, object value, string header, object defaultValue, string tooltip, string label, List<string> orderedDropdownOptions) : base()
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
        public ConfigValue(string key, object value, string header, object defaultValue, string tooltip, string label, List<string> orderedDropdownOptions, Type typeForce = null, float min = 1.0f, float max = 1.0f) : base()
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
    }
}