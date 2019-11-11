using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using SPCoder.Utils;

namespace SPCoder.Config
{
    /// <summary>
    /// Util class for all the Config objects. This class has methods for reading/writing of the config files.
    /// </summary>
    /// <author>Damjan Tomic</author>
    public class ConfigUtils
    {        
        public static string ApplicationDirectoryPath = null;

        /// <summary>
        /// Returns the full directory path to current application directory.
        /// </summary>
        public static string FullDirectoryPath
        {
            get
            {
                if (string.IsNullOrEmpty(ApplicationDirectoryPath))
                    ApplicationDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return ApplicationDirectoryPath;// +"\\" + ConfigPath;
            }
        }

        public static string GetFullPathToConfigFile(string path)
        {
            if (Path.IsPathRooted(path))
                return path;

            return FullDirectoryPath + "\\" + path;
        }

        public static object GetConfig(string path, Type type)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = ConfigurationManager.AppSettings["ConfigPath"];
            }
            object config = SPCoderUtils.Deserialize(type, GetFullPathToConfigFile(path));
            return config;
        }

        public static void SaveConfig(object config, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = ConfigurationManager.AppSettings["ConfigPath"];
            }
            SPCoderUtils.Serialize(config, GetFullPathToConfigFile(path));
        }

        public static Config GetActiveConfig()
        {
            SPCoderConfig spCoderConfig = (SPCoderConfig) GetConfig(null, typeof (SPCoderConfig));
            string activeConfigName = spCoderConfig.GetItemValueByName(SPCoderConstants.ACTIVE_CONFIG);
            Config activeConfig = spCoderConfig.GetConfigByName(activeConfigName);
            return activeConfig;
        }

        public static SPCoderConfig GetRootConfig()
        {
            SPCoderConfig spCoderConfig = (SPCoderConfig)GetConfig(null, typeof(SPCoderConfig));
            return spCoderConfig;
        }
    }
}