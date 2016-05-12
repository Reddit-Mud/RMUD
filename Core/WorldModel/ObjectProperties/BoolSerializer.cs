using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD
{
    public class BoolSerializer : TypeSerializer
    {
        public override string ConvertToString(object @object)
        {
            if (@object == null) return "%NULL";
            if (!(@object is bool)) return "false"; // wut?
            if ((@object as bool?).Value) return "true";
            return "false";
        }

        public override object ConvertFromString(string str)
        {
            return str == "true";
        }
    }
}
