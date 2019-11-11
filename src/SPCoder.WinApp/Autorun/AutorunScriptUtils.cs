using System.Collections.Generic;
using System.Linq;
using SPCoder.Config;
using SPCoder.Utils;
using System;

namespace SPCoder.Autorun
{
    /// <summary>
    /// Util class for the autorun scripts.
    /// </summary>
    /// <author>Damjan Tomic</author>
    public class AutorunScriptUtils
    {
        /// <summary>
        /// Method that finds all autorun scripts, takes scripts' source code from files and returns the code as list.
        /// </summary>
        /// <returns>Source code from autorun script files; Every member of the return list is the code from one autroun script file</returns>
        public static List<string> GetAutorunScriptSources()
        {
            List<string> scriptsSourceCodes = new List<string>();
            AutorunScriptConfig autorunScriptConfig = GetAutorunConfig();

            //Sort the list on the "Order" field
            List<AutorunScriptConfigItem> autoRunScripts = (from scripts in autorunScriptConfig.AutoRunScripts
                                                            orderby scripts.Order
                                                            select scripts
                                    ).ToList();

            //List<AutorunScriptConfigItem> autoRunScripts = GetAutorunScripts();
            bool replaceWorkingDirectory = false;
            string replaceKey = null;
            string replaceValue = null;

            if (autorunScriptConfig.Properties != null)
            {
                replaceWorkingDirectory = "true".Equals(autorunScriptConfig.GetItemValueByName(SPCoderConstants.AUTORUN_REPLACE_WORKING_DIRECTORY));
                if (replaceWorkingDirectory)
                {
                    replaceKey = autorunScriptConfig.GetItemValueByName(SPCoderConstants.AUTORUN_WORKING_DIRECTORY_PLACEHOLDER);
                    replaceValue = Environment.CurrentDirectory;
                }
            }
            foreach (AutorunScriptConfigItem autoRunScript in autoRunScripts)
            {
                if (replaceWorkingDirectory)
                {
                    scriptsSourceCodes.Add(autoRunScript.GetSource(replaceKey, replaceValue));
                }
                else
                {
                    scriptsSourceCodes.Add(autoRunScript.Source);
                }
            }
            return scriptsSourceCodes;
        }

        /// <summary>
        /// Returns all the autorun scripts objects.
        /// </summary>
        /// <returns></returns>
        //public static List<AutorunScriptConfigItem> GetAutorunScripts()
        //{
        //    AutorunScriptConfig autorunScriptConfig = GetAutorunConfig();

        //    //Sort the list on the "Order" field
        //    List<AutorunScriptConfigItem> autoRunScripts = (from scripts in autorunScriptConfig.AutoRunScripts
        //                          orderby scripts.Order
        //                          select scripts
        //                            ).ToList();
        //    return autoRunScripts;
        //}

        /// <summary>
        /// Returns previously saved Autorun config.
        /// </summary>
        /// <returns></returns>
        public static AutorunScriptConfig GetAutorunConfig()
        {
            Config.Config config = ConfigUtils.GetActiveConfig();
            ConfigFile configFile = config.GetConfigFileByName("Autorun");
            string autorunFilePath = (configFile != null) ? configFile.Path : "Config\\autorun.xml";
            AutorunScriptConfig autorunScriptConfig = (AutorunScriptConfig)ConfigUtils.GetConfig(autorunFilePath, typeof(AutorunScriptConfig));
            return autorunScriptConfig;
        }

        /// <summary>
        /// Saves the config file to appropriate xml file on file system. 
        /// </summary>
        /// <param name="configToSave">The config that will be serialized</param>
        public static void SaveConfig(AutorunScriptConfig configToSave)
        {
            Config.Config config = ConfigUtils.GetActiveConfig();
            ConfigFile configFile = config.GetConfigFileByName("Autorun");
            string autorunFilePath = (configFile != null) ? configFile.Path : "Config\\autorun.xml";
            ConfigUtils.SaveConfig(configToSave, autorunFilePath );
        }

    }
}
