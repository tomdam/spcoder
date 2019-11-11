using System;
using System.Xml.Serialization;

namespace SPCoder.Config
{
    /// <summary>
    /// POCO class that holds information about one config file. I.E. "autorun scripts" config,
    /// or "describer" config.
    /// </summary>
    /// <author>Damjan Tomic</author>
    [Serializable]
    public class ConfigFile
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string Path { get; set; }
        
        public ConfigFile()
        {
        }

        public ConfigFile(string path, string name)
        {
            Path = path;
            Name = name;
        }
    }
}
