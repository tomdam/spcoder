//Get the data using "Web page" connector
main.Connect("https://www.worldometers.info/coronavirus/#countries", "Web page"); 

//Get the table of latest infromation about the corona virus in the different countries
//------------

//here we use the HtmlAgilityPack project (https://html-agility-pack.net/) to scrape the page and get the html table 
//element that contains the data

var htmlnode = htmldocument.DocumentNode;
var htmltable = htmlnode.SelectSingleNode("//table[@id='main_table_countries_today']");
var thnodes = htmltable.SelectSingleNode("thead").SelectNodes(".//th");

var tbodyNode = htmltable.SelectSingleNode("(tbody)[1]");
var nodes     = tbodyNode.SelectNodes("tr[not(contains(@class,'row_continent'))]");

//create the DataTable object
var table = new DataTable("Corona");

var headers = thnodes.Select(th => th.InnerText.Trim().Replace("\n","").Replace(","," ").Replace("&nbsp;"," ").Replace("/"," per ")).ToList();

//create the columns in DataTable
foreach (var header in headers)
{
    table.Columns.Add(header);
}

//get the rows from html table and clean some of the values
var rows = nodes.Skip(1).Select(tr => tr
    .Elements("td")
    .Select(td => td.InnerText.Trim().Replace(",","").Replace("N/A","").Replace("+",""))
    //.Select(td => td.InnerText.Trim())
    .ToArray());

//add the rows to the DataTable    
foreach (var row in rows)
{
    table.Rows.Add(row);
}

//at this point, you can use GridViewer to check the table variable

//------------
//connect to your SharePoint online site
//instead of connecting from code, you can also choose "SharePoint Client O365" connector in Explorer view and 
//enter authentication details there directly
string myUsername = main.Decrypt("ENCRYPTEDUSERNAME");
string myPassword = main.Decrypt("ENCRYPTEDPASSWORD");

main.Connect("https://MYtenant.sharepoint.com/sites/SPCodertest/", "SharePoint Client O365", myUsername, myPassword);


//prepare the code for creating lists (you can also open the Utils.csx file in SPCoder and execute it instead)
execFile("Scripts\\CSharp\\SharePoint\\Utils.csx");
   
//create the list
List<SPCoderField> myFields = new List<SPCoderField> (); 

for(int i = 0; i < headers.Count; i++)
{
    var header       = headers[i];
    string fieldType = "Number";
    if (header == "Country Other" || header == "Continent")  fieldType = "String";
    if (header =="#") continue;
    myFields.Add(new SPCoderField {Name = header.Replace(" ",""), DisplayName = header.Replace(" ",""), Type = fieldType, Group = "Corona", Values = null});
}    

var list = CreateListWithFields(myFields, web, "Corona", context);

//------------
//add the data to the list
int cnt                 = 0;
//System.Data.DataRow row = table.Rows[0];
foreach(System.Data.DataRow row in table.Rows)
{
    ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
    ListItem oListItem                         = list.AddItem(itemCreateInfo); 

    oListItem["Title"] = row["Country Other"].ToString();
    
    for(int i = 0; i < headers.Count; i++)
    {
        var header       = headers[i];
        if (!String.IsNullOrEmpty(row[header].ToString()))
        {
            if (header =="#") continue;
            string internalName = list.Fields.Where(m => m.Title == header.Replace(" ","")).FirstOrDefault().InternalName;
            if (header != "Country Other" && header != "Continent")
            {
                double num = Double.Parse(row[header].ToString());
                oListItem[internalName] = num; 
            }
            else
            {
                oListItem[internalName] = row[header].ToString(); 
            }
        }
    }
    oListItem.Update();
    if (++cnt % 50 == 0) context.ExecuteQuery();
}    

context.ExecuteQuery();
//------------
//For the rest of the example please check the "Corona save page.csx" file
main.GenerateNewSourceTabsFromPath("Scripts\\CSharp\\Examples\\Corona\\Corona save page.csx");