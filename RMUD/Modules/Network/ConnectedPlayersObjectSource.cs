using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Modules.Network
{	
	public class ConnectedPlayersObjectSource : IObjectSource
	{
        public List<MudObject> GetObjects(PossibleMatch State, MatchContext Context)
        {
            return new List<MudObject>(Clients.ConnectedClients.Where(c => c is NetworkClient && (c as NetworkClient).IsLoggedOn).Select(c => c.Player));
        }
	}
}
