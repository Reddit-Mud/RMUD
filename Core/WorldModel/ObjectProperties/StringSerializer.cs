using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD
{
    public class StringSerializer : TypeSerializer
    {
        public override string ConvertToString(object @object)
        {
            if (@object == null) return "%NULL";
            else return @object.ToString();
        }

        public override object ConvertFromString(string str)
        {
            if (str == "%NULL") return null;
            return str;
        }
    }
}
