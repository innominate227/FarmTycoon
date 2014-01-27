using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public abstract class DataFile
    {
        /// <summary>
        /// Full text of the data file.
        /// </summary>
        protected string m_dataFileText;

        public DataFile(string dataFileText)
        {
            m_dataFileText = dataFileText;
        }

        /// <summary>
        /// Pare the data file.
        /// Parsing is not done in the constuctor so that all data file object can be created, becayse some data files reference others.
        /// </summary>
        public abstract void ParseFile();
        

        /// <summary>
        /// Full text of the data file.
        /// </summary>
        public string DataFileText
        {
            get { return m_dataFileText; }
            set { m_dataFileText = value; }
        }

    }
}
