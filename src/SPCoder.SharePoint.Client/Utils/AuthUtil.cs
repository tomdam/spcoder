using Microsoft.SharePoint.Client;
using SPCoder.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.SharePoint.Client.Utils
{
    internal static class AuthUtil
    {
        public static ClientContext GetContext(string authenticationType, string siteUrl, string username, string password)
        {
            ClientContext context = null;

            if (authenticationType == SPCoderConstants.O365)
            {
                context = new ClientContext(siteUrl);
                SecureString pass = new SecureString();
                foreach (char c in password.ToCharArray()) pass.AppendChar(c);
                context.Credentials = new SharePointOnlineCredentials(username, pass);
            }
            else if (authenticationType == SPCoderConstants.O365_APP)
            {
                //Get the realm for the URL
                string realm = SPCoder.SharePoint.Client.TokenHelper.GetRealmFromTargetUrl(new Uri(siteUrl));
                string accessToken = SPCoder.SharePoint.Client.TokenHelper.GetAppOnlyAccessToken(SPCoder.SharePoint.Client.TokenHelper.SharePointPrincipal, new Uri(siteUrl).Authority, realm).AccessToken;
                context = SPCoder.SharePoint.Client.TokenHelper.GetClientContextWithAccessToken(siteUrl, accessToken);

            }
            else if (authenticationType == SPCoderConstants.FBA)
            {
                context = new ClientContext(siteUrl);
                context.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
                context.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(username, password);
            }
            else if (authenticationType == SPCoderConstants.WIN)
            {
                context = new ClientContext(siteUrl);
                context.AuthenticationMode = ClientAuthenticationMode.Default;
                context.Credentials = new NetworkCredential(username, password);
            }

            return context;
        }
    }
}
