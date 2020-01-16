//This file contains the code for adding references to the SPCoder Roslyn context and usings
//SPCoder will replace {{WorkingDirectory}} with the full path to the folder where SPCoder.exe is located

#r "System.Windows.Forms"
#r "{{WorkingDirectory}}\SPCoder.Core.dll"
#r "{{WorkingDirectory}}\OfficeDevPnP.Core.dll"
#r "{{WorkingDirectory}}\Camlex.Client.dll"
#r "{{WorkingDirectory}}\EPPlus.dll"
#r "{{WorkingDirectory}}\SPCoder.2016.exe"
//#r "{{WorkingDirectory}}\Microsoft.Office.Client.Policy.dll"
//#r "{{WorkingDirectory}}\Microsoft.SharePoint.Client.WorkflowServices.dll"
//#r "Microsoft.SharePoint.Client"
//#r "Microsoft.SharePoint.Client.Runtime"
//#r "C:\Windows\Microsoft.Net\assembly\GAC_MSIL\Microsoft.SharePoint.Client.Taxonomy\v4.0_15.0.0.0__71e9bce111e9429c\Microsoft.SharePoint.Client.Taxonomy.dll"

#r "{{WorkingDirectory}}\SPCoder.SharePoint.Client.dll"
#r "{{WorkingDirectory}}\SPCoder.FileSystem.dll"
#r "{{WorkingDirectory}}\SPCoder.Web.dll"
#r "{{WorkingDirectory}}\SPCoder.Github.dll"

//for SharePoint SSOM, uncomment the next line and change the path to Microsoft.SharePoint.dll as appropriate
//15 - SP 2013
//16 - SP 2016
//#r "C:\Program Files\Common Files\Microsoft Shared\Web Server Extensions\15\ISAPI\Microsoft.SharePoint.dll"

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
//EPPlus - excel library
using OfficeOpenXml;

using System.Text;

//for SharePoint SSOM, uncomment the next two lines
//using Microsoft.SharePoint;
//using Microsoft.SharePoint.Administration;
