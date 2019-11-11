using System;
using System.Collections.Generic;

namespace SPCoder.Config
{
    /// <summary>
    /// The config class that is responsible for handling the xml configuration for one type of the project.
    /// Different scripting languages will have their own objects of this class. 
    /// </summary>
    /// <author>Damjan Tomic</author>
    [Serializable]    
    public class Config : BaseConfig
    {                
        public List<ConfigFile> ConfigFiles;

        public ConfigFile GetConfigFileByName(string name)
        {
            ConfigFile item = ConfigFiles.Find(
                delegate(ConfigFile it)
                {
                    return it.Name.Equals(name);
                });
            return item;
        }
    }
}