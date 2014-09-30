using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{	
	public class ConnectedPlayersObjectSource : IObjectSource
	{
        public List<MatchableObject> GetObjects(PossibleMatch State, CommandParser.MatchContext Context)
        {
            return new List<MatchableObject>(Mud.ConnectedClients.Where(c => c.IsLoggedOn).Select(c => new MatchableObject(c.Player, RelativeLocations.ConnectedPlayers)));
        }
	}
}
