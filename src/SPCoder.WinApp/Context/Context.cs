using System.Collections.Generic;
using System;

namespace SPCoder.Context
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Damjan Tomic</author>
    public abstract class Context
    {
        private ContextItemNameGenerator nameGenerator;
        private IDictionary<string, ContextItem> context;

        public object this[string index] {
            get {
                ContextItem value = null;
                if (context.TryGetValue(index, out value))
                {
                    return value.Data;
                }
                else
                {
                    return null;
                }
            }
        }

        public abstract object GetVariable(string name);
        
        public IDictionary<string, ContextItem> GetContext 
        {
            get { return context; }
        }
        public virtual void AddItem(ContextItem item)
        {
            if (item.Name == null)
            {
                item.Name = GenerateObjectName(item);
            }
            
            context.Add(item.Name, item);
        }

        public virtual void AddOrUpdateItem(ContextItem item)
        {
            if (item.Name == null)
                item.Name = GenerateObjectName(item);

            ContextItem value = null;
            if (!context.TryGetValue(item.Name, out value))
            {
                context.Add(item.Name, item);
            }
        }

        

        public virtual void UpdateItem(ContextItem item)
        {
            if (item == null || item.Name == null)
                throw new Exception("Item does not exist");

            ContextItem value = null;
            if (!context.TryGetValue(item.Name, out value))
            {
                throw new Exception("Item does not exist");
            }
        }

        public string GenerateObjectName(ContextItem item)
        {
            return nameGenerator.GenerateName(item);
        }

        public virtual void  RemoveItem(ContextItem item)
        {
            context.Remove(item.Name);
            
        }
        
        public Context()
        {
            context = new Dictionary<string, ContextItem>();
            nameGenerator = new ContextItemNameGenerator(this);
        }

        /// <summary>
        /// Renames the item in the context.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="newName"></param>
        public virtual void RenameItem(ContextItem item, string newName)
        {
            
        }
    }
}