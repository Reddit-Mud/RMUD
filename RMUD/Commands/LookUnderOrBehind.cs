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

	internal class LookLocProcessor : ICommandProcessor
	{
        public void Perform(PossibleMatch Match, Actor Actor)
        {
            if (Actor.ConnectedClient == null) return;

            var target = Match.Arguments["OBJECT"] as Thing;
            var container = target as IContainer;

            if (!Mud.IsVisibleTo(Actor, target))
            {
                Mud.SendMessage(Actor, "That doesn't seem to be here anymore.\r\n");
                return;
            }

            var relloc = (Match.Arguments["RELLOC"] as RelativeLocations?).Value;

            if (container == null || ((container.LocationsSupported & relloc) != relloc))
            {
                Mud.SendMessage(Actor, String.Format("You can't look {0} that.\r\n", Mud.RelativeLocationName(relloc)));
                return;
            }

            if (relloc == RelativeLocations.In)
            {
                var openable = target as OpenableRules;
                if (openable != null && !openable.Open)
                {
                    Mud.SendMessage(Actor, Mud.CapFirst(String.Format("{0} is closed.\r\n", target.Definite)));
                    return;
                }
            }

            var contents = Mud.GetContents(container, relloc).Where(o => o is Thing);
            if (contents.Count() > 0)
            {
                var builder = new StringBuilder();

                builder.Append(Mud.CapFirst(String.Format("{0} {1} is ", Mud.RelativeLocationName(relloc), (target as Thing).Definite)));
                builder.Append(String.Join(", ", contents.Select(o => (o as Thing).Indefinite)));
                builder.Append(".\r\n");

                Mud.SendMessage(Actor, builder.ToString());
            }
            else
            {
                Mud.SendMessage(Actor, String.Format("There is nothing {0} {1}.\r\n", Mud.RelativeLocationName(relloc), target.Definite));
            }
        }
	}
}
