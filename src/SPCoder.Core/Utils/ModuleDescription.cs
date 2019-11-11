using SPCoder.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace SPCoder.Core.Utils
{
    [InheritedExport]
    public abstract class ModuleDescription
    {
        public abstract BaseConnector Connector { get; set; }

        public abstract BaseConnector GetConnector(string connectorType);

        /// <summary>
        /// This is a list of strings that will be shown in SPCoder'a combo box for choosing a connector.
        /// </summary>
        public abstract List<string> ConnectorTypes { get; set; }
    }
}
