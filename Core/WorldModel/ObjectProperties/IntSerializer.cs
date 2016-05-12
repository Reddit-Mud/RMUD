using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD
{
    public class IntSerializer : TypeSerializer
    {
        public override string ConvertToString(object @object)
        {
            if (@object == null) return "%NULL";
            else return @object.ToString();
        }

        public override object ConvertFromString(string str)
        {
            var r = 0;
            if (!Int32.TryParse(str, out r)) throw new InvalidOperationException();
            return r;
        }
    }
}
