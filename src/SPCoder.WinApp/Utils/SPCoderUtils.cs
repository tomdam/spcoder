using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SPCoder.Utils
{
    /// <summary>
    /// This class contains utility methods.
    /// </summary>
    /// <author>Damjan Tomic</author>
    public class SPCoderUtils
    {
        public static string LoadStringFromResource(string resourceName)
        {            
            string result = "";            
            Stream inFile = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (inFile != null)
            {
                using (StreamReader reader = new StreamReader(inFile))
                {
                    result = reader.ReadToEnd();
                }
                inFile.Close();
            }
            return result;
        }

        public static object Deserialize(Type type, string path)
        {
            TextReader r = null;
            try
            {
                XmlSerializer s = new XmlSerializer(type);
                r = new StreamReader(path);
                return s.Deserialize(r);
            }
            finally
            {
                if (r != null)
                {
                    r.Close();
                }
            }
        }


        public static void Serialize(object obj, string path)
        {
            TextWriter w = null;
            try
            {
                XmlSerializer s = new XmlSerializer(obj.GetType());
                w = new StreamWriter(path);
                s.Serialize(w, obj);
                w.Close();
            }
            finally
            {
                if (w != null)
                {
                    w.Close();
                }
            }
        }

        public static IList<string> GetPropertiesAndMethods(object obj, string objName)
        {
            /*
            if (obj is IronPython.Runtime.Types.OldInstance)
                return GetPythonObjectMethods(obj, objName);
            */
            if (obj != null)
            {
                var type = obj.GetType();
                if (AutocompleteItems.ContainsKey(type))
                    return AutocompleteItems[type];
            
            IList<string> all = GetProperties(obj, objName);
            ((List<string>)all).AddRange(GetMethods(obj, objName));
            ((List<string>)all).Sort();
            AutocompleteItems[type] = all;
            return all;
            }
            return new List<string>();
        }

        public static IList<string> GetPropertiesAndMethods(string[] callTrail)
        {
            string root = callTrail[0];
            Type type = typeof(void);

            try
            {
                object variable = SPCoderForm.ScriptStateCSharp.GetVariable(root);
                if (variable != null)
                    variable = ((Microsoft.CodeAnalysis.Scripting.ScriptVariable)variable).Value;

                type = variable.GetType();

                for (int i = 1; i < callTrail.Length; i++)
                {
                    string current = callTrail[i];
                    if (string.IsNullOrEmpty(current))
                        continue;

                    if (current.Contains("(")) //Method
                    {
                        try
                        {
                            current = current.Substring(0, current.IndexOf('('));
                            //type = type.GetMethod(current).ReturnType;
                            type = ((MethodInfo)type.GetMember(current)[0]).ReturnType;
                            variable = null;
                        }
                        catch (Exception exc)
                        {
                        }
                    }
                    else//Property
                    {
                        try
                        {
                            //type = type.GetProperty(current).PropertyType;
                            variable = type.GetProperty(current).GetValue(variable);
                            if (variable == null)
                            {
                                type = ((PropertyInfo)type.GetMember(current)[0]).PropertyType;
                            }
                            else
                            {
                                type = variable.GetType();
                            }
                        }
                        catch (Exception exc)
                        {
                            //check for Non static property requires the target
                        }
                    }
                }
            }
            catch (Exception)
            {
                //
            }

            if (AutocompleteItems.ContainsKey(type))
                return AutocompleteItems[type];

            IList<string> all = GetProperties(type);
            ((List<string>)all).AddRange(GetMethods(type));
            ((List<string>)all).Sort();
            AutocompleteItems[type] = all;
            return all;
             //return GetPropertiesAndMethods(obj, objName);
        }

        public static IDictionary<Type, IList<string>> AutocompleteItems = new Dictionary<Type, IList<string>>();

        public static IList<string> GetProperties(object obj, string objName)
        {
            if (obj != null)
            {
                return GetProperties(obj.GetType());
            }
            else
                return new List<string>();
        }

        public static IList<string> GetProperties(Type type)
        {
            IList<string> properties = new List<string>();
            //if (obj != null)
            {
                //Type type = obj.GetType();
                properties.Clear();
                System.Reflection.PropertyInfo[] infos = type.GetProperties();

                foreach (System.Reflection.PropertyInfo info in infos)
                {
                    try
                    {
                        properties.Add(info.Name);
                    }
                    catch (Exception)
                    {
                        //just continue if you get an exception
                    }
                }
            }
            return properties;
        }

        public static IList<string> GetMethods(object obj, string objName)
        {
            if (obj != null)
            {
                return GetMethods(obj.GetType());
            }
            else
                return new List<string>();
        }

        public static IList<string> GetMethods(Type type)
        {           
            IList<string> methods = new List<string>();
            //if (obj != null)
            {
                //Type type = obj.GetType();
                methods.Clear();
                System.Reflection.MethodInfo[] infos = type.GetMethods();

                foreach (System.Reflection.MethodInfo info in infos)
                {
                    try
                    {
                        if (!info.Name.StartsWith("get_") && !info.Name.StartsWith("set_") && !info.Name.StartsWith("remove_") && !info.Name.StartsWith("add_"))
                        {
                            methods.Add(info.Name + GetSignature(info.GetParameters()));//+ " : " + info.ReturnType.Name);
                        }
                    }
                    catch (Exception)
                    {
                        //just continue if you get an exception
                    }
                }
                //Ovde pokusaj da uzmes i System.Linq extension metode
               
                //var linqAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == "System.Core");
                //if (linqAssembly != null)
                {
                    TypeFilter myFilter = new TypeFilter(MyInterfaceFilter);
                    //String[] myInterfaceList = new String[2]
                      //{"System.Collections.IEnumerable", "System.Collections.ICollection"};
                    //Type[] myInterfaces = type.FindInterfaces(myFilter, "System.Collections.IEnumerable");
                    Type[] myInterfaces = type.GetInterfaces();
                    if (myInterfaces.Length > 0)
                    {
                        foreach (var myInterface in myInterfaces)
                        {
                            
                            var extensionMethods = GetExtensionMethods(myInterface.Assembly, myInterface);
                            if (extensionMethods != null)
                            {
                                foreach (var m in extensionMethods)
                                {
                                    methods.Add(m.Name + GetSignature(m.GetParameters()));
                                }
                            }
                        }
                    }
                }               
            }
            return methods;
        }

        static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly,    Type extendedType)
        {
            var query = from type in assembly.GetTypes()
                        //where type.IsSealed //&& !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        //where method.IsDefined(typeof(ExtensionAttribute), false)
                        where method.IsDefined(typeof(ExtensionAttribute), true)
                        where method.GetParameters()[0].ParameterType == extendedType 
                        || extendedType.IsAssignableFrom(method.GetParameters()[0].ParameterType)
                        select method;
            return query;
        }

        public static bool MyInterfaceFilter(Type typeObj, Object criteriaObj)
        {
            if (typeObj.ToString() == criteriaObj.ToString())
                return true;
            else
                return false;
        }

        /*public static IList<string> GetPythonObjectMethods(object obj, string objName)
        {

            IList<string> methods = new List<string>();
            //SPCoderForm.ironPythonEngine.SetVariable("obj", objectDescription.DescribedObject);
            string code = "reflect__temp = reflect.members(" + objName + ")";
            SPCoderForm.ironPythonEngine.Execute(code);
            
            //var rez = SPCoderForm.MainForm.MyContext.Scope.GetVariable("reflect__temp");
            var rez = SPCoderForm.MainForm.MyContext.GetVariable("reflect__temp");
            if (rez != null && rez is IronPython.Runtime.List)
            { 
                var r = rez as IronPython.Runtime.List;
                foreach (var item in r)
                {
                    methods.Add(item.ToString());
                }
            }

            return methods;
        }*/

        public static string GetSignature(ParameterInfo[] pars)
        {
            StringBuilder sb = new StringBuilder();
            //ParameterInfo[] pars = info.GetParameters();
            
            sb.Append("(");
            foreach (ParameterInfo par in pars)
            {
                sb.Append(par.ParameterType.Name + " " + par.Name +  ", ");
            }
            //sb.Replace(",","",sb.Length-2)
            sb.Append(")");
            sb = sb.Replace(", )", ");");
            //name = sb.ToString() + " : " +name;
            return sb.ToString();
        }
    }
}