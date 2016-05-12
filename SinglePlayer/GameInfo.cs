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
            Title = "Cloak Of Darkness";
            DatabaseNameSpace = "CloakOfDarkness";
            Description = "This game implements the famous CLOAK OF DARKNESS sample that all new Interactive Fiction Authoring Systems are required to implement. By law. The source contains the Inform7 source for the game as comments.";
            Modules = new List<string>(new String[] { "StandardActionsModule.dll", "AdminModule.dll", "ClothingModule.dll" });
        }
    }
}
