using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.Core.Plugins
{
    public abstract class BasePlugin
    {
        public delegate void CallbackDelegate(object o);
        public event CallbackDelegate Callback;

        public void ExecuteCallback(object o)
        {
            Callback(o);
        }

        public object Result { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Abstract method that should be implemented in subclasses. Executes the plugin logic.
        /// </summary>
        /// <param name="target">Object that is the target of the invocation</param>
        public abstract void Execute(Object target);
        
        public string Filter { get; set; }

        public Type TargetType { get; set; }

        /// <summary>
        /// Checks if the plugin is active for the specific target.
        /// Default implementation checks if the type of the object is the same as TargetType property.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual bool IsActive(Object target)
        {
            if (target != null && this.TargetType == target.GetType())
                return true;
            return false;
        }
    }
}