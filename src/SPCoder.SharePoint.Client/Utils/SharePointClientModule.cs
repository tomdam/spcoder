using SPCoder.Core.Utils;
using SPCoder.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.SharePoint.Client.Utils
{
    public class SharePointClientModule : ModuleDescription
    {
        public SharePointClientModule()
        { }
        private CSOMConnector _connector;
        public override BaseConnector Connector
        {
            get
            {
                if (_connector == null)
                    _connector = new CSOMConnector();
                return _connector;
            }
            set
            { }
        }

        public override BaseConnector GetConnector(string connectorType)
        {
            
            _connector = new CSOMConnector(connectorType);
            return _connector;
        }

        public override List<string> ConnectorTypes
        {
            get
            {
                return new List<string> {
                    "SharePoint Client WIN",
                    "SharePoint Client FBA",
                    "SharePoint Client O365",                    
                    "SharePoint Client O365 APP",
                    "SharePoint Client O365 MFA",};
            }
            set
            { }
        }
    }
}
