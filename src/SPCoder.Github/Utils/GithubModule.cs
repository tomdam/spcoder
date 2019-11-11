using SPCoder.Core.Utils;
using SPCoder.Utils;
using System.Collections.Generic;

namespace SPCoder.Github.Utils
{
    public class GithubModule : ModuleDescription
    {
        public GithubModule()
        { }

        private GithubConnector _connector;
        public override BaseConnector Connector
        {
            get
            {
                if (_connector == null)
                    _connector = new GithubConnector();
                return _connector;
            }
            set
            { }
        }

        public override BaseConnector GetConnector(string connectorType)
        {
            _connector = new GithubConnector(connectorType);
            return _connector;
        }

        public override List<string> ConnectorTypes
        {
            get
            {
                return new List<string> { "Github repo"};
            }
            set
            { }
        }
    }
}
