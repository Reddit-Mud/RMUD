using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimum
{
    public class GameInfo : RMUD.SinglePlayer.GameInfo
    {
        public GameInfo()
        {
            Title = "Minimum";
            DatabaseNameSpace = "Minimum";
            Description = "This is a game with the absolute minimum required to run.";
            Modules = new List<string>(new String[] { "StandardActionsModule.dll", "AdminModule.dll" });
        }
    }
}
