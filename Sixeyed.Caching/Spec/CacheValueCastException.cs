using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sixeyed.Caching
{
    public class CacheValueCastException : Exception
    {
        public CacheValueCastException(string message)
        : base(message) { }
    }
}
