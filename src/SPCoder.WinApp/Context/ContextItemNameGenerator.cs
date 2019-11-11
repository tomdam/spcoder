using System.Collections.Generic;
using System.Linq;

namespace SPCoder.Context
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Damjan Tomic</author>
    public class ContextItemNameGenerator
    {
        #region Private fields
        private SPCoder.Context.Context context;
        private readonly IDictionary<string, string> typeNameMappings = new Dictionary<string, string>();
        #endregion

        #region Constructors
        public ContextItemNameGenerator(SPCoder.Context.Context context)
            : this()
        {
            this.context = context;
        }

        public ContextItemNameGenerator()
        {
            typeNameMappings.Add("SPList", "list");
            typeNameMappings.Add("SPWeb", "web");
            typeNameMappings.Add("SPSite", "site");
            typeNameMappings.Add("SPDocumentLibrary", "list");

            typeNameMappings.Add("ClientContext", "context");
        } 
        #endregion

        public string GenerateName(ContextItem item)
        {
            string type = item.Type;
            if (type.Contains("."))
            {
                string[] typeParts = type.Split('.');
                if (typeParts.Length > 0)
                    type = typeParts[typeParts.Length - 1];
            }
            string name = null;
            string prefferefName = type.ToLower();
            if (typeNameMappings.ContainsKey(type))
            {
                prefferefName = typeNameMappings[type];
            }
            if (!context.GetContext.ContainsKey(prefferefName))
            {
                name = prefferefName;
            }
            else
            {
                ContextItem[] myResult = (from i in context.GetContext
                                          where i.Key.StartsWith(prefferefName)
                                          orderby i.Key.Length, i.Key
                                          select i.Value
                                         ).ToArray();
                if (myResult.Length > 0)
                {
                    string lastObjectName = myResult[myResult.Length - 1].Name;
                    string lastObjectNumber = lastObjectName.Substring(prefferefName.Length);
                    int lastNumber;
                    int.TryParse(lastObjectNumber, out lastNumber);
                    lastNumber++;
                    name = prefferefName + lastNumber;
                }
            }
            return name;
        }                        
    }
}