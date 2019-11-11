using System;
using System.Collections.Generic;
using System.Linq;

namespace SPCoder.Describer
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Damjan Tomic</author>
    public class ObjectDescription
    {
        public IDictionary<string, object> Properties { get; set; }
        public int DescribedValueMaxLength { get; set; }
        public string Separator { get; set; }
        public object DescribedObject { get; set; }

        private const string defaultSeparator = "\n";

        //private List<string> keys;
        private IOrderedEnumerable<string> sortedKeys;
        public ObjectDescription()
        {
            Properties = new Dictionary<string, object>();
            DescribedValueMaxLength = 2048;
            Separator = defaultSeparator;            
        }

        private List<string> GetKeysList()
        {            
            if (sortedKeys != null)
            {
                return sortedKeys.ToList();
            }
            return Properties.Keys.ToList();
        }

        public void Sort(string order, string what)
        {
            if (order == "Asc")
            {
                if (what == "Names")
                {
                    sortedKeys = Properties.Keys.OrderBy(k => k);
                }
                else //what = "Values"
                {
                    sortedKeys = Properties.Keys.OrderBy(k => (Properties[k] == null) ? "" : Properties[k].ToString());
                }
            }
            else // (order == "Desc")
            {
                if (what == "Names")
                {
                    sortedKeys = Properties.Keys.OrderByDescending(k => k);
                }
                else //what = "Values"
                {
                    sortedKeys = Properties.Keys.OrderByDescending(k => (Properties[k] == null) ? "" : Properties[k].ToString());
                }
            }            
        }

        public string GetPropertiesAsString(string separator)
        {
            if (separator == null)
            {
                separator = defaultSeparator;
            }
            if (DescribedObject == null)
            {
                return "";
            }
//            if (Properties == null || Properties.Count == 0)
//            {
//
//            }
            string result = "";
            int length = GetMaxPropertyLength();
            result += string.Format("{0,-" + (length) + "}", "Value") + " : " + DescribedObject.ToString() + separator;
            result += string.Format("{0,-" + (length) + "}", "Type") + " : " + DescribedObject.GetType().ToString() + separator;

            result += new string('-', length) + separator;
            List<string> keys = GetKeysList();

            foreach (var key in keys)
            {
                string value = (Properties[key] != null) ? Properties[key].ToString() : "";
                if (value.Length >= DescribedValueMaxLength)
                {
                    value = value.Substring(0, DescribedValueMaxLength);
                }
                result += string.Format("{0,-" + (length) + "}", key) + " : " + value + separator;
            }
            return result;
        }

        public string GetObjectDescription(object item)
        {
            this.DescribedObject = item;
            return GetObjectDescription();
        }

        public string GetObjectDescription()
        {
            if (DescribedObject != null)
            {
                Type type = DescribedObject.GetType();
                Properties.Clear();
                System.Reflection.PropertyInfo[] infos = type.GetProperties();

                foreach (System.Reflection.PropertyInfo info in infos)
                {
                    try
                    {
                        Properties.Add(info.Name, info.GetValue(DescribedObject, null));
                    }
                    catch (Exception)
                    {
                        //just continue if you get an exception
                    }
                }
            }
            return GetPropertiesAsString(null);
        }   

        private int GetMaxPropertyLength()
        {
            int l = 0;
            foreach (var o in Properties)
            {
                if (o.Key.Length > l)
                    l = o.Key.Length;
            }
            return l;
        }
    }
}