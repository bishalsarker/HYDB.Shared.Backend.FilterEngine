using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.FilterEngine
{
    public class Variable
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public dynamic Value { get; set; }
    }
}
