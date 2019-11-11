using SPCoder.Config;
using SPCoder.Utils;
using System;

namespace SPCoder.Context
{
    public class ContextFactory
    {

        private static Context m_context = null;
        public static Context GetCurrentContext()
        {
            if (m_context == null)
            {
                SPCoderConfig spCoderConfig = (SPCoderConfig)ConfigUtils.GetConfig(null, typeof(SPCoderConfig));
                string activeConfigName = spCoderConfig.GetItemValueByName(SPCoderConstants.ACTIVE_CONFIG);
                if (activeConfigName == SPCoderConstants.CSHARP_CONFIG)
                {
                    m_context = new CSharpContext();
                }
                //else if (activeConfigName == SPCoderConstants.IP_CONFIG)
                //{
                //    m_context = new IronPythonContext();
                //}
                else
                {
                    throw new Exception("Active config not set");
                }
            }
            return m_context;
        }
    }
}
