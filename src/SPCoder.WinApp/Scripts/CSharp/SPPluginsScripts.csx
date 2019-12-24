//Helper variable and method, used for showing an object in gridview.
object objectForGrid = null;
public void DoShowObjectInGrid(object item)
{
    if (item == null) return;
    objectForGrid = item;
    main.SpGrid.ShowExpressionInGrid("objectForGrid");
    main.ShowGridWindow();
}

//For new plugins to become available, they need to be saved as csx files inside the Plugins directory

//execute plugin classes - make them ready to use
string pluginsDirectoryRoot = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"Scripts\CSharp\Plugins\");
List<string> pluginFiles = new DirectoryInfo(pluginsDirectoryRoot)
                                .GetFiles("*.csx")
                                .Select(m => m.FullName)
                                .ToList();

foreach (string pluginFile in pluginFiles)
{
     main.FilesRegisteredForExecution.Add(pluginFile);
}