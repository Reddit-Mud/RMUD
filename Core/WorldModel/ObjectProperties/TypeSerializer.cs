using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD
{
    public class TypeSerializer
    {
        public virtual String ConvertToString(Object @object)
        {
            throw new NotImplementedException();
        }

        public virtual Object ConvertFromString(String str)
        {
            throw new NotImplementedException();
        }
    }
}
