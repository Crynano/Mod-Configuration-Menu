using ModConfigMenu.Contracts;
using System.Collections.Generic;

namespace ModConfigMenu.Implementations
{
    public class TextBoxConfig : BaseConfig
    {
        public TextBoxConfig(string key, string value, string header, string defaultValue, string tooltip, string label) : base()
        {
            Key = key;
            Value = value;
            Header = header;
            Properties = new List<MetaData>
            {
                new MetaData("default", defaultValue),
                new MetaData("label", label),
                new MetaData("tooltip", tooltip)
            };
        }
    }
}