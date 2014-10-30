using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class AcceptQuest : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                new Sequence(
                    new KeyWord("ACCEPT"),
                    new KeyWord("QUEST")),
                new AcceptQuestProcessor(),
                "Accept an offered quest.");
        }
	}

	internal class AcceptQuestProcessor : CommandProcessor
	{
		public void Perform(PossibleMatch Match, Actor Actor)
		{
            var player = Actor as Player;
            if (player == null) return;

            if (player.OfferedQuest == null)
            {
                Mud.SendMessage(Actor, "Nobody has offered you a quest.");
                return;
            }

            if (player.OfferedQuest.CheckQuestStatus(Actor) != QuestStatus.Available)
            {
                Mud.SendMessage(Actor, "The quest is no longer available.");
                player.OfferedQuest = null;
                return;
            }

            if (player.ActiveQuest != null)
                player.ActiveQuest.HandleQuestEvent(QuestEvents.Abandoned, player);

            player.ActiveQuest = player.OfferedQuest;
            player.OfferedQuest = null;
            player.ActiveQuest.HandleQuestEvent(QuestEvents.Accepted, player);
		}
	}
}
