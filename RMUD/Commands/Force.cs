using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Force : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new RankGate(500),
                    new KeyWord("FORCE"),
                    new FailIfNoMatches(
                        new FirstOf(
                            new ObjectMatcher("OBJECT", new InScopeObjectSource()),
                            new Path("PATH")),
                        "Whom do you wish to command?"),
                    new Rest("COMMAND")),
                "Force others to do your bidding.")
                .ProceduralRule((Match, Actor) =>
                {
                    MudObject target = null;
                    if (Match.Arguments.ContainsKey("OBJECT")) target = Match.Arguments["OBJECT"] as MudObject;
                    else if (Match.Arguments.ContainsKey("PATH")) target = Mud.GetObject(Match.Arguments["PATH"].ToString());

                    if (target == null)
                    {
                        Mud.SendMessage(Actor, "I can't find whomever it is you want to submit to your foolish whims.");
                        return PerformResult.Stop;
                    }

                    var targetActor = target as Actor;
                    if (targetActor == null)
                    {
                        Mud.SendMessage(Actor, "You can order inanimate objects about as much as you like, they aren't going to listen.");
                        return PerformResult.Stop;
                    }

                    var command = Match.Arguments["COMMAND"].ToString();
                    var matchedCommand = Mud.ParserCommandHandler.Parser.ParseCommand(command, targetActor);

                    if (matchedCommand != null)
                    {
                        if (matchedCommand.Matches.Count > 1)
                            Mud.SendMessage(Actor, "The command was ambigious.");
                        else
                        {
                            Mud.SendMessage(Actor, "Enacting your will.");
                            Mud.ProcessPlayerCommand(matchedCommand.Command, matchedCommand.Matches[0], targetActor);
                        }
                    }
                    else
                        Mud.SendMessage(Actor, "The command did not match.");

                    return PerformResult.Continue;
                });
        }
	}
}
