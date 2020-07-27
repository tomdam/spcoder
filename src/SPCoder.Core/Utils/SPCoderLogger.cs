using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.Core.Utils
{
    public class SPCoderLogger
    {
        private static ISPCoderLog m_logger;

        public static ISPCoderLog Logger {
            set
            {
                m_logger = value;
            }

            get
            {
                return m_logger;
            }
        }
    }
}
