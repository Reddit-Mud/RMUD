using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace RMUD
{
    public partial class MudObject
    {
        public static String CapFirst(String str)
        {
            if (str.Length > 1)
                return str.Substring(0, 1).ToUpper() + str.Substring(1).ToLower();
            if (str.Length == 1)
                return str.ToUpper();
            return str;
        }
    }
}
