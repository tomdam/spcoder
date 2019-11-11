using Microsoft.Scripting.Hosting;

namespace SPCoder.Context
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Damjan Tomic</author>
    public class IronPythonContext : SPCoder.Context.Context
    {
        public ScriptScope Scope { get; set; }
        public override void AddItem(ContextItem item)
        {
            base.AddItem(item);
            Scope.SetVariable(item.Name, item.Data);
        }

        public override object GetVariable(string name)
        {
            if (Scope.ContainsVariable(name))
            {
                return Scope.GetVariable(name);
            }
            return null;
        }
        public override void RemoveItem(ContextItem item)
        {
            base.RemoveItem(item);
            if (Scope.ContainsVariable(item.Name))
            {
                Scope.RemoveVariable(item.Name);
            }
        }

        public override void RenameItem(ContextItem item, string newName)
        {
            base.GetContext.Remove(item.Name);
            if (Scope.ContainsVariable(item.Name))
            {
                Scope.RemoveVariable(item.Name);
            }
            item.Name = newName;
            base.GetContext.Add(item.Name, item);
            Scope.SetVariable(item.Name, item.Data);
        }
    }
}