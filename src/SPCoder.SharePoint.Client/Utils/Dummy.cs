using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.SharePoint.Client.Utils
{
    /// <summary>
    /// Dummy class which uses some of the refereced assemblies so that those are later available using Assembly.GetReferencedAssemblies();
    /// This is a hack needed to overcome a bug in roslyn, which can raise error when comparing types.
    /// </summary>
    public class Dummy
    {
        public OfficeDevPnP.Core.WikiPageLayout wiki;
        public OfficeOpenXml.ExcelWorksheet worksheet;
        public Newtonsoft.Json.JsonConverter converter;
    }
}
