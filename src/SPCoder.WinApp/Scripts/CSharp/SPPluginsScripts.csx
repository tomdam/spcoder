//Helper things for showing an object in gridview.

object objectForGrid = null;
public void DoShowObjectInGrid(object item)
{
    if (item == null) return;
    objectForGrid = item;
    main.SpGrid.ShowExpressionInGrid("objectForGrid");
    main.ShowGridWindow();
}

//execute plugin classes - mage them ready for usage
List<string> pluginFiles = new List<string>();
string pluginsDirectoryRoot = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"Scripts\CSharp\Plugins\");
pluginFiles.Add(pluginsDirectoryRoot + "SPSharePointListItemsGetter.csx");
pluginFiles.Add(pluginsDirectoryRoot + "SPExcelWorksheetDataGetter.csx");


foreach (string pluginFile in pluginFiles)
{
    //logger.LogInfo("Registering plugin " + pluginFile);
    main.FilesRegisteredForExecution.Add(pluginFile);
}

//listItemGetter.Execute(list);