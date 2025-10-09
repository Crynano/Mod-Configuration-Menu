using ModConfigMenu.Contracts;
using System.Collections.Generic;

namespace ModConfigMenu.Implementations
{
    /// <summary>
    /// Creates a RangeConfig with a forced type. Beware, the generic type must only be numerical or it will error.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RangeConfig<T> : BaseConfig
    {
        public RangeConfig(string key, T value, T defaultValue, T min, T max, string header, string tooltip, string label) : base()
        {
            // Forgive me if I have done this wrong. .NET 7 seems to have a feature but we're in Framework ;_;
            bool typeCheck = typeof(T).Equals(typeof(int)) || typeof(T).Equals(typeof(uint)) ||
                typeof(T).Equals(typeof(double)) || typeof(T).Equals(typeof(float)) || typeof(T).Equals(typeof(decimal));
            if (!typeCheck)
            {
                Logger.LogWarning($"RangeConfig does not allow non-numerical types. Please revise your configuration for the {key} property.");
            }

            this.Key = key;
            this.Value = value;
            this.Header = header;
            this.Properties = new List<MetaData>
            {
                new MetaData("default", defaultValue),
                new MetaData("label", label),
                new MetaData("tooltip", tooltip),
                new MetaData("min", min),
                new MetaData("max", max)
            };
        }
    }
}