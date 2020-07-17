using Microsoft.SharePoint.Client;
using File = Microsoft.SharePoint.Client.File;

public class SPFileDetails
{
    public string FileName { get; set; }
    public string FileContents { get; set; }
    public string FileExtension { get; set; }
}

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
        ctx.ExecuteQuery();

        string fileContent = string.Empty;

        using (StreamReader reader = new StreamReader(stream.Value))
        {
            fileContent = reader.ReadToEnd();
        }

        Console.WriteLine("Read file 2");

        var fileDetails = new SPFileDetails
        {
            FileName = file.Name,
            FileContents = fileContent,
            FileExtension = System.IO.Path.GetExtension(file.Name)
        };

        Result = fileDetails;
        ExecuteCallback(fileDetails);
    }
}

public void GenerateNewSourceTabSPFile(object item)
{
    if (item is SPFileDetails)
    {
        var fileDetails = (SPFileDetails)item;
        main.GenerateNewSourceTab(fileDetails.FileName, fileDetails.FileContents, null, fileDetails.FileExtension);
    }
}

SPSharePointFileEditor spEditFile = new SPSharePointFileEditor();
spEditFile.Callback += GenerateNewSourceTabSPFile;
PluginContainer.Register(spEditFile);

logger.LogInfo("Registered plugin SPSharePointFileEditor");