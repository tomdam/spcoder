using System;
using System.Collections.Generic;

namespace SPCoder.Config
{
    /// <summary>
    /// The config class that is responsible for handling the xml configuration.
    /// This configuration contains the information about separate config files that describe parts
    /// of the application.    
    /// </summary>
    /// <author>Damjan Tomic</author>
    [Serializable]    
    public class SPCoderConfig : BaseConfig
    {
        public List<Config> ConfigSections;

        public SPCoderConfig()
        {
            ConfigSections = new List<Config>();
        }

        public SPCoderConfig(string name) : base(name)
        {
            ConfigSections = new List<Config>();
        }

        public Config GetConfigByName(string name)
        {
            Config item = ConfigSections.Find(
                delegate(Config it)
                {
                    return it.Name.Equals(name);
                });
            return item;
        }
    }
}
