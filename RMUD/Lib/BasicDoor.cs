using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class BasicDoor : Thing, IOpenableRules, ITakeRules
	{
		public BasicDoor()
		{
			this.Nouns.Add("DOOR", "CLOSED");
            Open = false;
		}

		#region IOpenable

		public bool Open { get; set; }

		CheckRule IOpenableRules.CanOpen(Actor Actor)
		{
            if (Open) return CheckRule.Disallow("It's already open.");
            else return CheckRule.Allow();
		}

		CheckRule IOpenableRules.CanClose(Actor Actor)
		{
            if (Open) return CheckRule.Allow();
            else return CheckRule.Disallow("It's already closed.");
		}

		void IOpenableRules.HandleOpen(Actor Actor)
		{
			Open = true;
            Nouns.RemoveAll(n => n == "CLOSED");
            Nouns.Add("OPEN");
		}

		void IOpenableRules.HandleClose(Actor Actor)
		{
			Open = false;
            Nouns.RemoveAll(n => n == "OPEN");
            Nouns.Add("CLOSED");
		}

		#endregion

		CheckRule ITakeRules.CanTake(Actor Actor)
		{
			return CheckRule.Disallow("Doors only make good doors if you leave them where they are at.");
		}

        RuleHandlerFollowUp ITakeRules.HandleTake(Actor Actor) { return RuleHandlerFollowUp.Continue; }
	}
}
