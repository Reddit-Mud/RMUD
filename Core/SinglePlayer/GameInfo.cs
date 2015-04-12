using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD.SinglePlayer
{
    public class GameInfo
    {
        public String Title = "Untitled";
        public String Author = "RMUD";
        public String DatabaseNameSpace = "Don't load me.";
        public String Description = "Undescribed game.";
        public List<String> Modules = new List<string>();
    }
}
