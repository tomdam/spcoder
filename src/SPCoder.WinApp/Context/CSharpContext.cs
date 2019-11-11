using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace SPCoder.Context
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Damjan Tomic</author>
    public class CSharpContext : SPCoder.Context.Context
    {

        public CSharpContext()
        {
            string script = "";
            SPCoderForm.ScriptStateCSharp = SPCoderForm.ScriptStateCSharp == null ?
                    CSharpScript.RunAsync(script, ScriptOptions.Default.AddImports("System"), this).Result :
                    SPCoderForm.ScriptStateCSharp.ContinueWithAsync(script).Result;
        }

        public object DataHolder = null;
        public override void AddItem(ContextItem item)
        {
            base.AddItem(item);
            DataHolder = item.Data;
            string script = item.Data.GetType().FullName + " " + item.Name + " = (" + item.Data.GetType().FullName + ") DataHolder;";
            SPCoderForm.ScriptStateCSharp = SPCoderForm.ScriptStateCSharp.ContinueWithAsync(script).Result;
        }

        public override void AddOrUpdateItem(ContextItem item)
        {
            bool alreadyExistsInContext = GetContext.ContainsKey(item.Name);
            base.AddOrUpdateItem(item);
            DataHolder = item.Data;
            string script = "";
            if (alreadyExistsInContext)
            {
                //
                script = item.Name + " = (" + item.Data.GetType().FullName + ")DataHolder;";
            }
            else
            {
                script = item.Data.GetType().FullName + " " + item.Name + " = (" + item.Data.GetType().FullName + ") DataHolder;";
            }
            
            SPCoderForm.ScriptStateCSharp = SPCoderForm.ScriptStateCSharp.ContinueWithAsync(script).Result;
        }
        
        public override void UpdateItem(ContextItem item)
        {
            base.UpdateItem(item);            
            DataHolder = item.Data;
            string script =  item.Name + " = (" + item.Data.GetType().FullName + ") DataHolder;";
            SPCoderForm.ScriptStateCSharp = SPCoderForm.ScriptStateCSharp.ContinueWithAsync(script).Result;
        }

        public override object GetVariable(string name)
        {
            if (GetContext.ContainsKey(name))
            {
                return SPCoderForm.ScriptStateCSharp.GetVariable(name);
            }
            return null;
        }

        public override void RemoveItem(ContextItem item)
        {
            string script = item.Name + " = default;";
            SPCoderForm.ScriptStateCSharp = SPCoderForm.ScriptStateCSharp.ContinueWithAsync(script).Result;
            base.RemoveItem(item);
        }

        public override void RenameItem(ContextItem item, string newName)
        {
            base.GetContext.Remove(item.Name);
            item.Name = newName;

            DataHolder = item.Data;
            string script = item.Data.GetType().FullName + " " + item.Name + " = (" + item.Data.GetType().FullName + ") DataHolder;";
            SPCoderForm.ScriptStateCSharp = SPCoderForm.ScriptStateCSharp.ContinueWithAsync(script).Result;

            base.GetContext.Add(item.Name, item);
        }
    }
}