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
                        "I don't see that here.")),
                new ExamineProcessor(),
                "Look closely at an object.");

            Parser.AddCommand(
                new Sequence(
                    new Or(
                        new KeyWord("LOOK", false),
                        new KeyWord("L", false)),
                    new KeyWord("AT", false),
                    new FailIfNoMatches(
                        new ObjectMatcher("OBJECT", new InScopeObjectSource()),
                        "I don't see that here.")),
                new ExamineProcessor(),
                "Look closely at an object.");


        }
	}

	internal class ExamineProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
            var target = Match.Arguments["OBJECT"] as MudObject;

			if (Actor.ConnectedClient != null)
			{
                if (!Mud.IsVisibleTo(Actor, target))
                {
                    Mud.SendMessage(Actor, "That doesn't seem to be here anymore.");
                    return;
                }

                var builder = new StringBuilder();
                
                    builder.Append(target.Long.Expand(Actor, target as MudObject) + "\r\n");

                var openable = target as OpenableRules;
                if (openable != null)
                {
                    if (openable.Open)
                        builder.Append(Mud.CapFirst(String.Format("{0} is open.\r\n", (target as MudObject).Definite(Actor))));
                    else
                        builder.Append(Mud.CapFirst(String.Format("{0} is closed.\r\n", (target as MudObject).Definite(Actor))));
                }

                var container = target as Container;
                if (container != null && ((container.LocationsSupported & RelativeLocations.On) == RelativeLocations.On))
                {
                    var contents = Mud.GetContents(container, RelativeLocations.On).Where(o => o is MudObject);
                    if (contents.Count() > 0)
                    {
                        builder.Append(String.Format("\r\nOn {0} is ", (target as MudObject).Definite(Actor)));
                        builder.Append(String.Join(", ", contents.Select(o => (o as MudObject).Indefinite(Actor))));
                        builder.Append(".\r\n");
                    }
                }

                if (Mud.HasVisibleContents(target as MudObject))
                {
                    var contents = Mud.GetContents(container, RelativeLocations.In).Where(o => o is MudObject);
                    if (contents.Count() > 0)
                    {
                        builder.Append(String.Format("\r\nIn {0} is ", (target as MudObject).Definite(Actor)));
                        builder.Append(String.Join(", ", contents.Select(o => (o as MudObject).Indefinite(Actor))));
                        builder.Append(".\r\n");
                    }
                }

                Mud.SendMessage(Actor, builder.ToString());
			}
		}
	}
}
