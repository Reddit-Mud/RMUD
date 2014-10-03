using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Say : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
			Parser.AddCommand(
				new Sequence(
					new Or(
						new KeyWord("SAY", false),
						new KeyWord("'", false)),
                    new FailIfNoMatches(
					    new Rest("SPEECH"),
                        "Say what?\r\n")),
				new SayProcessor(SayProcessor.EmoteTypes.Speech),
				"Say someMudObject.");

			Parser.AddCommand(
				new Sequence(
					new Or(
						new KeyWord("EMOTE", false),
						new KeyWord("\"", false)),
                    new FailIfNoMatches(
					    new Rest("SPEECH"),
                        "You exist. Actually this is an error message, but that's what you just told me to say.\r\n")),
				new SayProcessor(SayProcessor.EmoteTypes.Emote),
				"Emote someMudObject.");
		}
	}

	internal class SayProcessor : ICommandProcessor
	{
		public enum EmoteTypes
		{
			Speech,
			Emote
		}

		public EmoteTypes EmoteType;

		public SayProcessor(EmoteTypes EmoteType)
		{
			this.EmoteType = EmoteType;
		}

		public void Perform(PossibleMatch Match, Actor Actor)
		{
            var speechBuilder = new StringBuilder();

            speechBuilder.Append(Actor.Short);
            if (EmoteType == EmoteTypes.Speech)
                speechBuilder.Append(": \"");
            else
                speechBuilder.Append(" ");

            Mud.AssembleText(Match.Arguments["SPEECH"] as LinkedListNode<String>, speechBuilder);

			if (EmoteType == EmoteTypes.Speech)
				speechBuilder.Append("\"\r\n");
			else
				speechBuilder.Append("\r\n");

			Mud.SendMessage(Actor, MessageScope.Local, speechBuilder.ToString());
		}

        
	}
}
