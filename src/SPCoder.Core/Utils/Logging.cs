using log4net;
using System;

namespace SPCoder.Core.Utils
{
    public class SPCoderLogging
    {
        private static ILog m_log;        
     
        public static ILog Logger
        {
            get
            {
                if (m_log==null)
                {
                    m_log = LogManager.GetLogger("spcoder-logger");                    
                }
                
                return m_log;
            }
        }
    }
    
}
