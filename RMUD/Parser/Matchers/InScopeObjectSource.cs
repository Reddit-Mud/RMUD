using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{	
	public class InScopeObjectSource : IObjectSource
	{
		public List<MatchableObject> GetObjects(PossibleMatch State, MatchContext Context)
		{
			return new List<MatchableObject>(Context.ObjectsInScope);
		}
	}
}
