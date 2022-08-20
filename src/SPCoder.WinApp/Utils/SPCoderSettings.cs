using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace SPCoder.Utils
{
    public class SPCoderSettings
    {
        public static Dictionary<string, object> Settings {
            get {
                return _settings;
            }
        }

        private static Dictionary<string, object> _settings;
        private static string settingsFilePath;
        public static void ReadSettings()
        {
            settingsFilePath = ConfigurationManager.AppSettings["SettingsFilePath"];
            if (!System.IO.Path.IsPathRooted(settingsFilePath))
            {
                settingsFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), settingsFilePath);
            }

            if (!System.IO.File.Exists(settingsFilePath))
            {
                SPCoderForm.MainForm.LogError("Settings file" + settingsFilePath + " does not exist");
                _settings = new Dictionary<string, object>();
            }
            else
            {
                string content = System.IO.File.ReadAllText(settingsFilePath);
                JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                jsSerializer.MaxJsonLength = int.MaxValue;
                _settings = jsSerializer.Deserialize<Dictionary<string, object>>(content);
            }
        }

        public static void SaveSettings()
        {
            try
            {
                if (!string.IsNullOrEmpty(settingsFilePath) && _settings != null)
                {
                    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                    jsSerializer.MaxJsonLength = int.MaxValue;
                    System.IO.File.WriteAllText(settingsFilePath, jsSerializer.Serialize(_settings));
                }
            }
            catch (Exception exc)
            {
                SPCoderForm.MainForm.LogError("Error while saving the settings file" + settingsFilePath + " " + exc.Message);
            }
        }

        public static Language GetLanguageFromExtension(string ext)
        {
            var r = (Dictionary<string, object>)_settings[SPCoderConstants.SP_SETTINGS_CODE];
            var l = (Dictionary<string, object>)r[SPCoderConstants.SP_SETTINGS_LANGUAGES];

            Language lang = Language.CSharp;

            foreach (var k in l.Keys)
            {
                var langs = l[k];
                var llangs = (System.Collections.ArrayList)langs;
                if (llangs != null)
                {
                    foreach (string lang1 in llangs)
                    {
                        if (lang1.ToLower() == ext.ToLower())
                        {
                            return GetLanguageByExtension(k);
                        }
                    }
                }
            }
            return lang;            
        }

        public static Language GetLanguageByExtension(string ext)
        {
            foreach (Language l in (Language[])Enum.GetValues(typeof(Language)))
            {
                if (l.ToString().ToUpper() == ext.ToUpper())
                {
                    return l;
                }
            }
            return Language.Custom;
        }

        public static void AddPathToHistory(string filepath)
        {
            try
            {
                var codeSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_CODE];
                var history = (System.Collections.ArrayList)codeSettings[SPCoderConstants.SP_SETTINGS_HISTORY];
                bool found = false;
                foreach (Dictionary<string, object> h in history)
                {
                    string path = h[SPCoderConstants.SP_SETTINGS_PATH].ToString();
                    if (path == filepath)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    Dictionary<string, object> h = new Dictionary<string, object>();
                    h[SPCoderConstants.SP_SETTINGS_PATH] = filepath;
                    history.Add(h);
                }
            }
            catch (Exception exc)
            {
                SPCoderForm.MainForm.LogError("Error while adding path to history " + filepath + " " + exc.Message);
            }
        }

        public static void RemovePathFromHistory(string filepath)
        {
            try
            {
                var codeSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_CODE];
                var history = (System.Collections.ArrayList)codeSettings[SPCoderConstants.SP_SETTINGS_HISTORY];
                
                Dictionary<string, object> itemToRemove = null;
                foreach (Dictionary<string, object> h in history)
                {
                    string path = h[SPCoderConstants.SP_SETTINGS_PATH].ToString();
                    if (path == filepath)
                    {
                        itemToRemove = h;                        
                        break;
                    }
                }
                if (itemToRemove != null)
                {
                    history.Remove(itemToRemove);
                }
            }
            catch (Exception exc)
            {
                SPCoderForm.MainForm.LogError("Error while removing path from history " + filepath + " " + exc.Message);
            }
        }

        public static void ClearOpenedWindows()
        {
            var codeSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_CODE];
            var windows = (System.Collections.ArrayList)codeSettings[SPCoderConstants.SP_SETTINGS_WINDOWS];
            windows.Clear();
        }

        public static void AddPathToOpenedWindows(string filepath)
        {
            var codeSettings = (Dictionary<string, object>)SPCoderSettings.Settings[SPCoderConstants.SP_SETTINGS_CODE];
            var windows = (System.Collections.ArrayList)codeSettings[SPCoderConstants.SP_SETTINGS_WINDOWS];
            bool found = false;
            foreach (Dictionary<string, object> w in windows)
            {
                string path = w[SPCoderConstants.SP_SETTINGS_PATH].ToString();
                if (path == filepath)
                {
                    found = true;
                }
            }

            if (!found)
            {
                Dictionary<string, object> w = new Dictionary<string, object>();
                w[SPCoderConstants.SP_SETTINGS_PATH] = filepath;
                windows.Add(w);
            }
        }
    }
}
