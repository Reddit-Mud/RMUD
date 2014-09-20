using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Door : Thing, IOpenable, Commands.ITakeRules
	{
		public Door()
		{
			this.Nouns.Add("DOOR");
		}

		#region IOpenable

		public bool Open { get; set; }

		bool IOpenable.CanOpen(Actor Actor)
		{
			return !Open;
		}

		bool IOpenable.CanClose(Actor Actor)
		{
			return Open;
		}

		void IOpenable.HandleOpen(Actor Actor)
		{
			if (!Open)
			{
				if (Actor.ConnectedClient != null)
					Mud.SendEventMessage(Actor, EventMessageScope.Single, "You open the door.\r\n");
				Mud.SendEventMessage(Actor, EventMessageScope.External, Actor.Short + " opens the door.\r\n");
			}

			Open = true;
		}

		void IOpenable.HandleClose(Actor Actor)
		{
			if (Open)
			{
				if (Actor.ConnectedClient != null)
					Mud.SendEventMessage(Actor, EventMessageScope.Single, "You close the door.\r\n");
				Mud.SendEventMessage(Actor, EventMessageScope.External, Actor.Short + " closes the door.\r\n");
			}

			Open = false;
		}

		#endregion

		bool Commands.ITakeRules.CanTake(Actor Actor)
		{
			return false;
		}
	}
}
