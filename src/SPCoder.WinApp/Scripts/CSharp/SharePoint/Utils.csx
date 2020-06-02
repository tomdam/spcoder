public class SPCoderField
{
    
    public string DisplayName { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public string Group { get; set; }
    public bool MultipleValues { get; set; }
    public string[] Values { get; set; }
    
    public string Guid {get; set;}
    
    public string FieldAsXML()
    {
        string FieldAsXML = @"<Field ID='{3}' Name='"+ this.Name + "' DisplayName='" + this.DisplayName + "' Type='{0}' {1} Required='False' Hidden='False' Group='"+ this.Group +"' Description='" + this.Description + "' >{2}</Field> ";
        //println(FieldAsXML);
        string gId = this.Guid ?? "{" + System.Guid.NewGuid().ToString().ToUpper() + "}";
        switch(this.Type)
        {
            case "Number":
                FieldAsXML = string.Format(FieldAsXML, "Number", "", "", gId);
            break;
            
            case "String":
                FieldAsXML = string.Format(FieldAsXML, "Text", "", "", gId);
            break;

            case "Note":
                FieldAsXML = string.Format(FieldAsXML, "Note", "NumLines='6' RichText='TRUE' Sortable='FALSE'", "", gId);
                break;            

            case "Date":
                FieldAsXML = string.Format(FieldAsXML, "DateTime", " Format='DateOnly' ", "", gId);
            break;
            case "Bool":
                FieldAsXML = string.Format(FieldAsXML, "Boolean", "", "", gId);
            break;
            case "Choice":
                //create choices and mappings inner xml based on possible values
                string innerXml = "";
                string myType   = "Choice";
                string format   = " Format='RadioButtons' ";
                if (this.MultipleValues)
                {
                    myType = "MultiChoice";
                    format = " ";
                }
              
                string choices  = "<CHOICES>";
                string mappings = "<MAPPINGS>";
                int i           = 0;
                foreach(string pos in this.Values)
                {
                    string val = pos.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
                    choices    +="<CHOICE>"+val+"</CHOICE>";
                    mappings    +="<MAPPING Value='"+ (++i) +"'>"+val+"</MAPPING>";
                }
                
                choices    += "</CHOICES>";
                mappings   += "</MAPPINGS>";
                innerXml   = choices + mappings;
                //println(innerXml);
                FieldAsXML = string.Format(FieldAsXML, myType, format, innerXml, gId);
            break;
        }
        return FieldAsXML;
    }
}


public List<Microsoft.SharePoint.Client.Field> CreateFields(List<SPCoderField> myFields, Web web, ClientContext context)
{
    List<Microsoft.SharePoint.Client.Field> newFields = new List<Microsoft.SharePoint.Client.Field> ();
    int cnt = 0;
    foreach(SPCoderField row in myFields)
    {
        if (row.DisplayName == null || string.IsNullOrEmpty(row.DisplayName.ToString()))
            continue;
        string FieldAsXML = row.FieldAsXML();
        
        cnt++;
        //println(FieldAsXML);
        var fld = web.Fields.AddFieldAsXml(FieldAsXML, true, AddFieldOptions.DefaultValue);
        //web.Context.Load(fields);
        web.Context.Load(fld);
        web.Context.ExecuteQuery();
        newFields.Add(fld);
    }
    return newFields;
}


public Microsoft.SharePoint.Client.List CreateListWithFields(List<SPCoderField> myFields, Web web, string listName, ClientContext context)
{
    //create the list
    ListCreationInformation listCreationInfo = new ListCreationInformation();
    listCreationInfo.Title                   = listName;

    listCreationInfo.TemplateType = 100; //Generic list

    List list = web.Lists.Add(listCreationInfo);
    list.ListExperienceOptions = Microsoft.SharePoint.Client.ListExperience.NewExperience;
    list.EnableVersioning      = true;
    list.Update();
    context.ExecuteQuery();      
    
    //add the fields to list
    List<Microsoft.SharePoint.Client.Field> newFields = new List<Microsoft.SharePoint.Client.Field> ();
    int cnt = 0;
    foreach(SPCoderField row in myFields)
    {
        if (row.DisplayName == null || string.IsNullOrEmpty(row.DisplayName.ToString()))
            continue;
        
        string FieldAsXML = row.FieldAsXML();
        cnt++;
        //println(FieldAsXML);
        var fld = list.Fields.AddFieldAsXml(FieldAsXML, true, AddFieldOptions.DefaultValue);
        
        context.Load(fld);
        newFields.Add(fld);
    }
    context.Load(list);
    //context.Load(list.Fields);
    context.Load(list.Fields, fields => fields.Include(
          field => field.Id,
          field => field.Title,
          field => field.InternalName
          ) );
    context.ExecuteQuery();
    
    return list;
}
