using OfficeDevPnP.Core.Pages;

// get a list of possible client side web parts that can be added
ClientSidePage p = new ClientSidePage(context);
var components = p.AvailableClientSideComponents();

var myWebPart = components.Where(s => s.ComponentType == 1 && s.Manifest.Contains("QuickChartWebPart")).FirstOrDefault();

CanvasSection cs = new CanvasSection(p, CanvasSectionTemplate.OneColumn, 5);
p.AddSection(cs);

ClientSideWebPart helloWp = new ClientSideWebPart(myWebPart) { Order = 10 };
helloWp.PropertiesJson = @"{'data':[{'id':'7CFFD4B0-436E-430D-94C5-A4F9D22DB3FE','label':'','value':'','valueNumber':0}],'type':1,'isInitialState':false,'dataSourceType':1,'listItemOrderBy':0,'selectedListId':'" + list.Id.ToString() + "','selectedLabelFieldName':'Title','selectedValueFieldName':'TotalCases','xAxisLabel':'','yAxisLabel':''}";
p.AddControl(helloWp, cs.Columns[0]);

//This will save the page to SitePages library
p.PageTitle = "Corona Stats page 2";
p.LayoutType = ClientSidePageLayoutType.Article;
p.Save("CoronaStats2.aspx");
//After the code executes, you can open your SharePoint site and look for the CoronaStats2.aspx in SitePages library

//ClientSidePage page1 = ClientSidePage.Load(context, "Stats.aspx");
//var components1      = page1.AvailableClientSideComponents();
//var li = page1.PageListItem;
//context.Load(li);
//context.ExecuteQuery();