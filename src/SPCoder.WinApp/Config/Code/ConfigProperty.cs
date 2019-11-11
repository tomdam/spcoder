using System;
using System.Xml.Serialization;

namespace SPCoder.Config
{
    /// <summary>    
    /// This class holds the name/value values for storing in xml configuration files.
    /// </summary>
    /// <author>Damjan Tomic</author>
    [Serializable]
    public class ConfigProperty
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Value { get; set; }

        public ConfigProperty()
        {
        }

        public ConfigProperty(string key, string value)
        {
            Name = key;
            Value = value;
        }
    }
}
