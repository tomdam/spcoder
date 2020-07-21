//This file contains the code for adding references to the SPCoder Roslyn context and usings
//SPCoder will replace {{WorkingDirectory}} with the full path to the folder where SPCoder.exe is located

//If you need to reference an assembly, you can do it with the #r directive

#r "System.Windows.Forms"
#r "System.Data"
#r "System.Web.Extensions"
/*
#r "{{WorkingDirectory}}\SPCoder.Core.dll"
#r "{{WorkingDirectory}}\OfficeDevPnP.Core.dll"
#r "{{WorkingDirectory}}\Camlex.Client.dll"
#r "{{WorkingDirectory}}\EPPlus.dll"

#r "Microsoft.SharePoint.WorkflowServicesBase"
*/

using System;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Security;
using System.Net;
using System.Data;
using System.Web.Script.Serialization;
using System.Text;

using SPCoder;
using SPCoder.Core.Utils;

//SharePoint CSOM 
using Microsoft.SharePoint.Client;

//EPPlus - excel library
//using OfficeOpenXml;


//for SharePoint SSOM, uncomment the next two lines
//using Microsoft.SharePoint;
//using Microsoft.SharePoint.Administration;
