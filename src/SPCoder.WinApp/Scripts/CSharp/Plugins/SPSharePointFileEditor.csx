using Microsoft.SharePoint.Client;
using File = Microsoft.SharePoint.Client.File;
using SPCoder.Core.Plugins;
using SPCoder.Core.Utils;
using SPCoder.SharePoint.Client.Utils;


public class SPSharePointFileEditor : BasePlugin
{
    public SPSharePointFileEditor()
    {
        this.TargetType = typeof(Microsoft.SharePoint.Client.File);
        this.Name = "Edit File";
    }

    public override void Execute(Object target)
    {
        File file = (File)target;
        var ctx = file.Context as ClientContext;
        var stream = file.OpenBinaryStream();

        ctx.Load(file);
        ctx.Load(file.ListItemAllFields);
        ctx.ExecuteQuery();

        var folderUrl = file.ListItemAllFields["FileDirRef"].ToString();
        var parentFolder = file.ListItemAllFields.ParentList.ParentWeb.GetFolderByServerRelativeUrl(folderUrl);
        ctx.Load(parentFolder);
        ctx.ExecuteQuery();

        var fileDetails = new SharePointEditedFile
        {
            Filename = file.Name,
            Stream = stream.Value,
            ParentContainer = parentFolder,
            FullFilePath = file.ServerRelativeUrl
        };

        Result = fileDetails;
        ExecuteCallback(fileDetails);
    }
}

public void GenerateNewSourceTabSPFile(object item)
{
    if (item is SharePointEditedFile)
    {
        var fileDetails = (SharePointEditedFile)item;
        main.GeneratedEditedFileTab(fileDetails);
    }
}

SPSharePointFileEditor spEditFile = new SPSharePointFileEditor();
spEditFile.Callback += GenerateNewSourceTabSPFile;
PluginContainer.Register(spEditFile);

logger.LogInfo("Registered plugin SPSharePointFileEditor");