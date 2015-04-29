using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD
{
    /// <summary>
    /// Define in modules to A) Identify the assembly as a module and B) Tell the engine what namespace
    /// to load from.
    /// </summary>
    public class ModuleInfo
    {
        public String BaseNameSpace = "Don't load me.";
        public String Author = "RMUD";
        public String Description = "Undescribed Module";
    }
}
