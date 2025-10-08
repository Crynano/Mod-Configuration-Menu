using System;
using System.Collections.Generic;

namespace ModConfigMenu.Contracts
{
    public interface IConfigValue
    {
        /// <summary>
        /// The name of the property
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// Value stored with the type set.
        /// </summary>
        object Value { get; set; }

        /// <summary>
        /// The value that needs to be saved to Value property.
        /// Can be discarded or applied from outside.
        /// </summary>
        object UnstoredValue { get; set; }

        /// <summary>
        /// Storage for all the comments converted to properties, which facilitates the search for data.
        /// Currently used properties are: min, max, default, type, tooltip, label.
        /// </summary>
        List<MetaData> Properties { get; set; }

        /// <summary>
        /// What category this data belongs to.
        /// Headers are defined between square brackets. []
        /// </summary>
        string Header { get; set; }

        Action OnValueChanged { get; set; }

        void Save();
        void ResetDefault();
        void ClearUnstored();
        void SetUnstoredValue<T>(T value);
        object GetDefault();
        float GetMax();
        float GetMin();
        string GetTypeProp();
        List<object> GetDropdownOptions();
        string GetLabel();
        string GetTooltip();
        void AddProperty(string untrimmedLine);
        void PrintDebug();
    }
}
