using System;
using System.IO;
using System.Xml.Serialization;
using SPCoder.Config;

namespace SPCoder.Autorun
{
    /// <summary>
    /// POCO class that holds information about one autorun script.
    /// </summary>
    /// <author>Damjan Tomic</author>
    [Serializable]
    public class AutorunScriptConfigItem : BaseConfig
    {
        public override string ToString()
        {
            return Title;
        }

        #region Constructors
        public AutorunScriptConfigItem(string path, string name, int order) : base(name)
        {
            Path = path;
            Order = order;
        }

        public AutorunScriptConfigItem()
        {
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the Order of the Autorun Script in the list (and in which it is being executed)
        /// </summary>
        [XmlElement]
        public int Order
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or Sets the Path for the Autorun Script
        /// </summary>
        [XmlElement]
        public string Path
        {
            get;
            set;
        }

        ///<summary>
        /// Gets or Sets the Title for the Autorun Script.         
        ///</summary>
        [XmlElement]
        public string Title
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the content of the Autorun Script file.
        /// </summary>
        public string Source
        {
            get
            {
                string source;                                
                using (StreamReader reader = new StreamReader(ConfigUtils.GetFullPathToConfigFile(Path)))
                {
                    source = reader.ReadToEnd();
                }                
                return source;
            }
        }

        public string GetSource(string replaceKey, string replaceValue)
        {                        
            return this.Source.Replace(replaceKey, replaceValue);
        }
        #endregion
    }
}