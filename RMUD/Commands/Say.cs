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
					new Rest("SPEECH")),
				new SayProcessor(SayProcessor.EmoteTypes.Speech),
				"Say something.");

			Parser.AddCommand(
				new Sequence(
					new Or(
						new KeyWord("EMOTE", false),
						new KeyWord("\"", false)),
					new Rest("SPEECH")),
				new SayProcessor(SayProcessor.EmoteTypes.Emote),
				"Emote something.");
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

			for (var node = Match.Arguments["SPEECH"] as LinkedListNode<String>; node != null; node = node.Next)
			{
				speechBuilder.Append(node.Value);
				speechBuilder.Append(" ");
			}

			speechBuilder.Remove(speechBuilder.Length - 1, 1);
			if (EmoteType == EmoteTypes.Speech)
				speechBuilder.Append("\"\r\n");
			else
				speechBuilder.Append("\r\n");

			Mud.SendEventMessage(Actor, EventMessageScope.Local, speechBuilder.ToString());
		}
	}
}
