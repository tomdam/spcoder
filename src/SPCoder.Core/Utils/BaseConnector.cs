using SPCoder.Utils.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;


namespace SPCoder.Utils
{    
    public abstract class BaseConnector
    {
        public abstract BaseNode GetSPStructure(string siteUrl);

        public virtual BaseNode GenerateRootNode()
        {
            return null;
        }

        public virtual BaseNode ExpandNode(BaseNode node, bool doIfLoaded = false)
        {
            //
            return node;
        }

        public abstract string ImagesPath { get; set; }

        public abstract bool IsImagesPathLocal { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Endpoint { get; set; }

        /// <summary>
        /// In Sharepoint CSOM connector this can have one of the following values:
        /// O365 for Office 365
        /// FBA for Forms Based Auth
        /// WIN for Window Based Auth
        /// </summary>
        public string AuthenticationType { get; set; }




        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectorType">A value from module's ConnectorTypes list</param>
        public BaseConnector(string connectorType)
        { }

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public BaseConnector()
        { }


        public virtual List<object> AutoAddToContext()
        {
            return null;
        }

    }
}
