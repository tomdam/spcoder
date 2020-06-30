/// <summary>
/// SPCoder plugin that uses Pnp library to get the xml template from Web object.
/// After getting the xml, the plugin opens a new tab with xml template.
/// </summary>
public class SPSharePointWebPnpTemplateGetter : BasePlugin
{
    public SPSharePointWebPnpTemplateGetter()
    {
        this.TargetType = typeof(Microsoft.SharePoint.Client.Web);
        this.Name = "Get PnP provisioning template xml";
    }

    public override void Execute(Object target)
    {
        String xmlSource = this.GetTemplate((Microsoft.SharePoint.Client.Web)target);
        Result = xmlSource;
        ExecuteCallback(xmlSource);
    }

    public String GetTemplate(Microsoft.SharePoint.Client.Web web)
    {
        var ptci = new OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers.ProvisioningTemplateCreationInformation(web);

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
SPSharePointWebPnpTemplateGetter webPnPTemplateGetter = new SPSharePointWebPnpTemplateGetter();
webPnPTemplateGetter.Callback += GenerateNewSourceTab;
PluginContainer.Register(webPnPTemplateGetter);

logger.LogInfo("Registered plugin SPSharePointWebPnpTemplateGetter");

//webPnPTemplateGetter.Execute(web);
