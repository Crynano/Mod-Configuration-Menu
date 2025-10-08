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
    public class DropdownConfig : BaseConfig
    {
        /// <summary>
        /// Constructor for dropdown configurations. Ensure defaultValue and orderedDropdownOptions are of the same type.
        /// </summary>
        /// <param name="key">The key which to identify the configuration.</param>
        /// <param name="value">The starting value of the configuration.</param>
        /// <param name="header">Category in UI where to place value under.</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="tooltip">Text that appears when value is hovered. Used to clarify config's functionality</param>
        /// <param name="label">Alternative name for the variable.</param>
        /// <param name="orderedDropdownOptions">List of objects to </param>
        public DropdownConfig(string key, object value, string header, object defaultValue, string tooltip, string label, List<object> orderedDropdownOptions)
        {
            this.Key = key;
            this.Value = value;
            this.Header = header;

            this.Properties = new List<MetaData>
            {
                new MetaData("default", defaultValue),
                new MetaData("tooltip", tooltip),
                new MetaData("label", label),
                new MetaData("dropdowns", orderedDropdownOptions)
            };
        }
    }
}