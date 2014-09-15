using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public enum EventMessageScope
	{
		Private,
		Locality,
		Broadcast
	}

	public class EventMessage
	{
		public Actor TriggeredBy;
		public EventMessageScope Scope;
		public String Message;

		public EventMessage(Actor TriggeredBy, EventMessageScope Scope, String Message)
		{
			this.TriggeredBy = TriggeredBy;
			this.Scope = Scope;
			this.Message = Message;
		}

		public String FormatMessage(Actor For)
		{
			var replacement = (For == TriggeredBy) ? "you" : TriggeredBy.Short;
			return Message.Replace("{0}", replacement);
		}
	}
}
