using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
    [Flags]
    public enum RelativeLocations
    {
        None = 0,
        Default = 0,

        Contents = 1, //As of a room
        Links = 2,
        Scenery = 4,
        
        In = 8,
        On = 16,
        Under = 32,
        Behind = 64,

        Held = 128,
        Worn = 256,

        ConnectedPlayers = 512,

        Room = Contents | Links | Scenery,
        Player = Held | Worn,
        MudObject = In | On | Under | Behind,
        EveryMudObject = Contents | Links | Scenery | In | On | Under | Behind | Held | Worn,
    }

	public interface Container
	{
		void Remove(MudObject Object);
		void Add(MudObject Object, RelativeLocations Locations);
        EnumerateObjectsControl EnumerateObjects(RelativeLocations Locations, Func<MudObject, RelativeLocations, EnumerateObjectsControl> Callback);
        bool Contains(MudObject Object, RelativeLocations Locations);
        RelativeLocations LocationOf(MudObject Object);
        RelativeLocations LocationsSupported { get; }
        RelativeLocations DefaultLocation { get; }
	}
}
