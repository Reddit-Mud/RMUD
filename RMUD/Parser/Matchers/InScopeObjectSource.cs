using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{	
	public class InScopeObjectSource : IObjectSource
	{
		public List<IMatchable> GetObjects(PossibleMatch State, CommandParser.MatchContext Context)
		{
			return new List<IMatchable>(Context.ObjectsInScope.Select(t => t as IMatchable));
		}
	}
}
