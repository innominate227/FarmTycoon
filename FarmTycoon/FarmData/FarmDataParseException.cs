using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FarmTycoon
{
    public class FarmDataParseException : Exception
    {
        public FarmDataParseException(string message) : base(message) { }
    }
}
