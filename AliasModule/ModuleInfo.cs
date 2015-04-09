using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ModuleInfo : RMUD.ModuleInfo
{
    public ModuleInfo()
    {
        BaseNameSpace = "AliasModule";
        Author = "Blecki";
        Description = "Allow players to create aliases for commands.";
    }
}
