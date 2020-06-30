using SPCoder.Core.Plugins;

/// <summary>
/// SPCoder plugin that reads the items from SharePoint list and shows the data in SPCoder's viewer.
/// </summary>
public class SPSharePointListItemsGetter : BasePlugin
{
    public SPSharePointListItemsGetter()
    {
        this.TargetType = typeof(Microsoft.SharePoint.Client.List);
        this.Name = "Show items in grid";
    }

    public override void Execute(Object target)
    {
        //if (target is typeof(this.TargetType))            
        DataTable dt = this.GetItemsValues((Microsoft.SharePoint.Client.List)target);
        Result = dt;
        ExecuteCallback(dt);
    }
    
    public DataTable GetItemsValues(Microsoft.SharePoint.Client.List list)
    {

        Microsoft.SharePoint.Client.CamlQuery query = Microsoft.SharePoint.Client.CamlQuery.CreateAllItemsQuery();
        var items = list.GetItems(query);
        list.Context.Load(items);
        list.Context.ExecuteQuery();

        DataTable dt = new DataTable();
        if (items == null || items.Count == 0) return dt;
        var item = items[0];
        foreach (var key in item.FieldValues.Keys)
        {
            Type t = (item.FieldValues[key] != null) ? item.FieldValues[key].GetType() : null;
            if (t != null)
            {
                dt.Columns.Add(key, t);
            }
            else
            {
                dt.Columns.Add(key);
            }
        }

        foreach (var it in items)
        {
            DataRow row = dt.NewRow();
            foreach (var key in it.FieldValues.Keys)
            {
                if (it.FieldValues[key] != null)
                {
                    row[key] = it.FieldValues[key];
                }
            }
            dt.Rows.Add(row);
        }

        return dt;
    }
}

//registration code
SPSharePointListItemsGetter listItemGetter = new SPSharePointListItemsGetter();
listItemGetter.Callback += DoShowObjectInGrid;
PluginContainer.Register(listItemGetter);

logger.LogInfo("Registered plugin SPSharePointListItemsGetter");

//listItemGetter.Execute(list);