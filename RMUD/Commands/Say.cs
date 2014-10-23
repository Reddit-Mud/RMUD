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
                        "Say what?")),
				new SayProcessor(SayProcessor.EmoteTypes.Speech),
				"Say something.");

            Parser.AddCommand(
                new GenericMatcher((pm, context) =>
                    {
                        var r = new List<PossibleMatch>();
                        if (pm.Next == null || pm.Next.Value.Length <= 1 || pm.Next.Value[0] != '\'')
                            return r;

                        pm.Next.Value = pm.Next.Value.Substring(1); //remove the leading '
                        r.Add(pm.EndWith("SPEECH", pm.Next));
                        return r;
                    }, "' [TEXT]"),
                new SayProcessor(SayProcessor.EmoteTypes.Speech),
                "Say something.");

			Parser.AddCommand(
				new Sequence(
					new Or(
						new KeyWord("EMOTE", false),
						new KeyWord("\"", false)),
                    new FailIfNoMatches(
					    new Rest("SPEECH"),
                        "You exist. Actually this is an error message, but that's what you just told me to say.")),
				new SayProcessor(SayProcessor.EmoteTypes.Emote),
				"Emote something.");
		}
	}

	internal class SayProcessor : CommandProcessor
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
				speechBuilder.Append("\"");

			Mud.SendLocaleMessage(Actor, speechBuilder.ToString());
		}

        
	}
}
