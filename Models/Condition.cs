using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace HYDB.FilterEngine
{
    public class Condition
    {
        public string Left { get; set; }
        public string Right { get; set; }
        public string Operator { get; set; }
        public List<Variable> Definitions { get; set; }
    }
}
