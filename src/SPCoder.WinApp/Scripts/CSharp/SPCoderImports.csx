/*
try
{   
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.Administration;
}
catch(Exception exc)
        {
            Console.WriteLine(exc);
        }
*/
#r "System.Windows.Forms"
//#r "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7\System.Windows.Forms.dll"
#r "{{WorkingDirectory}}\SPCoder.Core.dll"
//#r "{{WorkingDirectory}}\OfficeDevPnP.Core.dll"
//#r "{{WorkingDirectory}}\Camlex.Client.dll"
#r "{{WorkingDirectory}}\SPCoder.2016.exe"
//#r "{{WorkingDirectory}}\Microsoft.Office.Client.Policy.dll"
//#r "{{WorkingDirectory}}\Microsoft.SharePoint.Client.WorkflowServices.dll"
//#r "Microsoft.SharePoint.Client"
//#r "Microsoft.SharePoint.Client.Runtime"
//#r "C:\Projects\POC\EPPlus\EPPlus\bin\Debug\EPPlus.dll"

//#r "C:\Windows\Microsoft.Net\assembly\GAC_MSIL\Microsoft.SharePoint.Client.Taxonomy\v4.0_15.0.0.0__71e9bce111e9429c\Microsoft.SharePoint.Client.Taxonomy.dll"

#r "{{WorkingDirectory}}\SPCoder.SharePoint.Client.dll"
#r "{{WorkingDirectory}}\SPCoder.FileSystem.dll"
#r "{{WorkingDirectory}}\SPCoder.Web.dll"
#r "{{WorkingDirectory}}\SPCoder.Github.dll"

using System;
using System.Windows.Forms;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Security;
using System.Net;
using System.Data;
using System.Web.Script.Serialization;

using SPCoder;
using SPCoder.Core.Utils;

using Microsoft.SharePoint.Client;

using System.IO;
//using OfficeOpenXml;

using System.Text;

//using Microsoft.SharePoint.Client.Taxonomy;


