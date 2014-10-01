using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Examine : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new Or(
                        new KeyWord("EXAMINE", false),
                        new KeyWord("X", false)),
                    new FailIfNoMatches(
                        new ObjectMatcher("OBJECT", new InScopeObjectSource()),
                        "I don't see that here.\r\n")),
                new ExamineProcessor(),
                "Look closely at an object.");

        }
	}

	internal class ExamineProcessor : ICommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
            var target = Match.Arguments["OBJECT"] as MudObject;

			if (Actor.ConnectedClient != null)
			{
                if (!Mud.IsVisibleTo(Actor, target))
                {
                    Mud.SendMessage(Actor, "That doesn't seem to be here anymore.\r\n");
                    return;
                }

                var builder = new StringBuilder();
                if (!(target is IDescribed))
                    builder.Append("That object is indescribeable.\r\n");
                else
                    builder.Append((target as IDescribed).Long.Expand(Actor, target as MudObject) + "\r\n");

                var openable = target as OpenableRules;
                if (openable != null)
                {
                    if (openable.Open)
                        builder.Append(Mud.CapFirst(String.Format("{0} is open.\r\n", (target as Thing).Definite)));
                    else
                        builder.Append(Mud.CapFirst(String.Format("{0} is closed.\r\n", (target as Thing).Definite)));
                }

                var container = target as IContainer;
                if (container != null && ((container.LocationsSupported & RelativeLocations.On) == RelativeLocations.On))
                {
                    var contents = Mud.GetContents(container, RelativeLocations.On).Where(o => o is Thing);
                    if (contents.Count() > 0)
                    {
                        builder.Append(String.Format("\r\nOn {0} is ", (target as Thing).Definite));
                        builder.Append(String.Join(", ", contents.Select(o => (o as Thing).Indefinite)));
                        builder.Append(".\r\n");
                    }
                }

                if (Mud.HasVisibleContents(target as MudObject))
                {
                    var contents = Mud.GetContents(container, RelativeLocations.In).Where(o => o is Thing);
                    if (contents.Count() > 0)
                    {
                        builder.Append(String.Format("\r\nIn {0} is ", (target as Thing).Definite));
                        builder.Append(String.Join(", ", contents.Select(o => (o as Thing).Indefinite)));
                        builder.Append(".\r\n");
                    }
                }

                Mud.SendMessage(Actor, builder.ToString());
			}
		}
	}
}
