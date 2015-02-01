using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RMUD;

namespace ClothingModule
{
    public class ClothingMessages 
    {
        public static void AtStartup(RuleEngine GlobalRules)
        {
            Core.StandardMessage("clothing nude", "You are naked.");
            Core.StandardMessage("clothing wearing", "You are wearing..");
            Core.StandardMessage("clothing remove first", "You'll have to remove <the0> first.");
            Core.StandardMessage("clothing they are nude", "^<the0> is naked.");
            Core.StandardMessage("clothing they are wearing", "^<the0> is wearing <l1>.");
            Core.StandardMessage("clothing remove what", "I couldn't figure out what you're trying to remove.");
            Core.StandardMessage("clothing not wearing", "You'd have to be actually wearing that first.");
            Core.StandardMessage("clothing you remove", "You remove <the0>.");
            Core.StandardMessage("clothing they remove", "^<the0> removes <a1>.");
            Core.StandardMessage("clothing wear what", "I couldn't figure out what you're trying to wear.");
            Core.StandardMessage("clothing already wearing", "You're already wearing that.");
            Core.StandardMessage("clothing you wear", "You wear <the0>.");
            Core.StandardMessage("clothing they wear", "^<the0> wears <a1>.");
            Core.StandardMessage("clothing cant wear", "That isn't something that can be worn.");
        }
    }
}
