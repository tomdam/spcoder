using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPCoder.Utils
{
    public abstract class BaseEditedFile
    {
        /// <summary>
        /// Gets or sets the filename
        /// </summary>
        public string Filename { get; set; }


        /// <summary>
        /// Gets or sets the full path to the file
        /// </summary>
        public string FullFilePath { get; set; }

        /// <summary>
        /// Gets or sets the Stream that contains the file contents
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        /// Gets or sets an object that contains the file
        /// </summary>
        public object ParentContainer { get; set; }

        /// <summary>
        /// Override this to control file saving logic
        /// </summary>
        /// <param name="overwrite"></param>
        /// <returns>True if the file was saved, False if not</returns>
        public virtual bool Save(bool overwrite = false)
        {
            return true;
        }
    }
}
