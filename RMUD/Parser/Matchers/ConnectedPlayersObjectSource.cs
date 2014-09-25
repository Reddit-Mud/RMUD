using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{	
	public class ConnectedPlayersObjectSource : IObjectSource
	{
        public List<IMatchable> GetObjects(PossibleMatch State, CommandParser.MatchContext Context)
        {
            return new List<IMatchable>(Mud.ConnectedClients.Where(c => c.IsLoggedOn).Select(c => c.Player));
        }
	}
}
