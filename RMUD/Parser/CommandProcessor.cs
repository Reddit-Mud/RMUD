using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    public interface CommandProcessor
    {
		void Perform(PossibleMatch Match, Actor Actor);
    }

    public static class CommandHelper
    {
        public static bool CheckHolding(MudObject Actor, MudObject Target)
        {
            if (!Mud.ObjectContainsObject(Actor, Target))
            {
                Mud.SendMessage(Actor, "You'd have to be holding <the0> for that to work.", Target);
                return false;
            }
            return true;
        }
    }

	public class CommandProcessorWrapper : CommandProcessor
	{
		private Action<PossibleMatch, Actor> Processor;

		public CommandProcessorWrapper(Action<PossibleMatch, Actor> Processor)
		{
			this.Processor = Processor;
		}

		public void Perform(PossibleMatch Match, Actor Actor)
		{
			Processor(Match, Actor);
		}
	}

}
