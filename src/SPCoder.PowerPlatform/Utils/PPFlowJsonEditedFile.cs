using SPCoder.Core.Utils;
using SPCoder.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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
    }
}
