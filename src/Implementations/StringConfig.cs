using ModConfigMenu.Contracts;

namespace ModConfigMenu.Implementations
{
    public class StringConfig : BaseConfig
    {
        public StringConfig(string key, string value, string header) : base()
        {
            Key = key;
            Value = value;
            Header = header;
        }
    }
}