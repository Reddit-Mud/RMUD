using RMUD;
using StandardActionsModule;

namespace Akko.Areas.Test
{
    public class Player : RMUD.Player
    {
        public override void Initialize()
        {
            Short = "you";
            Long = "You are a generic human female.";            
        }
    }
}
