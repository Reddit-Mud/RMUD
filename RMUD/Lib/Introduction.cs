using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public static class Introduction
    {
        public static bool HasKnowledgeOf(Actor Player, Actor Whom)
        {
            if (Player is Player) return (Player as Player).Recall<bool>(Whom, "knows");
            return false;
        }

        public static void GrantKnowledgeOf(Actor Player, Actor Whom)
        {
            if (Player is Player) (Player as Player).Remember(Whom, "knows", true);
        }
    }
}
