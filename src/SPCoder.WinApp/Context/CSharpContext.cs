using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Reflection;

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
                    CSharpScript.RunAsync(script, DefaultScriptOptions(), this).Result :
                    //CSharpScript.RunAsync(script, ScriptOptions.Default.AddImports("System"), this).Result :
                    SPCoderForm.ScriptStateCSharp.ContinueWithAsync(script).Result;
        }


        private ScriptOptions DefaultScriptOptions()
        {
            ScriptOptions options = ScriptOptions.Default.AddImports("System");
            //get all referenced assemblies and reference them to try to avoid issue of not recognizing the same types as same
            //due to a bug in roslyn
            var loadedFiles = ((System.ComponentModel.Composition.Hosting.DirectoryCatalog)SPCoderForm.MainForm.container.Catalog).LoadedFiles;
            List<string> assemblyLocations = new List<string>(loadedFiles);
            assemblyLocations.Add(typeof(SPCoderForm).Assembly.Location);

            List<Assembly> assemblies = new List<Assembly>();

            foreach (var loadedFile in assemblyLocations)
            {
                List<Assembly> referencedAssemblies = GetReferencedAssembliesNames(loadedFile);
                foreach (var assembly in referencedAssemblies)
                {
                    if (!assemblies.Contains(assembly))
                    {
                        assemblies.Add(assembly);
                    }
                }
            }


            options = options.WithReferences(assemblies);
            return options;
        }

        private List<Assembly> GetReferencedAssembliesNames(string assemblyPath)
        {
            List<Assembly> assemblies = new List<Assembly>();
            var assembly = Assembly.LoadFrom(assemblyPath);
            assemblies.Add(assembly);
            var referenced = assembly.GetReferencedAssemblies();
            
            foreach (var aa in referenced)
            {
                try
                {
                    var ass1 = Assembly.Load(aa.FullName);
                    assemblies.Add(ass1);
                }
                catch (System.Exception exc)
                {
                    Console.WriteLine(exc);
                    //
                }
            }
            return assemblies;
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