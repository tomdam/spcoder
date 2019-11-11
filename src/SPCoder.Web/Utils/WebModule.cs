using SPCoder.Core.Utils;
using SPCoder.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.Web.Utils
{
    public class WebModule : ModuleDescription
    {
        public WebModule()
        { }
        private WebConnector _connector;
        public override BaseConnector Connector
        {
            get
            {
                if (_connector == null)
                    _connector = new WebConnector();
                return _connector;
            }
            set
            { }
        }

        public override BaseConnector GetConnector(string connectorType)
        {
            _connector = new WebConnector(connectorType);
            return _connector;
        }

        public override List<string> ConnectorTypes
        {
            get
            {
                return new List<string> { "Web page"};
            }
            set
            { }
        }
    }
}
