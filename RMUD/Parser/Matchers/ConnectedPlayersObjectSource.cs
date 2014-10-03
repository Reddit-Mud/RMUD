using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{	
	public class ConnectedPlayersObjectSource : IObjectSource
	{
        public List<MudObject> GetObjects(PossibleMatch State, MatchContext Context)
        {
            return new List<MudObject>(Mud.ConnectedClients.Where(c => c.IsLoggedOn).Select(c => c.Player));
        }
	}
}
