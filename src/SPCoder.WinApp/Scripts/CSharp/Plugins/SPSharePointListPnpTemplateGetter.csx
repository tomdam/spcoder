/// <summary>
/// SPCoder plugin that uses Pnp library to get the xml template from Web object.
/// After getting the xml, the plugin opens a new tab with xml template.
/// </summary>
public class SPSharePointListPnpTemplateGetter : BasePlugin
{
    public SPSharePointListPnpTemplateGetter()
    {
        this.TargetType = typeof(Microsoft.SharePoint.Client.List);
        this.Name = "Get PnP provisioning template xml for list";
    }

    public override void Execute(Object target)
    {
        String xmlSource = this.GetTemplate((Microsoft.SharePoint.Client.List)target);
        Result = xmlSource;
        ExecuteCallback(xmlSource);
    }

    public String GetTemplate(Microsoft.SharePoint.Client.List list)
    {
        var ctx = list.Context as Microsoft.SharePoint.Client.ClientContext;
        Microsoft.SharePoint.Client.Web web = ctx.Web;

        var ptci = new OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.ProvisioningTemplateCreationInformation(web);
        ptci.ListsToExtract.Add(list.Title);
        ptci.HandlersToProcess = OfficeDevPnP.Core.Framework.Provisioning.Model.Handlers.Lists;

        ptci.ProgressDelegate = delegate (String message, Int32 progress, Int32 total)
        {
            // Only to output progress for console UI
            Console.WriteLine("{0:00}/{1:00} - {2}", progress, total, message);
        };

        // Execute actual extraction of the template
        //var template = web.GetProvisioningTemplate(ptci);
        var template = Microsoft.SharePoint.Client.WebExtensions.GetProvisioningTemplate(web, ptci);

        return template.ToXML(null);
    }
}

//registration code
SPSharePointListPnpTemplateGetter listPnPTemplateGetter = new SPSharePointListPnpTemplateGetter();
listPnPTemplateGetter.Callback += GenerateNewSourceTab;
PluginContainer.Register(listPnPTemplateGetter);

logger.LogInfo("Registered plugin SPSharePointListPnpTemplateGetter");

//webPnPTemplateGetter.Execute(web);
