using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using SPCoder.Core.Plugins;
using SPCoder.Core.Utils;
using SPCoder.PowerPlatform.Utils;


public class SPPowerAutomateFlowUpdater : BasePlugin
{
    public SPPowerAutomateFlowUpdater()
    {
        this.TargetType = typeof(Newtonsoft.Json.Linq.JObject);
        this.Name = "View client data";
    }

    public override void Execute(Object target)
    {
        Newtonsoft.Json.Linq.JObject jsonClientData = (Newtonsoft.Json.Linq.JObject)target;
        
        string formatedJson = JsonConvert.SerializeObject(System.Web.Helpers.Json.Decode(jsonClientData["clientdata"].ToString()), Formatting.Indented);
        byte[] jsonbytes = Encoding.UTF8.GetBytes(formatedJson);

        MemoryStream strm = new MemoryStream(jsonbytes);
        //strm.Write(bytes, 0, bytes.Length);        

        var fileDetails = new PPFlowJsonEditedFile
        {
            Filename = jsonClientData["name"].ToString() + ".json",
            Stream = strm,
            FlowJson = jsonClientData
        };

        Result = fileDetails;
        ExecuteCallback(fileDetails);
    }
}

public void GenerateNewSourceTabSPFile(object item)
{
    if (item is PPFlowJsonEditedFile)
    {
        var fileDetails = (PPFlowJsonEditedFile)item;
        main.GeneratedEditedFileTab(fileDetails);
    }
}

SPPowerAutomateFlowUpdater spEditFile = new SPPowerAutomateFlowUpdater();
spEditFile.Callback += GenerateNewSourceTabSPFile;
PluginContainer.Register(spEditFile);

logger.LogInfo("Registered plugin SPPowerAutomateFlowUpdater");