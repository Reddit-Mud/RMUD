using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class LookUnderOrBehind : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new Or(
                        new KeyWord("LOOK", false),
                        new KeyWord("L", false)),
                    new RelativeLocationMatcher("RELLOC"),
                    new ObjectMatcher("OBJECT", new InScopeObjectSource())),
                new LookLocProcessor(),
                "Look on, in, under, or behind an object.");

        }
	}

	internal class LookLocProcessor : CommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Actor.ConnectedClient == null) return;

            var target = Match.Arguments["OBJECT"] as MudObject;
            var container = target as Container;

            if (!Mud.IsVisibleTo(Actor, target))
            {
                Mud.SendMessage(Actor, "That doesn't seem to be here anymore.");
                return;
            }

            var relloc = (Match.Arguments["RELLOC"] as RelativeLocations?).Value;

            if (container == null || ((container.LocationsSupported & relloc) != relloc))
            {
                Mud.SendMessage(Actor, String.Format("You can't look {0} that.", Mud.GetRelativeLocationName(relloc)));
                return;
            }

            if (relloc == RelativeLocations.In)
            {
                if (!Mud.IsOpen(target))
                {
                    Mud.SendMessage(Actor, "^<the0> is closed.", target);
                    return;
                }
            }

            var contents = container.GetContents(relloc);
            if (contents.Count() > 0)
            {
                var builder = new StringBuilder();

                builder.Append(String.Format("^{0} {1} is ", Mud.GetRelativeLocationName(relloc), (target as MudObject).Definite(Actor)));
                builder.Append(String.Join(", ", contents.Select(o => (o as MudObject).Indefinite(Actor))));
                builder.Append(".");

                Mud.SendMessage(Actor, builder.ToString());
            }
            else
            {
                Mud.SendMessage(Actor, String.Format("There is nothing {0} {1}.", Mud.GetRelativeLocationName(relloc), target.Definite(Actor)));
            }
        }
	}
}
