using SPCoder.Core.Utils;
using SPCoder.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SPCoder.PowerPlatform.Utils
{
    public class PPFlowJsonEditedFile : BaseEditedFile
    {
        public Newtonsoft.Json.Linq.JObject FlowJson { get; set; }

        public override bool Save(bool overwrite = false)
        {
            try
            {
                SaveBackupToDisk();
                string editedJson = new StreamReader(this.Stream).ReadToEnd();
                FlowJson["clientdata"] = editedJson;
                SPCoderLogger.Logger.LogInfo("Updated flow clientdata localy! Use \"Update cloud flow\" action to push the changes to server.");
                //just update the local object here. 
                //the real push to server will be done in another action 
            }
            catch (Exception ex)
            {
                SPCoderLogger.Logger.LogError(ex);
                throw;
            }

            return true;
        }

        private void SaveBackupToDisk()
        {
            try
            {
                //check if the backup folder exists
                string backupFolder = Path.Combine(Environment.CurrentDirectory, "Flows/Backup", FlowJson["workflowid"].ToString());

                DirectoryInfo di = new DirectoryInfo(backupFolder);
                if (!di.Exists)
                {
                    di.Create();                    
                }
                string fileName = Regex.Replace(DateTime.Now.ToString("o"), @"[+T:\.]", "") + ".json";
                File.WriteAllText(Path.Combine(di.FullName, fileName), FlowJson["clientdata"].ToString());
            }
            catch (Exception exc)
            {
                SPCoderLogger.Logger.LogError("Error while saving the backup of the flow localy." + exc.Message);
            }
        }
    }
}
