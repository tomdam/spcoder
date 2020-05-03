//This file contains the code for adding references to the SPCoder Roslyn context and usings
//SPCoder will replace {{WorkingDirectory}} with the full path to the folder where SPCoder.exe is located

#r "System.Windows.Forms"
#r "System.Data"
#r "System.Web.Extensions"
/*
#r "{{WorkingDirectory}}\SPCoder.Core.dll"
#r "{{WorkingDirectory}}\OfficeDevPnP.Core.dll"
#r "{{WorkingDirectory}}\Camlex.Client.dll"
#r "{{WorkingDirectory}}\EPPlus.dll"
#r "{{WorkingDirectory}}\SPCoder.2016.exe"
*/

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
