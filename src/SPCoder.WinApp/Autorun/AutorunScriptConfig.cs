using System;
using System.Collections.Generic;
using SPCoder.Config;

namespace SPCoder.Autorun
{
    /// <summary>
    /// The config class that is responsible for handling the xml configuration 
    /// for the autorun scripts.
    /// </summary>
    /// <author>Damjan Tomic</author>
    [Serializable]
    public class AutorunScriptConfig : BaseConfig 
    {
        public List<AutorunScriptConfigItem> AutoRunScripts;

        public AutorunScriptConfig()
        {
            AutoRunScripts = new List<AutorunScriptConfigItem>();
        }

        public AutorunScriptConfig(string name) : base(name)
        {
            AutoRunScripts = new List<AutorunScriptConfigItem>();
        }
    }
}