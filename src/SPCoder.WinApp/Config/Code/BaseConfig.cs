using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SPCoder.Config
{
    /// <summary>    
    /// This is the base config class. Other classes that are serialized to/from xml inherit from this class.
    /// This class holds the list of name/value pairs.
    /// </summary>
    /// <author>Damjan Tomic</author>
    [Serializable]
    public class BaseConfig
    {
        [XmlAttribute]
        public string Name { get; set; }
        public List<ConfigProperty> Properties;

        public BaseConfig()
        {
            Properties = new List<ConfigProperty>();
        }

        public BaseConfig(string name) : this()
        {
            Name = name;
        }

        public string GetItemValueByName(string name)
        {
            ConfigProperty property = Properties.Find(
                delegate(ConfigProperty it)
                {
                    return it.Name.Equals(name);
                });
            return (property == null) ? null : property.Value;
        }
    }
}