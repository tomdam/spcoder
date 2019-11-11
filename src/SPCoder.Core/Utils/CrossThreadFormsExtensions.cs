using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/*[assembly: Microsoft.Scripting.Runtime.ExtensionType(
    typeof(System.Windows.Forms.Control),
    typeof(SPCoder.Core.Utils.CrossThreadFormsExtensions)
    )]*/
namespace SPCoder.Core.Utils
{
    public static class CrossThreadFormsExtensions
    {
        public static void PerformSafely(this Control target, Action action)
        {
            if (target.InvokeRequired)
            {
                target.BeginInvoke(action);
            }
            else
            {
                action();
            }
        }

        public static void PerformSafely2<T1>(this Control target, Action<T1> action, T1 parameter)
        {
            if (target.InvokeRequired)
            {
                target.BeginInvoke(action, parameter);
            }
            else
            {
                action(parameter);
            }
        }

        public static void PerformSafely3<T1, T2>(this Control target, Action<T1, T2> action, T1 p1, T2 p2)
        {
            if (target.InvokeRequired)
            {
                target.BeginInvoke(action, p1, p2);
            }
            else
            {
                action(p1, p2);
            }
        }
    }
}
