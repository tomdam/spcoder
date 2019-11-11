using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SPCoder.Config;
using SPCoder.Utils;

namespace SPCoder.Describer
{
    public class DescriberUtils
    {
//        /// <summary>
//        /// Method that finds all autorun scripts, takes scripts' source code from files and returns the code as list.
//        /// </summary>
//        /// <returns>Source code from autorun script files; Every member of the return list is the code from one autroun script file</returns>
//        public static List<string> GetAutorunScriptSources()
//        {
//            List<string> scriptsSourceCodes = new List<string>();
//            List<AutorunScriptConfigItem> autoRunScripts = GetAutorunScripts();
//
//            foreach (AutorunScriptConfigItem autoRunScript in autoRunScripts)
//            {
//                scriptsSourceCodes.Add(autoRunScript.Source);
//            }
//            return scriptsSourceCodes;
//        }

//        /// <summary>
//        /// Returns all the autorun scripts objects.
//        /// </summary>
//        /// <returns></returns>
//        public static List<AutorunScriptConfigItem> GetAutorunScripts()
//        {
//            Config.Config config = ConfigUtils.GetActiveConfig();
//            ConfigFile configFile = config.GetConfigFileByName("Autorun");
//            string autorunFilePath = (configFile != null) ? configFile.Path : "Config\\autorun.xml";
//            AutorunScriptConfig autorunScriptConfig = (AutorunScriptConfig)ConfigUtils.GetConfig(autorunFilePath, typeof(AutorunScriptConfig));
//
//            //Sort the list on the "Order" field
//            List<AutorunScriptConfigItem> autoRunScripts = (from scripts in autorunScriptConfig.AutoRunScripts
//                                                            orderby scripts.Order
//                                                            select scripts
//                                    ).ToList();
//            return autoRunScripts;
//        }

        public static DescriberPropertiesData GetValuesFromConfig()
        {
            Config.Config config = ConfigUtils.GetActiveConfig();
            ConfigFile configFile = config.GetConfigFileByName("Describer");
            string filePath = (configFile != null) ? configFile.Path : "Config\\describer.xml";
            BaseConfig baseConfig = (BaseConfig) ConfigUtils.GetConfig(filePath, typeof (BaseConfig));
            //return baseConfig;
            DescriberPropertiesData describerPropertiesData = new DescriberPropertiesData();
            string value = baseConfig.GetItemValueByName(SPCoderConstants.DESCRIBER_EDITABLE);
            describerPropertiesData.IsEditable = Boolean.Parse(value);

            value = baseConfig.GetItemValueByName(SPCoderConstants.DESCRIBER_WORDWRAP);
            describerPropertiesData.WordWrap = Boolean.Parse(value);

            value = baseConfig.GetItemValueByName(SPCoderConstants.DESCRIBER_LINKFORMAT);
            describerPropertiesData.MsdnLinkFormat = value;

            value = baseConfig.GetItemValueByName(SPCoderConstants.DESCRIBER_MAXCHARACTERS);
            describerPropertiesData.MaxDisplayedSize = Convert.ToInt32(value);

            return describerPropertiesData;
        }
    }
}
