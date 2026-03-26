using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class PathItem
    {
		public string InputFilePath { get; set; }
        public string OutputFilePath { get; set; }
        public string StatusFilePath { get; set; }
        public string LogFilePath { get; set; }
        public string PythonCodePath { get; set; }
        public string PythonDLL { get; set; }
    }
}
