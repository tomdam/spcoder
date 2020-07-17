using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.SharePoint.Client.Utils
{
    internal static class WebUtils
    {
        public static string MakeAbsoluteUrl(Web web, string serverRelativeUrl)
        {
            // e.g. 
            // Web: 
            // https://tenant.sharepoint.com/sites/foo/bar
            // serverRelative:
            // /sites/foo/bar/documents/sample.txt
            web.EnsureProperties(w => w.Url);

            Uri webUri = new Uri(web.Url);
            string tenantStub = $"{webUri.Scheme}://{webUri.Authority}";

            return $"{tenantStub}{serverRelativeUrl}";
        }
    }
}
