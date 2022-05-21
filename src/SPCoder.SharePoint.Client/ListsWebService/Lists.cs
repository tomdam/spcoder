using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.SharePoint.Client.ListsWebService
{
    public partial class Lists : System.Web.Services.Protocols.SoapHttpClientProtocol
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(uri);
            // here you can set the custom headers if it is necessary for FBA
            //request.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
            return request;
        }
    }
}
