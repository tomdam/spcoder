using System;
using SPCoder.Context;
using SPCoder.Describer;

namespace SPCoder.Describer
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Damjan Tomic</author>
    public class ObjectDescriber
    {
        public string DescribeItem(ContextItem item)
        {
            return this.GetObjectDescription(item.Data);
        }


        public string GetObjectDescription(object item)
        {            
            Type type = item.GetType();
            object data = item;
            ObjectDescription description = new ObjectDescription();
            if (type != null)
            {
                System.Reflection.PropertyInfo[] infos = type.GetProperties();

                foreach (System.Reflection.PropertyInfo info in infos)
                {
                    //if (info.CanRead)
                    {
                        try
                        {
                            description.Properties.Add(info.Name, info.GetValue(data, null));
                            //desc += info.Name + " " + info.GetValue(data, null) + "\n";
                        }
                        catch (Exception)
                        {
                            //just continue if you get an exception
                        }
                    }
                }
            }
            return description.GetPropertiesAsString("\n");
        }        
    }
}