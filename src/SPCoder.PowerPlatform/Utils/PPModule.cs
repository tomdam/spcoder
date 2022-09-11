using SPCoder.Core.Utils;
using SPCoder.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.PowerPlatform.Utils
{
    public class PPModule : ModuleDescription
    {
        public PPModule()
        { }

        private PPConnector _connector;
        public override BaseConnector Connector
        {
            get
            {
                if (_connector == null)
                    _connector = new PPConnector();
                return _connector;
            }
            set
            { }
        }

        public override BaseConnector GetConnector(string connectorType)
        {
            _connector = new PPConnector(connectorType);
            return _connector;
        }

        public override List<string> ConnectorTypes
        {
            get
            {
                return new List<string> { "Power platform" };
            }
            set
            { }
        }
    }
}
