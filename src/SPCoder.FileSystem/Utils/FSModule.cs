using SPCoder.Core.Utils;
using SPCoder.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.FileSystem.Utils
{
    public class FSModule : ModuleDescription
    {
        public FSModule()
        { }

        private FSConnector _connector;
        public override BaseConnector Connector
        {
            get
            {
                if (_connector == null)
                    _connector = new FSConnector();
                return _connector;
            }
            set
            { }
        }

        public override BaseConnector GetConnector(string connectorType)
        {
            _connector = new FSConnector(connectorType);
            return _connector;
        }

        public override List<string> ConnectorTypes
        {
            get
            {
                return new List<string> { "File system" };
            }
            set
            { }
        }
    }
}
