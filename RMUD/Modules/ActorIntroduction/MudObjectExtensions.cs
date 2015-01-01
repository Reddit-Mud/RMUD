using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public partial class MudObject
    {
        public static bool ActorKnowsActor(Actor Player, Actor Whom)
        {
            if (Player is Player) return (Player as Player).Recall<bool>(Whom, "knows");
            return false;
        }

        public static void IntroduceActorToActor(Actor Introductee, Actor ToWhom)
        {
            if (ToWhom is Player) (ToWhom as Player).Remember(Introductee, "knows", true);
        }

        public static void Introduce(Actor Introductee)
        {
            var locale = MudObject.FindLocale(Introductee);
            if (locale != null)
                foreach (var player in MudObject.EnumerateObjectTree(locale).Where(o => o is Player).Select(o => o as Player))
                    IntroduceActorToActor(Introductee, player);
        }
    }
}
